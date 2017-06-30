using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TurnManager : NetworkBehaviour {



    public Player CurrentPlayer { get; private set; }

    private Player[] players;

    private void Awake() {

    }

    private void Start() {
        FetchPlayers();
    }

    public void StartTurn(Player player) {
        SetPlayer(player);
        player.OnStartTurn();
    }

    public void UpdateTurn() {
        if (!isLocalPlayer) return;
        if (CurrentPlayer != null) {
            CurrentPlayer.Run();
        }
    }

    private void SetPlayer(Player newPlayer) {
        Player previousPlayer = null;
        if (CurrentPlayer != null) {
            previousPlayer = CurrentPlayer;
            CurrentPlayer.OnPlayer -= SetPlayer;
            CurrentPlayer.OnTurnComplete();
        }

        CurrentPlayer = newPlayer;

        if (previousPlayer != null)
            CurrentPlayer.OnTransition(previousPlayer);

        CurrentPlayer.OnPlayer += SetPlayer;

    }

    private void FetchPlayers() {
        players = GameManager.GetAllPlayers();
    }
}
