using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour {

    [HideInInspector]
    public PlayerUI Ui;

    [SerializeField]
    private GameObject _playerUiPrefab;
    [HideInInspector]
    public GameObject PlayerUiInstance;



    private void Start() {

        if (!isLocalPlayer) return;

        PlayerUiInstance = Instantiate(_playerUiPrefab);
        PlayerUiInstance.name = _playerUiPrefab.name;
        Ui = PlayerUiInstance.GetComponent<PlayerUI>();
        if (Ui == null)
            Debug.LogError("No PlayerUI component on PlayerUI prefab.");
        Ui.SetPlayer(GetComponent<Player>());


        var username = "Loading...";
        username = AccountManager.IsLoggedIn ? AccountManager.User.UserName : transform.name;

        CmdSetUsername(transform.name, username);
    }

    [Command]
    private void CmdSetUsername(string playerId, string userName) {

        var player = GameManager.GetPlayer(playerId);
        if (player == null) return;
        player.PlayerName = userName;
    }

    public override void OnStartClient() {
        base.OnStartClient();

        var netId = GetComponent<NetworkIdentity>().netId.ToString();
        var player = GetComponent<Player>();

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