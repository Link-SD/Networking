using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInfoLobby : MonoBehaviour {
    public Text nameText;

    private void Start() {
        if (!AccountManager.IsLoggedIn || AccountManager.Instance == null) {
            gameObject.SetActive(false);
            return;
        }
        nameText.text = AccountManager.User.FirstName + " (" + AccountManager.User.UserName + ")";
    }

    public void HandleLogout() {
        AccountManager.Instance.LogOut();
    }
}
