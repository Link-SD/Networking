using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UIManager : NetworkBehaviour {
    public GameObject namePrefab;
    public Transform parent;


    [Header("Player Info References")]
    public Text PlayerNameText;
    public Text PlayerColorText;
    // Use this for initialization

    void Start() {
           
        StartCoroutine(Init());
    }

    private IEnumerator Init() {
        while (!GameManager.AllPlayersJoined()) {
            yield return null;
        }


        foreach (Player player in GameManager.GetAllPlayers()) {
            GameObject go = GameObject.Instantiate(namePrefab, parent, false);
            Text text = go.GetComponentInChildren<Text>();
            text.text = player.PlayerName;
        }

    }

    [Client]
    public void SetUserData() {
        
    } 
   
}
