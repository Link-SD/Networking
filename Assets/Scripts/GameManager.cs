using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class GameManager : NetworkBehaviour {

    public static GameManager Instance;

    private const string PLAYER_ID_PREFIX = "Player ";

    private static readonly Dictionary<string, Player> Players = new Dictionary<string, Player>();

    public bool IsGameReady { get; private set; }

    private TurnManager _turnManager;

    void Awake() {
        if (Instance != null) {
            Debug.LogError("More than one GameManager in scene.");
        } else {
            Instance = this;
        }
        _turnManager = GetComponent<TurnManager>();
    }

    public override void OnStartServer()
    {
        StartCoroutine(CheckForGameReady());
    }

    //Wrapper for the Turnmanager
    public void StartTurn(Player player)
    {
        _turnManager.StartTurn(player);
    }

    public Player GetCurrentPlayer() {
        return _turnManager.CurrentPlayer;
    }

    private int i = 1;
    public void EndTurn()
    {
        Player[] players = GetAllPlayers();
        int t = i % players.Length;
        if (t >= players.Length) return;
        if (_turnManager.CurrentPlayer == players[t]) return;

        _turnManager.StartTurn(players[t]);
        i++;
    }

    public void Update()
    {
        _turnManager.UpdateTurn();
    }

    private IEnumerator CheckForGameReady()
    {
        while (!AllPlayersJoined())
        {
            yield return null;
        }
        IsGameReady = true;

        Player startingPlayer = null;

        startingPlayer = GetAllPlayers()[0];

        if (startingPlayer == null)
        {
            Debug.LogError("Something went wrong when trying to assign the starting player");
        }
        else
        {
            StartTurn(startingPlayer);
        }
    }

    public static int CalculateDiceNumber() {
        System.Random r = new System.Random();
        return r.Next(1, 7); ;
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

    /// <summary>
    /// Returns a player by his player ID
    /// </summary>
    /// <param name="_playerID"></param>
    /// <returns></returns>
    public static Player GetPlayer(string _playerID) {
        return Players[_playerID];
    }

    /// <summary>
    /// Returns all players in this active game
    /// </summary>
    /// <returns></returns>
    public static Player[] GetAllPlayers() {
        return Players.Values.ToArray();
    }

    /// <summary>
    /// Returns all players in this active game as dictionary
    /// </summary>
    /// <returns></returns>
    public static Dictionary<string, Player> GetAllPlayersAsDictionary() {
        return Players;
    }

    public static bool AllPlayersJoined() {
       //  return NetworkManager.singleton.numPlayers == GetAllPlayers().Length;
        return NetworkManager.singleton.numPlayers == GetAllPlayers().Length && GetAllPlayers().All(p => !string.IsNullOrEmpty(p.PlayerName));

    }


    #endregion

    /// <summary>
    /// Helper method that picks a random set of values from a dictionary
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <returns></returns>
    public IEnumerable<TValue> RandomValuesFromDictionary<TKey, TValue>(IDictionary<TKey, TValue> dict) {
        System.Random rand = new System.Random();
        List<TValue> values = dict.Values.ToList();
        int size = dict.Count;
        while (true)
            yield return values[rand.Next(size)];
    }
}
