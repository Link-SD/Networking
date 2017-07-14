using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class Dice : MonoBehaviour {

    public Button DiceButton;

    private Text resulText;

	// Use this for initialization
	private void Awake() {
	    resulText = DiceButton.transform.GetChild(1).GetComponent<Text>();
	    resulText.text = "Throw";
	}
	
	// Update is called once per frame
	void Update () {
	 
	}

    public void RollDice() {
        var rng = new Random();
        var numberThrown = rng.Next(1, 7);
        
        resulText.text = numberThrown.ToString();
        
    }

    //TODO: If dice is rolled, wait for the pawn to reach the set place.
}
