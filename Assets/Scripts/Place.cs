using UnityEngine;

public class Place : MonoBehaviour {

    public PlayerColors PlayerColorStart = PlayerColors.None;
    public PlayerColors PlayerColorFinish = PlayerColors.None;
    public bool IsEnd = false;
    public int ID;
    public bool IsTaken { get; set; }

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

        switch (PlayerColorFinish) {
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
