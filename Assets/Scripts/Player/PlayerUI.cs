using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerUI : NetworkBehaviour {

    [SerializeField]
    private Text _playerNameText;

    [SerializeField]
    private Image _playerColor;

    [SerializeField]
    private Text _currentPlayer;

    [SerializeField]
    private Image _currentPlayerColor;

    [SerializeField]
    private Button _diceButton;

    [SerializeField]
    private Text _diceButtonText;

    public Button DiceButton { get { return _diceButton; } }

    private Player _player;

    public void SetPlayer(Player _player) {
        this._player = _player;

        StartCoroutine(WaitForReady());
        _diceButton.interactable = false;
    }

    private IEnumerator WaitForReady() {
        while (string.IsNullOrEmpty(_player.PlayerName)) {
            yield return null;
        }
        _playerNameText.text = _player.PlayerName;
        _playerColor.color = _player.PlayerColor;
        _diceButton.onClick.AddListener(Roll);
    }

    private void Roll() {
        _player.AskToRollDice();
    }

    public void SetDiceNumber(int i) {
        _diceButtonText.text = i.ToString();
    }

    public void SetCurrentPlayer(string name) {
        _currentPlayer.text = name;
    }

}
