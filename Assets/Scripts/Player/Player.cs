using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour, IPlayer {

    [SyncVar]
    public string PlayerName;
    [SyncVar]
    public Color PlayerColor;

    public event PlayerEvent OnPlayer;

    public bool IsReady { get; set; }

    private PlayerUI _playerUi;

    private readonly List<Pawn> _pawns = new List<Pawn>();
    private readonly List<Pawn> _activePawns = new List<Pawn>();
    private readonly List<Pawn> _pawnsOnBoardLocal = new List<Pawn>();

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

    public override void OnStartClient() {
        //Set player color for all other players
        TransmitColor();
    }

    [Command]
    private void CmdSpawnPawns() {
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
    private void CmdProvideColorToServer(Color c) {
        PlayerColor = c;
    }

    [ClientCallback]
    private void TransmitColor() {
        if (isLocalPlayer) {
            CmdProvideColorToServer(PlayerColor);
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
            //I know, I have sinned.. ..but it is only done once (everytime a turn starts).
            _playerUi = GameObject.Find("PlayerUI").GetComponent<PlayerUI>();
        }
        _playerUi.DiceButton.interactable = true;
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
        foreach (Pawn pawn in _pawnsOnBoardLocal) {
            if (pawn == null) return;
            if (!pawn.hasAuthority) return;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.gameObject == pawn.gameObject) {
                    //Hovering over object
                    pawn.ChangeColor(Pawn.ColorType.Highlighted);
                    if (Input.GetMouseButtonDown(0)) {

                        if (_pawnsOnBoardLocal.Any(p => p.Selected)) {
                            foreach (Pawn p in _pawnsOnBoardLocal.Where(p => p.Selected)) {
                                p.Selected = false;
                                p.ChangeColor(Pawn.ColorType.None);
                            }
                        }

                        if (!pawn.Selected) {
                            pawn.ChangeColor(Pawn.ColorType.Selected);
                            pawn.Selected = true;
                            _selectedPawn = pawn;
                            _playerUi.DiceButton.interactable = true;
                            CmdProvideSelectedPawnToServer(pawn.gameObject);
                        }
                    }
                }
            }
        }

    }

    [Command]
    private void CmdProvideSelectedPawnToServer(GameObject pawn) {
        _selectedPawn = pawn.GetComponent<Pawn>();
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
        int i = ;// GameManager.CalculateDiceNumber();

        if (_activePawns.Count == 0) {
            //No pawns on board yet.
           //if (i == 6) {
                //Player can put pawn on board
                Place startPosition = Board.Instance.StartPlaces[_myColor];

                _selectedPawn = _pawns.First(p => p.IsActive);
                _activePawns.Add(_selectedPawn);
                RpcAddActivePawnsToList(_selectedPawn.gameObject, true);
                MovePawnToPosition(startPosition.ID, true);
          //  }
        } else if (i == 6) {
            //Can put another pawn on board

            if (_activePawns.Count < 4) {
                Place startPosition = Board.Instance.StartPlaces[_myColor];
                _selectedPawn = _pawns.FirstOrDefault(p => !_activePawns.Contains(p) && p.IsActive);
                if (_selectedPawn == null) return;

                _activePawns.Add(_selectedPawn);

                RpcAddActivePawnsToList(_selectedPawn.gameObject, true);
                MovePawnToPosition(startPosition.ID, true);
            } else {
                MovePawnToPosition(i, false);
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
        } else {

            _selectedPawn.BoardPosition += position;
            _selectedPawn.BoardPosition = _selectedPawn.BoardPosition % Board.Instance.GetBoardPlaces.Count;

            if (_selectedPawn.BoardPosition == Board.Instance.EndPoints[_myColor].ID) {
                //On Finish 
                _activePawns.Remove(_selectedPawn);

                RpcAddActivePawnsToList(_selectedPawn.gameObject, false);
                Place endPosition = Board.Instance.FinishedPlaces.First(p => p.IsEnd && !p.IsTaken && p.PlayerColorFinish == _myColor);

                _selectedPawn.BoardPosition = -1;
                _selectedPawn.IsActive = false;
                _selectedPawn.UpdatePosition(endPosition.transform.position);
                endPosition.IsTaken = true;
                _selectedPawn = null;

                if (_pawns.TrueForAll(p => !p.IsActive)) {
                    RpcShowGameOverScreen(PlayerName);
                }
            }
        }

        if (_selectedPawn != null) {
            _selectedPawn.UpdatePosition();
        }
    }

    [ClientRpc]
    private void RpcShowGameOverScreen(string test) {
        _playerUi.ShowGameOverScreen(test + " WINS!");

        if (isLocalPlayer) {
            AccountManager.Instance.AddHighScore(1);
        } else {
            AccountManager.Instance.AddHighScore(0);
        }
        StartCoroutine(GoToLobby());
    }

    private IEnumerator GoToLobby() {
        yield return new WaitForSeconds(2);
        LobbyManager.s_Singleton.GoBackButton();
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
            _pawnsOnBoardLocal.Add(p.GetComponent<Pawn>());
        } else {
            _pawnsOnBoardLocal.Remove(p.GetComponent<Pawn>());
        }
    }
    private IEnumerator EndTurn() {
        yield return new WaitForSeconds(.1f);
        CmdEndTurn();
    }
}

public delegate void PlayerEvent(Player newPlayer);

public interface IPlayer {
    event PlayerEvent OnPlayer;

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