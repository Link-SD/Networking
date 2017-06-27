using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class GameManager : NetworkBehaviour {

    public static GameManager Instance;

    private const string PLAYER_ID_PREFIX = "Player ";

    private static readonly Dictionary<string, Player> Players = new Dictionary<string, Player>();


    void Awake() {
        if (Instance != null) {
            Debug.LogError("More than one GameManager in scene.");
        } else {
            Instance = this;
        }
    }


    #region Player tracking

    public static void RegisterPlayer(string _netID, Player _player) {
        string playerId = PLAYER_ID_PREFIX + _netID;

        if (Players.ContainsKey(playerId)) {
            Debug.LogError("There already is a player with this name. Overwriting..");
            UnRegisterPlayer(playerId);
        }

        Players.Add(playerId, _player);
        _player.transform.name = playerId;
    }

    public static void UnRegisterPlayer(string _playerID) {
        Players.Remove(_playerID);
    }

    public static Player GetPlayer(string _playerID) {
        return Players[_playerID];
    }

    public static Player[] GetAllPlayers() {
        return Players.Values.ToArray();
    }

    public static bool AllPlayersJoined() {
     //   print("Should be: " + NetworkManager.singleton.numPlayers + " " + GetAllPlayers().Length);
        return NetworkManager.singleton.numPlayers == GetAllPlayers().Length && GetAllPlayers().All(p => p.IsReady);
    }

    public void Update() {

    }
    #endregion

}
