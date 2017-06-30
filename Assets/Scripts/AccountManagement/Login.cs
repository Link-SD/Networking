using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour {

    [Header("Login form References")]
    public InputField UserNameText;
    public InputField PassWordText;
    public Button LoginButton;
    public Text ErrorText;
    private string _errorText = "";


    public void TryLogin() {
        _errorText = "";
        if (UserNameText.text == "" || PassWordText.text == "") {
            _errorText = "You forgot to fill in one or more things!";

        }
        ErrorText.text = _errorText;
        
        if (_errorText != "") {
            return;
        }
        ToggleLoader(true);
        StartCoroutine(WaitForResult());
    }

    private void ToggleLoader(bool show)
    {
            LoginButton.transform.GetChild(0).gameObject.SetActive(!show);
            LoginButton.transform.GetChild(1).gameObject.SetActive(show);
    }

    IEnumerator WaitForResult() {
        yield return StartCoroutine(AccountManager.Instance.TryLogin(UserNameText.text, PassWordText.text, ShowAlert));
    }

    private void ShowAlert(string msg, MsgType msgType) {
        switch (msgType) {
            case MsgType.Error:
                ErrorText.color = Color.red;
                ToggleLoader(false);
                break;
            case MsgType.Success:
                ErrorText.color = Color.green;
                break;
            case MsgType.Warning:
                ErrorText.color = Color.yellow;
                ToggleLoader(false);
                break;
            case MsgType.Generic:
                ErrorText.color = Color.blue;
                ToggleLoader(false);
                break;
            default:
                ErrorText.color = Color.blue;
                break;
        }
        ErrorText.text = msg;
    }
}
