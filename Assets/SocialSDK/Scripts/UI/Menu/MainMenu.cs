using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SocialSDK;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    public TMP_InputField name;
    public TMP_InputField username;
    public TMP_InputField password;
    public GameObject mainMenuScreen;
    public UserData UserLoginData;

    public Button loginButton;

    private API _api;
    private WorldHandler _worldHandler;

    void Awake() {
        if (_worldHandler == null) {
            _worldHandler = GameObject.Find("SocialSDK").GetComponent<WorldHandler>();
        }
    }

    void Start() {
        _api = GameObject.Find("SocialSDK").GetComponent<API>();
        _worldHandler = GameObject.Find("SocialSDK").GetComponent<WorldHandler>();
    }

    public void Login() {
        _api.Login(username.text, password.text);

    }

    public void LoginDataDone(UserData userData) {
        UserLoginData = userData;
        _worldHandler.LoadWorld("SocialSDK", "DefaultHome");
    }

    public void ResetButtons() {
        loginButton.interactable = false;
        loginButton.interactable = true;
    }
    
    public void Signup() { _api.Signup(name.text, username.text, password.text); }
}
