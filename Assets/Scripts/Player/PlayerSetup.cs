using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour {

    [SerializeField] private Behaviour[] componentsToDisable;

    [SerializeField] private string remoteLayerName = "RemotePlayer";

    //  [SerializeField]
    //  GameObject playerGraphics;

    //   [SerializeField]
    //  GameObject playerUIPrefab;
    //  [HideInInspector]
    // public GameObject playerUIInstance;

    private void Start() {
        if (!isLocalPlayer) return;

        string _username = "Loading...";
        if (AccountManager.IsLoggedIn)
            _username = AccountManager.User.UserName;
        else
            _username = transform.name;
        CmdSetUsername(transform.name, _username);
    }

    [Command]
    private void CmdSetUsername(string playerID, string username) {
        Player player = GameManager.GetPlayer(playerID);
        if (player == null) return;
        Debug.Log(username + " has joined!");
        player.PlayerName = username;
        player.IsReady = true;

        player.Init();
    }


    public override void OnStartClient() {
        base.OnStartClient();
        string netId = GetComponent<NetworkIdentity>().netId.ToString();
        Player player = GetComponent<Player>();

        GameManager.RegisterPlayer(netId, player);
    }


    // When we are destroyed
    private void OnDisable() {
        GameManager.UnRegisterPlayer(transform.name);
    }
}