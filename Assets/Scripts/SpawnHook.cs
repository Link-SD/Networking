using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class SpawnHook : LobbyHook {

    private ConnectionManager _connectionManager;

    private void Start() {
        _connectionManager = GetComponent<ConnectionManager>();
    }

    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) {

        base.OnLobbyServerSceneLoadedForPlayer(manager, lobbyPlayer, gamePlayer);

        //Player player = gamePlayer.GetComponent<Player>();

        gamePlayer.transform.name = lobbyPlayer.GetComponent<LobbyPlayer>().playerName;
        //player.Init();
    }
    
}
