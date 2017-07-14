using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Place : MonoBehaviour {

    public PlayerColors PlayerColorStart = PlayerColors.None;
    public int ID;

    private void OnDrawGizmosSelected() {

        switch (PlayerColorStart) {
            case PlayerColors.None:
                Gizmos.color = Color.gray;
                break;
            case PlayerColors.Black:
                Gizmos.color = Color.black;
                break;
            case PlayerColors.Green:
                Gizmos.color = Color.green;
                break;
            case PlayerColors.Red:
                Gizmos.color = Color.red;
                break;
            case PlayerColors.Yellow:
                Gizmos.color = Color.yellow;
                break;
            default:
                Gizmos.color = Color.gray;
                break;
        }
        Gizmos.DrawSphere(transform.position, .5f);
    }
}
