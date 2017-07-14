using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public enum PlayerColors {
    Red,
    Black,
    Yellow,
    Green,
    None
}
public class Board : NetworkBehaviour {

    public Pawn PawnPrefab;

    private readonly List<Place> _boardPlaces = new List<Place>();
    public List<Place> GetBoardPlaces {
        get { return _boardPlaces; }
    }

    private readonly Dictionary<PlayerColors, Place> _startPlaces = new Dictionary<PlayerColors, Place>();
    public Dictionary<PlayerColors, Place> StartPlaces {
        get { return _startPlaces; }
    }

    private readonly Dictionary<PlayerColors, Transform> _bases = new Dictionary<PlayerColors, Transform>();

    public Dictionary<PlayerColors, Transform> Bases {
        get { return _bases; }
    }

    public static Board Instance = null;

    private void Awake() {
        if (Instance == null) Instance = this; else Destroy(this);
        
        FillPlaces();
        GetBases();
    }

    public override void OnStartServer() {

    }

    private void FillPlaces() {
        var placesTransform = transform.GetChild(0);
        int i = 0;
        foreach (Transform child in placesTransform) {
            var place = child.GetComponent<Place>();
            place.ID = i;
            if (place.PlayerColorStart != PlayerColors.None) {
                place.gameObject.name = "Start "+ "["+ place.PlayerColorStart +"]"+" [" + i + "]";
                _startPlaces.Add(place.PlayerColorStart, place);
            } else {
                place.gameObject.name = "Place [" + i + "]";
            }
            i++;
            _boardPlaces.Add(place);
        }
    }

    private void GetBases() {
        var basesTransform = transform.GetChild(1);

        for (int i = 0; i < basesTransform.childCount; i++) {
            _bases.Add((PlayerColors)i, basesTransform.GetChild(i));
        }
    }
}
