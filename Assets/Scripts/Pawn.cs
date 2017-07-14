using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class Pawn : NetworkBehaviour {

    public enum ColorType {
        Highlighted,
        Selected,
        None
    }

    private Renderer _renderer;

    [SyncVar]
    public Color PlayerColor;

    private Color _highLightedColor;
    private Color _selectedColor;

    public bool Selected { get; set; }

    [SyncVar]
    public int BoardPosition;

    public Player Owner { get; set; }

    private void Awake() {
        _renderer = GetComponent<Renderer>();
    }

    private void Start() {
        _renderer.material.color = PlayerColor;
        _highLightedColor = new Color(PlayerColor.r - .2f, PlayerColor.g - .2f, PlayerColor.b - .2f, PlayerColor.a);
        _selectedColor = new Color(PlayerColor.r - .3f, PlayerColor.g - .3f, PlayerColor.b - .3f, PlayerColor.a);
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

    //Just to make things not overcomplicated
    public void OnMouseExit() {
        if (!hasAuthority) return;

        if (!Selected)
            _renderer.material.color = PlayerColor;
    }


    public void UpdatePosition(int i) {
        /*Place place = Board.Instance.GetBoardPlaces.FirstOrDefault(p => p.ID == BoardPosition);
        if (place != null)
            transform.position = place.transform.position;
        */
        RpcUpdatePositionForClient(i);
    }
    [ClientRpc]
    public void RpcUpdatePositionForClient(int i) {
        print(BoardPosition);
        Place place = Board.Instance.GetBoardPlaces.FirstOrDefault(p => p.ID == i);
        if (place == null) return;
        
        while (transform.position != place.transform.position) {
            transform.position = Vector3.Lerp(transform.position, place.transform.position, 2 * Time.deltaTime);
        }
    }

}
