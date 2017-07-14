using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum MsgType {
    Error,
    Success,
    Warning,
    Generic
}
public class AccountManager : MonoBehaviour {

    public delegate void AlertHandler(string msg, MsgType msgType = MsgType.Generic);
    

    [Header("Properties")]
    public string LoggedInSceneName = "Menu";
    public string LoggedOutSceneName = "Login";
    public static bool IsLoggedIn { get; protected set; }

    private const string _registerURL = "http://dev.sandordaemen.nl/kernmodule/php/register_handler.php";
    private const string _loginUrl = "http://dev.sandordaemen.nl/kernmodule/php/login_handler.php";
    private const string _updateURL = "http://dev.sandordaemen.nl/kernmodule/php/update_user_handler.php";

    //  public static Dictionary<string, string> TempUserData { get; protected set; }
    public static UserData User;
    public static AccountManager Instance;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    #region User Login

    public IEnumerator TryLogin(string userName, string password, AlertHandler alert) {
        WWWForm form = new WWWForm();

        form.AddField("username-login", userName);
        form.AddField("pass-login", password);
        form.AddField("hidden-field", "hidden");
        WWW w = new WWW(_loginUrl, form);

        yield return StartCoroutine(DoLogin(w, alert));
    }

    private IEnumerator DoLogin(WWW w, AlertHandler alert) {
        yield return w;
        if (w.error == null) {
            string[] receivedData = Regex.Split(w.text, "<>");
            string[] userData = Regex.Split(receivedData[1], ";");
            string errorData = receivedData[2];
            if (errorData == "") {
                FillUserData(userData);
                IsLoggedIn = true;
                alert("Successfully logged in!", MsgType.Success);
                StartCoroutine(DoAction(LoggedInSceneName));
            } else {
                alert(errorData, MsgType.Error);
            }
        } else {
            if (w.error == "Couldn't resolve host name")
            {
                alert("There appears to be no internet connection. Please connect to the internet.", MsgType.Warning);
            }
            else
            {
                alert(w.error, MsgType.Error);
            }
            
        }
    }

    private IEnumerator DoAction(string scene)
    {
        yield return new WaitForSeconds(2);
        SD.Menu.MenuManager.GoToScene(scene);
    }

    public void LogOut() {
        User.Clear();
        IsLoggedIn = false;

        Debug.Log("User logged out!");
        SD.Menu.MenuManager.GoToScene(LoggedOutSceneName);
    }
    #endregion

    private void FillUserData(string[] userData) {
        
        
        /*TempUserData = new Dictionary<string, string> {
            {"id", userData[0]},
            {"Username", userData[1]},
            {"Firstname", userData[2]},
            {"Lastname", userData[3]},
            {"Gender", userData[4]},
            {"Email", userData[5]},
            {"Birthdate", userData[6]},
            {"Registrationdate", userData[7]},
            {"SessionId", userData[8]}
        };*/


        User = new UserData(userData[0], userData[1], userData[2], userData[3], userData[5], userData[4], userData[6], userData[7], userData[8]);
    }



    #region Register User

    public IEnumerator TryRegister(string[] formData, AlertHandler alert) {
        WWWForm form = new WWWForm();

        form.AddField("register-username", formData[0]);
        form.AddField("register-first-name", formData[1]);
        form.AddField("register-last-name", formData[2]);
        form.AddField("register-email", formData[3]);
        form.AddField("register-gender-options", formData[4]);
        form.AddField("register-birthdate", formData[5]);
        form.AddField("register-pass", formData[6]);
        form.AddField("register-re-pass", formData[7]);
        form.AddField("hidden-field", "hidden");

        WWW w = new WWW(_registerURL, form);

        yield return StartCoroutine(DoRegister(w, alert));
    }

    private IEnumerator DoRegister(WWW w, AlertHandler alert) {
        yield return w;
        if (w.error == null) {
            string[] receivedData = Regex.Split(w.text, "<>");
            string msg = receivedData[1];
            string errorData = receivedData[2];

            if (string.IsNullOrEmpty(errorData)) {
                if (msg.Contains("SUCCESS")) {
                    SceneManager.LoadScene(LoggedOutSceneName);
                }
            }
            else {
                alert(errorData, MsgType.Error);
            }
        } else {
            alert(w.error, MsgType.Error);
        }
    }
    #endregion

   
    #region Update User Details
    public IEnumerator TryUpdate(string[] formData, AlertHandler alert) {

        if (!IsLoggedIn) {
            alert("You are not logged in!", MsgType.Error);
            yield break;
        }
 

        WWWForm form = new WWWForm();
        form.AddField("ID", User.ID);
        form.AddField("update-username", formData[0]);
        form.AddField("update-first-name", formData[1]);
        form.AddField("update-last-name", formData[2]);
        form.AddField("update-email", formData[3]);
        form.AddField("update-gender-options", formData[4]);
        form.AddField("update-birthdate", formData[5]);

        form.AddField("hidden-field", "hidden");

        WWW w = new WWW(_updateURL, form);

        yield return StartCoroutine(DoUpdate(w, alert));
    }

    private IEnumerator DoUpdate(WWW w, AlertHandler alert) {
        yield return w;

        if (w.error == null) {
            string[] receivedData = Regex.Split(w.text, "<>");
            string msg = receivedData[1];
            string errorData = receivedData[2];
            print(msg);
            if (string.IsNullOrEmpty(errorData)) {
                alert(msg);
                if (msg.Contains("SUCCESS")) {
                   // SceneManager.LoadScene(LoggedInSceneName);
                }
            } else {
                alert(errorData, MsgType.Error);
            }
        } else {
            alert(w.error, MsgType.Error);
        }
    }

    #endregion


    public struct UserData {

        public int ID { get; private set; }
        public string UserName { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string Gender { get; private set; }
        public string Birthdate { get; private set; }
        public string JoinDate { get; private set; }
        public string SessionID { get; private set; }

        public UserData(string iD, string userName, string firstName, string lastName, string email, string gender, string birthdate, string joinDate, string sessionID) : this() {
            ID = int.Parse(iD);
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Gender = gender;
            Birthdate = birthdate;
            JoinDate = joinDate;
            SessionID = sessionID;
        }


        /// <summary>
        /// Clear all data stored from the user
        /// </summary>
        /// <returns>True if succesful</returns>
        public bool Clear() {
            ID = 0;
            UserName = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            Gender = string.Empty;
            Birthdate = string.Empty;
            JoinDate = string.Empty;
            SessionID = string.Empty;
            return true;
        }
    }
}
