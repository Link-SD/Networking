using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

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

    [SerializeField]
    private GameObject _gameOverScreen;

    [SerializeField]
    private GameObject _uiScreen;

    [SerializeField]
    private Text _gameOverText;

    public Button DiceButton { get { return _diceButton; } }

    private Player _player;

    private void Start() {
        _gameOverScreen.SetActive(false);
        _uiScreen.SetActive(true);
    }

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

    public void ShowGameOverScreen(string msg) {
        _gameOverScreen.SetActive(true);
        _gameOverText.text = msg;
        _uiScreen.SetActive(false);
    }
}
