using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class Pawn : NetworkBehaviour {

    public enum ColorType {
        Highlighted,
        Selected,
        None
    }

    [SyncVar]
    public Color PlayerColor;

    public bool Selected { get; set; }
    public bool IsActive { get; set; }

    [SyncVar(hook = "OnBoardPositionChange")]
    public int BoardPosition;

    public Player Owner { get; set; }

    private Renderer _renderer;
    private Color _highLightedColor;
    private Color _selectedColor;

    private void Awake() {
        _renderer = GetComponent<Renderer>();
        IsActive = true;
    }

    private void Start() {
        _renderer.material.color = PlayerColor;
        _highLightedColor = new Color(PlayerColor.r - .4f, PlayerColor.g - .4f, PlayerColor.b - .4f, PlayerColor.a);
        _selectedColor = new Color(PlayerColor.r - .7f, PlayerColor.g - .7f, PlayerColor.b - .7f, PlayerColor.a);
    }

    public void ChangeColor(Color color) {
        _renderer.material.color = color;
    }

    public void ChangeColor(ColorType type) {

        switch (type) {
            case ColorType.Highlighted:
                _renderer.material.color = _highLightedColor;
                break;
            case ColorType.Selected:
                _renderer.material.color = _selectedColor;
                break;
            case ColorType.None:
                _renderer.material.color = PlayerColor;
                break;
        }
    }

    /// <summary>
    /// Called on BoardPosition Change
    /// </summary>
    /// <param name="i"></param>
    [Server]
    private void OnBoardPositionChange(int i) {
        RpcUpdateBoardPositionClient(i);
    }

    [ClientRpc]
    private void RpcUpdateBoardPositionClient(int i) {
        BoardPosition = i;
    }

    //Just to make things not overcomplicated
    public void OnMouseExit() {
        if (!hasAuthority) return;

        if (!Selected)
            _renderer.material.color = PlayerColor;
    }

    public void UpdatePosition() {
        RpcUpdatePositionForClient();
    }

    public void UpdatePosition(Vector3 pos) {
        RpcPositionPawn(pos);
    }

    [ClientRpc]
    private void RpcPositionPawn(Vector3 pos) {
        transform.position = pos;
    }

    [ClientRpc]
    private void RpcUpdatePositionForClient() {
        StartCoroutine(Move());
    }

    private Place previousPosition = null;
    private IEnumerator Move() {
        float timeToStart = Time.time;
        List<Place> places = Board.Instance.GetBoardPlaces;
        Place finalPosition = places.First(p => p.ID == BoardPosition);
        /*
        if (previousPosition != null) {
            IList<Place> placesToGo = places.Where(p => p.ID > previousPosition.ID &&p.ID < finalPosition.ID + 1).ToList();
            int currentIndex = 0;
            while (Vector3.Distance(transform.position, placesToGo[currentIndex].transform.position) > 0.05f) {

                transform.position = Vector3.Lerp(transform.position, placesToGo[currentIndex].transform.position,
                    (Time.time - timeToStart) * 1f);

                if ((Vector3.Distance(transform.position, placesToGo[currentIndex].transform.position) <= 0.06f)) {
                    if (currentIndex < placesToGo.Count - 1) {
                        currentIndex++;
                        currentIndex = currentIndex % placesToGo.Count;
                        Debug.Log(currentIndex);
                    }
                }
                yield return null;
            }


        } */
        while (Vector3.Distance(transform.position, finalPosition.transform.position) > 0.05f) {

            transform.position = Vector3.Lerp(transform.position, finalPosition.transform.position,
                (Time.time - timeToStart) * 2.5f);
            yield return null;
        }
        // }
        //previousPosition = finalPosition;
    }
}

