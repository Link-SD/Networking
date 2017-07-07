using UnityEngine;

public class TurnManager : MonoBehaviour {

    public Player CurrentPlayer { get; private set; }

    public void StartTurn(Player player) {
        SetPlayer(player);
        player.OnStartTurn();
    }

    public void UpdateTurn() {
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
}
