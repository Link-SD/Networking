using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour, IPlayer {

    [SyncVar]
    public string PlayerName;
    [SyncVar]
    public Color PlayerColor;

    public event PlayerEvent OnPlayer;
    public string GetPlayerName { get; private set; }

    public bool IsReady { get; set; }

    private PlayerUI _playerUi;

    private readonly List<Pawn> _pawns = new List<Pawn>();
    private List<Pawn> _pawnsOnBoard = new List<Pawn>();
    private Pawn _selectedPawn = null;
    [SerializeField]
    private GameObject _pawnPrefab;

    private PlayerColors _myColor = PlayerColors.None;

    private void Start() {
        if (isLocalPlayer) {
            SetColorEnum();
            StartCoroutine(WaitForConnectionEstablished());
        }
    }

    private void SetColorEnum() {
        if (PlayerColor == Color.red) {
            _myColor = PlayerColors.Red;
        } else if (PlayerColor == Color.black) {
            _myColor = PlayerColors.Black;
        } else if (PlayerColor == Color.green) {
            _myColor = PlayerColors.Green;
        } else if (PlayerColor == Color.yellow) {
            _myColor = PlayerColors.Yellow;
        } else {
            _myColor = PlayerColors.None;
        }
        int i = (int)_myColor;
        CmdSendEnumToServer(i);
    }

    [Command]
    private void CmdSendEnumToServer(int i) {
        _myColor = (PlayerColors)i;
    }

    private IEnumerator WaitForConnectionEstablished() {
        if (isServer) {
            while (!connectionToClient.isReady) {
                yield return null;
            }
        } else {
            while (!connectionToServer.isReady) {
                yield return null;
            }
        }
        CmdSpawnPawns();
    }

    public void SetupPlayer() {
        if (isLocalPlayer) {
            GetPlayerName = PlayerName;
        }
    }

    public override void OnStartClient() {
        //Set player color for all other players
        TransmitColor();
    }

    [Command]
    void CmdSpawnPawns() {
        for (int i = 0; i < 4; i++) {
            var go = Instantiate(
                _pawnPrefab,
                GetPawnStartPosition(i),
                Quaternion.identity, transform);
            Pawn pawn = go.GetComponent<Pawn>();
            pawn.Owner = this;
            pawn.PlayerColor = PlayerColor;
            NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
            _pawns.Add(pawn);
            RpcAddPawnsToList(pawn.gameObject);
        }
    }

    [ClientRpc]
    private void RpcAddPawnsToList(GameObject p) {
        _pawns.Add(p.GetComponent<Pawn>());
    }


    private Vector3 GetPawnStartPosition(int i) {
        Transform t = Board.Instance.Bases[_myColor];
        return t.GetChild(i).position;
    }


    [Command]
    void Cmd_ProvideColorToServer(Color c) {
        PlayerColor = c;
    }

    [ClientCallback]
    void TransmitColor() {
        if (isLocalPlayer) {
            Cmd_ProvideColorToServer(PlayerColor);
        }
    }

    #region Turn Handling

    [Server]
    public void OnStartTurn() {
        RpcChangeCurrentPlayer();
        RpcStartTurn();
    }

    [ClientRpc]
    private void RpcStartTurn() {
        if (!isLocalPlayer) return;
        if (_playerUi == null) {
            //I know, I have sinned..
            _playerUi = GameObject.Find("PlayerUI").GetComponent<PlayerUI>();
        }
        _playerUi.DiceButton.interactable = _pawnsOnBoard.Count == 0;
    }

    [ClientRpc]
    private void RpcChangeCurrentPlayer() {
        if (!isLocalPlayer) return;
        CmdAskForCurrentPlayer();
    }

    [Command]
    private void CmdAskForCurrentPlayer() {
        RpcGetCurrentPlayerForClient(GameManager.Instance.GetCurrentPlayer().PlayerName);
    }

    [ClientRpc]
    private void RpcGetCurrentPlayerForClient(string currentPlayerName) {
        if (_playerUi == null) {
            //I know, I have sinned..
            _playerUi = GameObject.Find("PlayerUI").GetComponent<PlayerUI>();
        }
        _playerUi.SetCurrentPlayer(currentPlayerName);
    }

    [Server]
    public void OnTransition(Player previousPlayer) {
        RpcOnTransition(previousPlayer.gameObject);
    }

    [ClientRpc]
    private void RpcOnTransition(GameObject prevPlayerGameObject) {
        if (!isLocalPlayer) return;
        Player prevPlayer = prevPlayerGameObject.GetComponent<Player>();
        //  print("Transitioned from " + prevPlayer.PlayerName + " To " + PlayerName);
    }

    [Server]
    public void Run() {
        RpcRun();
    }

    [ClientRpc]
    private void RpcRun() {
        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.T)) {
            CmdEndTurn();
        }

        foreach (Pawn pawn in _pawnsOnBoard) {
            if (pawn == null) return;
            if (!pawn.hasAuthority) return;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.gameObject == pawn.gameObject) {
                    //Hovering over object
                    pawn.ChangeColor(Pawn.ColorType.Highlighted);
                    if (Input.GetMouseButtonDown(0)) {

                        if (_pawnsOnBoard.Any(p => p.Selected)) {
                            foreach (Pawn p in _pawnsOnBoard.Where(p => p.Selected)) {
                                p.Selected = false;
                                p.ChangeColor(Pawn.ColorType.None);
                            }
                        }

                        if (!pawn.Selected) {
                            pawn.ChangeColor(Pawn.ColorType.Selected);
                            pawn.Selected = true;
                            _selectedPawn = pawn;
                            _playerUi.DiceButton.interactable = true;
                        }
                    }
                }
            }
        }

    }


    [Command]
    private void CmdEndTurn() {
        GameManager.Instance.EndTurn();
    }

    [Server]
    public void OnTurnComplete() {
        RpcOnTurnComplete();
    }
    [ClientRpc]
    private void RpcOnTurnComplete() {
        if (!isLocalPlayer) return;
        _playerUi.DiceButton.interactable = false;
    }
    #endregion Turn Handling


    public void AskToRollDice() {
        if (isLocalPlayer)
            CmdRollDice();
    }

    [Command]
    private void CmdRollDice() {
        if (!isServer)
            return;


        if (!GameManager.Instance.GetCurrentPlayer() == this) return;
        int i = GameManager.CalculateDiceNumber();

        if (_pawnsOnBoard.Count == 0) {
            //No pawns on board yet.
            if (i == 6) {
                //Player can put pawn on board
                Place startPosition = Board.Instance.StartPlaces[_myColor];

                
                _selectedPawn = _pawns[Random.Range(0, _pawns.Count)];
                _pawnsOnBoard.Add(_selectedPawn);
                RpcAddActivePawnsToList(_pawnsOnBoard[0].gameObject, true);
                MovePawnToPosition(startPosition.ID, true);
            }
        } else {
            MovePawnToPosition(i, false);
        }

        RpcUpdateDiceNumber(i);
        StartCoroutine(EndTurn());
    }

    [Server]
    private void MovePawnToPosition(int position, bool placeOnBoard) {

        if (placeOnBoard) {
            _selectedPawn.BoardPosition = position;
        }
        else {
            _selectedPawn.BoardPosition += position;
        }


        _selectedPawn.UpdatePosition(position);
        //  RpcMoveToPosition(position);
    }

    [ClientRpc]
    private void RpcMoveToPosition(int position) {
        if (isLocalPlayer)
            _selectedPawn.BoardPosition = position;
    }
    [ClientRpc]
    private void RpcUpdateDiceNumber(int position) {
        if (!isLocalPlayer) return;
        _playerUi.SetDiceNumber(position);
        _playerUi.DiceButton.interactable = false;

    }

    [ClientRpc]
    private void RpcAddActivePawnsToList(GameObject p, bool add) {
        if (add) {
            _pawnsOnBoard.Add(p.GetComponent<Pawn>());
        } else {
            _pawnsOnBoard.Remove(p.GetComponent<Pawn>());
        }
    }
    private IEnumerator EndTurn() {
        yield return new WaitForSeconds(1);
        CmdEndTurn();
    }
}

public delegate void PlayerEvent(Player newPlayer);

public interface IPlayer {
    event PlayerEvent OnPlayer;
    string GetPlayerName { get; }

    /// <summary>
    /// Callback called when the turn has started.
    /// </summary>
    void OnStartTurn();
    /// <summary>
    /// Callback called when the players switch turns. 
    /// Access to the previous player
    /// </summary>
    /// <param name="previousPlayer"></param>
    void OnTransition(Player previousPlayer);
    /// <summary>
    /// Custom update method for efficientcy. Only the player with the active turn will be updated.
    /// </summary>
    void Run();
    /// <summary>
    /// Callback called when the player completes it's turn.
    /// </summary>
    void OnTurnComplete();
}