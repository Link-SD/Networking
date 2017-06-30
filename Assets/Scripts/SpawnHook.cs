using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class SpawnHook : LobbyHook {

    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) {

        base.OnLobbyServerSceneLoadedForPlayer(manager, lobbyPlayer, gamePlayer);

        gamePlayer.transform.name = lobbyPlayer.GetComponent<LobbyPlayer>().playerName;
        gamePlayer.GetComponent<Player>().PlayerColor = lobbyPlayer.GetComponent<LobbyPlayer>().playerColor;
    }
    
}
