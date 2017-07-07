using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour, IPlayer {

    [SyncVar] public string PlayerName;
    [SyncVar] public Color PlayerColor;

    public event PlayerEvent OnPlayer;
    public string GetPlayerName { get; private set; }

    public bool IsReady { get; set; }

    private void Awake() {

    }

    public void SetupPlayer() {
        if (isLocalPlayer)
        {
            GetPlayerName = PlayerName;
        }
    }

    public override void OnStartLocalPlayer() {
        //Set own player color
        if (isLocalPlayer)
            GetComponent<MeshRenderer>().material.color = PlayerColor;

    }

    public override void OnStartClient()
    {
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
    public void OnStartTurn()
    {
        print("Turn Started for: " + PlayerName);
    }

    public void OnTransition(Player previousPlayer)
    {
        print("Transitioned from: " + previousPlayer.PlayerName +" to: " + PlayerName);
    }

    /// <summary>
    /// This is essentially the Update method
    /// </summary>
    public void Run()
    {
        if (!isLocalPlayer) return;

        print("Running: " + PlayerName);

        if (Input.GetKeyDown(KeyCode.T))
        {
            //GameManager.Instance.EndTurn();
            Cmd_EndTurn();
        }
    }

    [Command]
    private void Cmd_EndTurn()
    {
        GameManager.Instance.EndTurn();
    }

    public void OnTurnComplete()
    {
        print("Turn Complete: " + PlayerName);
    }
}

public delegate void PlayerEvent(Player newPlayer);

public interface IPlayer
{
    event PlayerEvent OnPlayer;
    string GetPlayerName { get; }

    void OnStartTurn();
    void OnTransition(Player previousPlayer);
    void Run();
    void OnTurnComplete();

}
