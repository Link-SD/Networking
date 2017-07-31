using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Highscores;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreManager : MonoBehaviour {

    public enum SortType {
        UserOnly,
        All,
        DateUp,
        DateDown,
        NameUp,
        NameDown
    }

    [SerializeField]
    private HighScoreEntry _entryPrefab;
    public Transform HighScoreContentParent;
    public Text ErrorText;

    private const string HIGHSCORE_URL = "http://dev.sandordaemen.nl/kernmodule/php/highscore_handler.php";

    public HighScoreData[] HighScores { get; private set; }

    public static HighScoreManager Instance;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Start() {
        StartCoroutine(WaitForResult());
    }

    public void Sort(SortType type) {
        StartCoroutine(WaitForResult(type));
    }

    private IEnumerator WaitForResult(SortType type = SortType.All) {
        yield return StartCoroutine(TryGetHighScores(DoneLoading, type));
    }

    private void DoneLoading(string msg, MsgType msgType) {
        FillHighscoreUI();

        switch (msgType) {
            case MsgType.Error:
                ErrorText.color = Color.red;
                break;
            case MsgType.Success:
                ErrorText.color = Color.green;
                break;
            case MsgType.Warning:
                ErrorText.color = Color.yellow;
                break;
            case MsgType.Generic:
                ErrorText.color = Color.blue;
                break;
            default:
                ErrorText.color = Color.black;
                break;
        }
        ErrorText.text = msg;
    }


    public void FillHighscoreUI() {

        foreach (Transform child in HighScoreContentParent) {
            Destroy(child.gameObject);
        }

        if (HighScores == null) return;

        foreach (HighScoreData highScore in HighScores) {
            HighScoreEntry entry = Instantiate(_entryPrefab, HighScoreContentParent, false);
            entry._IndexText.text = highScore.Index.ToString();
            entry._playerImage.sprite = null;
            entry._userNameText.text = highScore.UserName;
            entry._userScoreText.text = "WINS: "+highScore.Score.ToString();
            entry._dateText.text = highScore.Date;
        }
    }

    public IEnumerator TryGetHighScores(AccountManager.AlertHandler alert, SortType type) {
        WWWForm form = new WWWForm();

        form.AddField("user_id", AccountManager.User.ID);
        form.AddField("game_id", 5);
        form.AddField("session_id", AccountManager.User.SessionID);
        form.AddField("hidden-field", "hidden");
        form.AddField("sortby", type.ToString());

        WWW www = new WWW(HIGHSCORE_URL, form);

        yield return StartCoroutine(DoGetHighScores(www, alert));
    }

    private IEnumerator DoGetHighScores(WWW www, AccountManager.AlertHandler alert) {
        yield return www;
        if (www.error == null) {
            string[] receivedData = Regex.Split(www.text, "<>");
            string[] highScores = Regex.Split(receivedData[1], "\n");
            string errorData = receivedData[2];

            if (errorData == "") {
                //Regex adds one extra element that I don't need
                HighScores = new HighScoreData[highScores.Length - 1];
                for (int i = 0; i < HighScores.Length; i++) {
                    string[] highscore = Regex.Split(highScores[i], "\\|");
                    HighScores[i] = new HighScoreData(i + 1, int.Parse(highscore[0]), highscore[1], highscore[3]);
                }
                alert("Success", MsgType.Success);
            } else {
                alert(errorData, MsgType.Error);
            }
        } else {
            if (www.error == "Couldn't resolve host name") {
                alert("There appears to be no internet connection. Please connect to the internet.", MsgType.Warning);
            } else {
                alert(www.error, MsgType.Error);
            }
        }
    }


    public void AddHighScore(int Score) {

    }



    public struct HighScoreData {

        public int Index { get; private set; }
        public int Score { get; private set; }
        public string UserName { get; private set; }
        public string Date { get; private set; }

        public HighScoreData(int index, int score, string userName, string date) : this() {
            Index = index;
            Score = score;
            UserName = userName;
            Date = date;
        }
    }
}
