using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    public string PlayerName { get; set; }
    public Color PlayerColor;

    public bool IsReady { get; set; }

    private void Awake() {
        
    }   

    public void Init() {
  
        RpcSyncLocalData();
    }



    [ClientRpc]
    public void RpcSyncLocalData() {
        print(PlayerName);
    }

}
