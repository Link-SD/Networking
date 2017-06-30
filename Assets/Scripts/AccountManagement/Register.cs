using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Register : MonoBehaviour {

    [Header("Register form References")]
    public InputField UsernameField;
    public InputField FirstNameField;
    public InputField LastNameField;
    public InputField EmailField;
    public Dropdown GenderField;
    public InputField PasswordField;
    public InputField RePasswordField;
    public Text ErrorText;
    public Button RegisterButton;

    private string _errorText = "";

    private string[] _formData;

    public void TryRegister() {
        _errorText = "";

        if (string.IsNullOrEmpty(UsernameField.text) ||
            string.IsNullOrEmpty(FirstNameField.text) ||
            string.IsNullOrEmpty(LastNameField.text) ||
            string.IsNullOrEmpty(EmailField.text) ||
            string.IsNullOrEmpty(PasswordField.text) ||
            string.IsNullOrEmpty(RePasswordField.text)) {
            _errorText = "You forgot to fill in one or more things!";
        }
        ErrorText.text = _errorText;


        if (_errorText != "") {
            return;
        }
        ToggleLoader(true);


        _formData = new[] {
            UsernameField.text,
            FirstNameField.text,
            LastNameField.text,
            EmailField.text,
            GenderField.options[GenderField.value].text,
            "1995-05-03",
            PasswordField.text,
            RePasswordField.text
        };

        StartCoroutine(WaitForResult());
    }

    private void ToggleLoader(bool show) {
        RegisterButton.transform.GetChild(0).gameObject.SetActive(!show);
        RegisterButton.transform.GetChild(1).gameObject.SetActive(show);
    }

    IEnumerator WaitForResult() {
        yield return StartCoroutine(AccountManager.Instance.TryRegister(_formData, ShowAlert));

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
