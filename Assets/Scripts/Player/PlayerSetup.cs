using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour {



    [SerializeField]
    private GameObject _playerUIPrefab;
    [HideInInspector]
    public GameObject playerUIInstance;
    [HideInInspector]
    public PlayerUI ui;


    private void Start() {

        if (!isLocalPlayer) return;

        playerUIInstance = Instantiate(_playerUIPrefab);
        playerUIInstance.name = _playerUIPrefab.name;
        ui = playerUIInstance.GetComponent<PlayerUI>();
        if (ui == null)
            Debug.LogError("No PlayerUI component on PlayerUI prefab.");
        ui.SetPlayer(GetComponent<Player>());


        var username = "Loading...";
        username = AccountManager.IsLoggedIn ? AccountManager.User.UserName : transform.name;

        CmdSetUsername(transform.name, username);
        GetComponent<Player>().SetupPlayer();
    }

    [Command]
    private void CmdSetUsername(string playerId, string userName) {

        Player player = GameManager.GetPlayer(playerId);
        if (player == null) return;
        player.PlayerName = userName;
    }

    [ClientRpc]
    private void RpcSetUsername(string playerId, string userName) {
        if (!isLocalPlayer) return;
        Player player = GameManager.GetPlayer(playerId);
        if (player == null) return;
        player.PlayerName = userName;
    }

    public override void OnStartClient() {
        base.OnStartClient();

        string netId = GetComponent<NetworkIdentity>().netId.ToString();
        Player player = GetComponent<Player>();

        GameManager.RegisterPlayer(netId, player);
    }

    private void OnDisable() {
        GameManager.UnRegisterPlayer(transform.name);
    }

    public override void OnNetworkDestroy() {
        GameManager.UnRegisterPlayer(transform.name);
        base.OnNetworkDestroy();
    }
}