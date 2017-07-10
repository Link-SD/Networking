using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour, IPlayer {

    [SyncVar]
    public string PlayerName;
    [SyncVar]
    public Color PlayerColor;
    private bool _turnEnded;

    public event PlayerEvent OnPlayer;
    public string GetPlayerName { get; private set; }

    public bool IsReady { get; set; }

    private PlayerUI _playerUi;

    private void Start() {
        if (!isLocalPlayer) return;
    }


    public void SetupPlayer() {
        if (isLocalPlayer) {
            GetPlayerName = PlayerName;
        }
    }

    public override void OnStartLocalPlayer() {
        //Set own player color
        if (isLocalPlayer)
            GetComponent<MeshRenderer>().material.color = PlayerColor;

    }

    public override void OnStartClient() {
        //Set player color for all other players
        TransmitColor();
        if (!isLocalPlayer)
            GetComponentInChildren<Renderer>().material.color = PlayerColor;
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

    //Turn Handling
    [Server]
    public void OnStartTurn() {
        RpcStartTurn();
        RpcChangeCurrentPlayer();
    }

    [ClientRpc]
    private void RpcStartTurn() {
        if (!isLocalPlayer) return;

        //     print("OnStartTurn CLIENT " + PlayerName);
    }

    [ClientRpc]
    private void RpcChangeCurrentPlayer() {
        CmdAskForCurrentPlayer();
    }

    [Command]
    private void CmdAskForCurrentPlayer() {
        RpcGetCurrentPlayerForClient(GameManager.Instance.GetCurrentPlayer().PlayerName);
    }

    [ClientRpc]
    private void RpcGetCurrentPlayerForClient(string name) {
        if (_playerUi == null) {
            //I know, I have sinned..
            _playerUi = GameObject.Find("PlayerUI").GetComponent<PlayerUI>();
        }
       _playerUi.SetCurrentPlayer(name);
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
        //   print("OnTurnComplete CLIENT " + PlayerName);
    }
}

public delegate void PlayerEvent(Player newPlayer);

public interface IPlayer {
    event PlayerEvent OnPlayer;
    string GetPlayerName { get; }

    void OnStartTurn();
    void OnTransition(Player previousPlayer);
    void Run();
    void OnTurnComplete();

}
