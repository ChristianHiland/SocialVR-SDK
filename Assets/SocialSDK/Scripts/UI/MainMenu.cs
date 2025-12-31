using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SocialSDK;
using TMPro;

public class MainMenu : MonoBehaviour {
    public TMP_InputField name;
    public TMP_InputField username;
    public TMP_InputField password;
    public UserData UserLoginData;
    
    private API _api;
    private WorldHandler _worldHandler;
    
    void Start() {
        _api = GameObject.Find("SocialSDK").GetComponent<API>();
        _worldHandler = GameObject.Find("SocialSDK").GetComponent<WorldHandler>();
    }

    public void Login() {
        _api.Login(username.text, password.text);
    }

    public void LoginDataDone(UserData userData) {
        UserLoginData = userData;
        _api.Save(UserLoginData, Path.Combine(Application.persistentDataPath, "User/player_login.info"));
        _worldHandler.LoadWorld("SocialSDK", "DefaultHome");
    }
    
    public void Signup() { _api.Signup(name.text, username.text, password.text); }
}
