using System;
using UnityEngine.Networking;
using System.IO.Compression;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Text;

namespace SocialSDK {
    public class API_OLD : MonoBehaviour {
        
        public string ServerURL = "http://127.0.0.1:8002/";
        public TMP_InputField serverIP;

        // Locations for File Data
        public string WorldPath;
        public string UserPath;

        public UserData UserLoginData;
        public string WorldListData;
        public string WorldInfo;

        public MainMenu mainMenu;
        public Menu menu;
        public WorldHandler worldHandler;
        public bool WaitingOver = false;

        public Settings settings;
        
        // Downloading a World and Unzipping it into the WorldPath with the extract folder named: worldName_publisher/

        public void Start() {
            // Setting Paths.
            WorldPath = Application.persistentDataPath + "/Worlds/";
            UserPath = Application.persistentDataPath + "/User/";
            // Finding Components
            mainMenu = GameObject.Find("Main Menu").GetComponent<MainMenu>();
            menu = GameObject.Find("Menu").GetComponent<Menu>();
            worldHandler = GameObject.Find("SocialSDK").GetComponent<WorldHandler>();
            // Making sure the API doesn't get lost or destroyed.
            DontDestroyOnLoad(gameObject);
            // Checking for previous login, to not ask for a login, and load the default world already.
            bool loginStatus = CheckLogin();
            if (loginStatus) { worldHandler.LoadWorld(settings.defaultWorld.publisher, settings.defaultWorld.name); }
        }

        private bool CheckLogin() {
            string userDataPath = Path.Combine(Application.persistentDataPath, settings.userLoginFile);
            if (File.Exists(userDataPath)) {
                string userLoginContents = LoadFile(userDataPath);
                settings.userData = JsonUtility.FromJson<UserData>(userLoginContents);
                return true;
            }
            return false;
        }
        
        public void DownloadWorld(string worldName, string publisher) {
            StartCoroutine(DownloadWorldCo(worldName, publisher));
        }

        private IEnumerator DownloadWorldCo(string worldName, string publisher) {
            string encodedWorldName = UnityWebRequest.EscapeURL(worldName);
            string encodedPublisher = UnityWebRequest.EscapeURL(publisher);
            string fullUrl = $"{ServerURL}game/assets/getWorld?publisher={encodedPublisher}&worldName={encodedWorldName}";
            using (var uwr = new UnityWebRequest(fullUrl, UnityWebRequest.kHttpVerbGET)) {
                uwr.downloadHandler = new DownloadHandlerFile(WorldPath + $"{worldName}_{publisher}.zip");
                yield return uwr.SendWebRequest();
                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError) { Debug.LogError($"Download Error: {uwr.error}"); }
                if (!Directory.Exists(WorldPath + $"{worldName}_{publisher}")) { Directory.CreateDirectory(WorldPath + $"{worldName}_{publisher}"); }
                ZipFile.ExtractToDirectory(WorldPath + $"{worldName}_{publisher}.zip", WorldPath + $"{worldName}_{publisher}", overwriteFiles: true);
                File.Delete(WorldPath + $"{worldName}_{publisher}.zip");
            }
        }

        // Getting a list of worlds
        public GetWorldData GetWorldsOnServer() {
            StartCoroutine(GetWorldsOnServerCo());
            if (!WaitingOver) {
                Debug.Log("Not ready.");
            }else {
                WaitingOver = false;
                return JsonUtility.FromJson<GetWorldData>(WorldListData);
            }
            return new GetWorldData();
        }

        private IEnumerator GetWorldsOnServerCo() {
            using (var uwr = new UnityWebRequest(ServerURL + "game/assets/getWorldList", UnityWebRequest.kHttpVerbGET)) {
                uwr.SetRequestHeader("Content-Type", "application/json");
                uwr.downloadHandler = new DownloadHandlerBuffer();
                yield return uwr.SendWebRequest();
                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError) { Debug.LogError($"Download Error: {uwr.error}"); }
                WorldListData = uwr.downloadHandler.text;
            }

            menu.PopulateWorlds2(JsonUtility.FromJson<GetWorldData>(WorldListData));
        }

        // Downloading User Data, and unziping it into the UserPath, with extract folder named: username/

        public void DownloadUser(string username) {
            StartCoroutine(DownloadUserCo(username));
        }

        private IEnumerator DownloadUserCo(string username) {
            using (var uwr = new UnityWebRequest(ServerURL + "user/assets/get", UnityWebRequest.kHttpVerbGET)) {
                uwr.downloadHandler = new DownloadHandlerFile(UserPath + $"{username}.zip");
                yield return uwr.SendWebRequest();
                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError) { Debug.LogError($"Download Error: {uwr.error}"); }
                if (!Directory.Exists(UserPath + $"{username}")) { Directory.CreateDirectory(UserPath + $"{username}"); }
                ZipFile.ExtractToDirectory(UserPath + $"{username}.zip", UserPath + $"{username}", overwriteFiles: true);
            }
        }

        // Login in function

        public UserData Login(string username, string password) {
            StartCoroutine(LoginCo(username, password));
            return UserLoginData;
        }

        private IEnumerator LoginCo(string username, string password) {
            UserLogin loginPayload = new UserLogin { username = username, password = password};
            string jsonPayload = JsonUtility.ToJson(loginPayload);
            Debug.Log(jsonPayload);
            byte[] rawBody = Encoding.UTF8.GetBytes(jsonPayload);
            using (var uwr = new UnityWebRequest(ServerURL + "user/login", UnityWebRequest.kHttpVerbPOST)) {
                uwr.uploadHandler = new UploadHandlerRaw(rawBody);
                uwr.SetRequestHeader("Content-Type", "application/json");
                uwr.downloadHandler = new DownloadHandlerBuffer();
                uwr.SetRequestHeader("Accept", "application/json");
                yield return uwr.SendWebRequest();
                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError) { Debug.LogError($"Download Error: {uwr.error}"); }
                string responseText = uwr.downloadHandler.text;
                UserLoginData = JsonUtility.FromJson<UserData>(responseText);
            }
        }
        
        public void Signup(string name, string username, string password) {
            StartCoroutine(SignupCo(name, username, password));
        }

        private IEnumerator SignupCo(string name, string username, string password) {
            UserSignup loginPayload = new UserSignup { name = name, username = username, password = password};
            string jsonPayload = JsonUtility.ToJson(loginPayload);
            byte[] rawBody = Encoding.UTF8.GetBytes(jsonPayload);
            using (var uwr = new UnityWebRequest(ServerURL + "user/signup", UnityWebRequest.kHttpVerbGET)) {
                uwr.uploadHandler = new UploadHandlerRaw(rawBody);
                uwr.SetRequestHeader("Content-Type", "application/json");
                yield return uwr.SendWebRequest();
                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError) { Debug.LogError($"Signup Error: {uwr.error}"); }
            }
        }

        public void Save<T>(T dataToSave, string filePath) {
            string jsonString = JsonUtility.ToJson(dataToSave, true);
            try {
                File.WriteAllText(filePath, jsonString);
                Debug.Log("Saved");
            }catch (Exception e) {
                Debug.LogError($"[ERROR]: {e}");
            }
        }

        public string LoadFile(string filepath) {
            if (File.Exists(filepath)) {
                string fileContent = File.ReadAllText(filepath);
                return  fileContent;
            }else {
                Debug.Log("File not found");
                return "";
            }
        }

        public void UpdateIP() {
            ServerURL = serverIP.text;
        }
    }
}