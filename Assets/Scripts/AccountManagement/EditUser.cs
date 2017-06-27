using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditUser : MonoBehaviour {

    [Header("Register form References")]
    public InputField UsernameField;
    public InputField FirstNameField;
    public InputField LastNameField;
    public InputField EmailField;
    public Dropdown GenderField;
    public Text ErrorText;
    private string _errorText = "";

    private string[] _formData;

    public void Awake() {
        GetUserData();
    }

    private void GetUserData() {
        UsernameField.text = AccountManager.User.UserName;
        FirstNameField.text = AccountManager.User.FirstName;
        LastNameField.text = AccountManager.User.LastName;
        EmailField.text = AccountManager.User.Email;
        GenderField.value = GenderField.options.FindIndex(o => o.text == AccountManager.User.Gender);
    }

    public void TryUpdate() {
        _errorText = "";

        if (string.IsNullOrEmpty(UsernameField.text) ||
            string.IsNullOrEmpty(FirstNameField.text) ||
            string.IsNullOrEmpty(LastNameField.text) ||
            string.IsNullOrEmpty(EmailField.text)) {
            _errorText = "You forgot to fill in one or more things!";
        }
        ErrorText.text = _errorText;


        if (_errorText != "") {
            return;
        }

        _formData = new[] {
            UsernameField.text,
            FirstNameField.text,
            LastNameField.text,
            EmailField.text,
            GenderField.options[GenderField.value].text,
            "1995-05-03",
        };

        StartCoroutine(WaitForResult());
    }

    IEnumerator WaitForResult() {
        yield return StartCoroutine(AccountManager.Instance.TryUpdate(_formData, ShowAlert));

    }

    private void ShowAlert(string msg, MsgType msgType) {

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
                ErrorText.color = Color.blue;
                break;
        }

        ErrorText.text = msg;
    }
}
