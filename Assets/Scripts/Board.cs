using System.Collections.Generic;
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

    private readonly Dictionary<PlayerColors, Place> _endPoints = new Dictionary<PlayerColors, Place>();
    public Dictionary<PlayerColors, Place> EndPoints {
        get { return _endPoints; }
    }

    private readonly List<Place> _finishedPlaces = new List<Place>();
    public List<Place> FinishedPlaces {
        get { return _finishedPlaces; }
    }

    private readonly Dictionary<PlayerColors, Transform> _bases = new Dictionary<PlayerColors, Transform>();
    public Dictionary<PlayerColors, Transform> Bases {
        get { return _bases; }
    }

    public static Board Instance = null;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(this);
        }

        FillPlaces();
        GetBases();
    }

    private void FillPlaces() {
        var placesTransform = transform.Find("Places");

        var i = 0;
        foreach (Transform child in placesTransform) {
            var place = child.GetComponent<Place>();
            place.ID = i;
            if (place.PlayerColorStart != PlayerColors.None) {
                place.gameObject.name = "Start " + "[" + place.PlayerColorStart + "]" + " [" + place.ID + "]";
                _startPlaces.Add(place.PlayerColorStart, place);
            } else if (place.PlayerColorFinish != PlayerColors.None) {
                place.gameObject.name = "Finish " + "[" + place.PlayerColorFinish + "]" + " [" + place.ID + "]";
                _endPoints.Add(place.PlayerColorFinish, place);
            } else {
                place.gameObject.name = "Place [" + i + "]";
            }
            i++;
            _boardPlaces.Add(place);
        }

        var finishedPlacesTransform = transform.Find("FinishedPlaces");

        foreach (Transform t in finishedPlacesTransform) {
            foreach (Transform child in t) {
                child.name = "[" + t.name + "]" + " Finish";
                var place = child.GetComponent<Place>();
                _finishedPlaces.Add(place);
            }
        }
    }

    private void GetBases() {
        var basesTransform = transform.GetChild(1);

        for (int i = 0; i < basesTransform.childCount; i++) {
            _bases.Add((PlayerColors)i, basesTransform.GetChild(i));
        }
    }
}
