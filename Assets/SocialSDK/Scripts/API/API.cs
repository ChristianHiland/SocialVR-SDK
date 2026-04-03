using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace SocialSDK {
    
    public class API : MonoBehaviour {
        // API Only Vars
        
        // Ref to other components.
        [SerializeField] private WorldHandler worldHandler;
        [SerializeField] private SocialDll _socialDll;
        public Settings settings;

        [Header("Events")] 
        public LoginProcessed loginProcessed;
        public SignupProcessed signupProcessed;
        public DownloadProcessed downloadProcessed;
        public WorldListProcessed worldListProcessed;
        public WorldThumbnailProcessed worldThumbnailProcessed;
        
        [Header("Error Handling")] 
        public string errorReason;
        public bool erroredScene = false;
        
        void Start() {
            // Finding Components in Game.
            worldHandler = GameObject.Find("SocialSDK").GetComponent<WorldHandler>();
            string result = _socialDll.HandleRustString(SocialDll.checkForPresentLogin(Path.Combine(Application.persistentDataPath, "User/player_login.info")));
            if (result == "true")
            {
                string loginContent = LoadFile(Path.Combine(Application.persistentDataPath, "User/player_login.info"));
                UserData responseData = JsonUtility.FromJson<UserData>(loginContent);
                loginProcessed.Invoke(responseData);
            }
        }
        
        // Login Function
        public void Login(string username, string password) {
            string data = _socialDll.HandleRustString(SocialDll.login($"{settings.serverURL}user/login", username, password));
            UserData responseData = JsonUtility.FromJson<UserData>(data);
            Save<UserData>(responseData, Path.Combine(Application.persistentDataPath, "User/player_login.info"));
            loginProcessed.Invoke(responseData);
        }
        
        // Sign Up Functions.
        public void Signup(string name, string username, string password) { StartCoroutine(SignupCo(name, username, password)); }
        private IEnumerator SignupCo(string name, string username, string password) {
            UserSignup loginPayload = new UserSignup { name = name, username = username, password = password};
            byte[] rawBody = Encoding.UTF8.GetBytes(JsonUtility.ToJson(loginPayload));
            using (var uwr = new UnityWebRequest(settings.serverURL + "user/signup", UnityWebRequest.kHttpVerbGET)) {
                uwr.uploadHandler = new UploadHandlerRaw(rawBody);
                uwr.SetRequestHeader("Content-Type", "application/json");
                yield return uwr.SendWebRequest();
                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result != UnityWebRequest.Result.Success) { ErrorHappened(uwr.error, "Signup"); }
            }
            signupProcessed.Invoke();
        }
        
        // Download World Functions.
        public void DownloadWorld(string worldName, string publisher) {
            if (File.Exists(Path.Combine(Application.persistentDataPath, $"{settings.worldPath}{worldName}_{publisher}/world.socialWorld"))) {
                // Check file size and compare with server if it's not the same then download the changed world.
                string serverSize = _socialDll.HandleRustString(SocialDll.getServerWorldSize($"{settings.serverURL}game/assets/getWorldSize", publisher, worldName));
                FileInfo info = new FileInfo(Path.Combine(Application.persistentDataPath, $"{settings.worldPath}{worldName}_{publisher}/world.socialWorld"));
                long serverSideSize = long.Parse(serverSize);
                long sizeInBytes = info.Length;
                Debug.Log($"Server: {serverSideSize}, local: {sizeInBytes}");
                if (serverSideSize != sizeInBytes) {
                    StartCoroutine(DownloadWorldCo(worldName, publisher));
                }else {
                    downloadProcessed.Invoke(worldName, publisher, Path.Combine(Application.persistentDataPath, $"{settings.worldPath}"));
                }
            } else {
                StartCoroutine(DownloadWorldCo(worldName, publisher));
            }
            
        }
        
        private IEnumerator DownloadWorldCo(string worldName, string publisher) {
            // Setting up Loading UI.
            if (worldHandler.worldName != null) {
                worldHandler.worldName.text = worldName;
            }
            // Making Request URL.
            string encodedWorldName = UnityWebRequest.EscapeURL(worldName);
            string encodedPublisher = UnityWebRequest.EscapeURL(publisher);
            string fullUrl = $"{settings.serverURL}game/assets/getWorld?publisher={encodedPublisher}&worldName={encodedWorldName}";
            if (Directory.Exists(Path.Combine(Application.persistentDataPath, $"{settings.worldPath}{worldName}_{publisher}"))) {
                Directory.Delete(Path.Combine(Application.persistentDataPath, $"{settings.worldPath}{worldName}_{publisher}"), true);    
            }
            
            using (var uwr = new UnityWebRequest(fullUrl, UnityWebRequest.kHttpVerbGET)) {
                uwr.downloadHandler = new DownloadHandlerFile(Path.Combine(Application.persistentDataPath, $"{settings.worldPath}{worldName}_{publisher}.zip"));
                // Sending Request.
                UnityWebRequestAsyncOperation operation = uwr.SendWebRequest();
                while (!operation.isDone) {
                    // Processing & Getting Data
                    float progress = uwr.downloadProgress;
                    long currentBytes = (long)uwr.downloadedBytes;
                    string totalHeader = uwr.GetResponseHeader("Content-Length");
                    long.TryParse(totalHeader, out long totalBytes);
                    // Updating UI
                    worldHandler.loadingProgress.value = progress;
                    worldHandler.progressText.text = $"{(progress * 100):F0}%";
                    if (totalBytes > 0) {
                        string currentMB = (currentBytes / 1024f / 1024f).ToString("F1");
                        string totalMB = (totalBytes / 1024f / 1024f).ToString("F1");
                        worldHandler.dataDownloadedStatus.text = $"{currentMB}MB / {totalMB}MB";
                    }
                    yield return null;
                }
                // Checking for Errors.
                if (uwr.result == UnityWebRequest.Result.ConnectionError ||
                    uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result != UnityWebRequest.Result.Success) { ErrorHappened(uwr.error, "DownloadWorld"); }

                if (uwr.result == UnityWebRequest.Result.Success) {
                    worldHandler.loadingProgress.value = 1f;
                }
                // Checking for folder to make or keep..
                if (!Directory.Exists(Path.Combine(Application.persistentDataPath, $"{settings.worldPath}{worldName}_{publisher}"))) { Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, $"{settings.worldPath}{worldName}_{publisher}")); }
                Debug.Log("Downloaded World");
                // Try to Extract the zip.
                Debug.Log(uwr.result);
                ZipFile.ExtractToDirectory(Path.Combine(Application.persistentDataPath, $"{settings.worldPath}{worldName}_{publisher}.zip"), Path.Combine(Application.persistentDataPath, $"{settings.worldPath}{worldName}_{publisher}"));
                // Cleaning up by deleting zip file.
                File.Delete(Path.Combine(Application.persistentDataPath, $"{settings.worldPath}{worldName}_{publisher}.zip"));
            }
            downloadProcessed.Invoke(worldName, publisher, Path.Combine(Application.persistentDataPath, $"{settings.worldPath}"));
        }
        
        // World Listing Functions
        public void GetWorldsOnServer() { StartCoroutine(GetWorldList()); }
        private IEnumerator GetWorldList() {
            // Temp Var for storing Data
            GetWorldData worldData = new GetWorldData();
            using (var uwr = new UnityWebRequest(settings.serverURL + "game/assets/getWorldList", UnityWebRequest.kHttpVerbGET)) {
                uwr.SetRequestHeader("Content-Type", "application/json");
                uwr.downloadHandler = new DownloadHandlerBuffer();
                // Sending Request
                yield return uwr.SendWebRequest();
                // Error Checking.
                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result != UnityWebRequest.Result.Success) { ErrorHappened(uwr.error, "GetWorldList"); }
                worldData = JsonUtility.FromJson<GetWorldData>(uwr.downloadHandler.text);
            }
            worldListProcessed.Invoke(worldData);
        }

        public string GetWorldThumbnail(string worldName, string publisher) {
            StartCoroutine(GetWorldThumbnailCo(worldName, publisher));
            return Path.Combine(Application.persistentDataPath, $"{settings.worldThumbnailPath}{worldName}_{publisher}.png");
        }
        
        private IEnumerator GetWorldThumbnailCo(string worldName, string publisher) {
            // Temp Var for storing Data
            string encodedWorldName = UnityWebRequest.EscapeURL(worldName);
            string encodedPublisher = UnityWebRequest.EscapeURL(publisher);
            string fullUrl = $"{settings.serverURL}game/assets/getWorldThumbnail?publisher={encodedPublisher}&worldName={encodedWorldName}";
            using (var uwr = new UnityWebRequest(fullUrl, UnityWebRequest.kHttpVerbGET)) {
                uwr.downloadHandler = new DownloadHandlerFile(Path.Combine(Application.persistentDataPath, $"{settings.worldThumbnailPath}{worldName}_{publisher}.png"));
                // Sending Request
                UnityWebRequestAsyncOperation operation = uwr.SendWebRequest();
                while (!operation.isDone) {
                    // Processing & Getting Data
                    float progress = uwr.downloadProgress;
                    long currentBytes = (long)uwr.downloadedBytes;
                    string totalHeader = uwr.GetResponseHeader("Content-Length");
                    long.TryParse(totalHeader, out long totalBytes);
                    // Updating UI
                    worldHandler.loadingProgress.value = progress;
                    worldHandler.progressText.text = $"{(progress * 100):F0}%";
                    if (totalBytes > 0) {
                        string currentMB = (currentBytes / 1024f / 1024f).ToString("F1");
                        string totalMB = (totalBytes / 1024f / 1024f).ToString("F1");
                        worldHandler.dataDownloadedStatus.text = $"{currentMB}MB / {totalMB}MB";
                    }
                    yield return null;
                }
                // Error Checking.
                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result != UnityWebRequest.Result.Success) { ErrorHappened(uwr.error, "GetWorldThumbnail"); }
            }
            worldThumbnailProcessed.Invoke(Path.Combine(Application.persistentDataPath, $"{settings.worldThumbnailPath}{worldName}_{publisher}.png"));
        }
        
        
        // Helper Functions

        public void ErrorHappened(string reason, string function) {
            Debug.LogError($"{reason} caused by {function}");
            erroredScene = true;
            SceneManager.LoadScene(1);
            worldHandler.sceneLoadingUI.SetActive(false);
            TMP_Text reasonText = GameObject.Find("Reason").GetComponent<TMP_Text>();
            reasonText.text = reason;
            errorReason = reason;
        }
        
        void Update() {
            if (erroredScene) {
                GameObject player = GameObject.Find("Player");
                Transform stickTo = GameObject.Find("StickTo").GetComponent<Transform>();
                player.transform.position = stickTo.position;
            }
        }
        
        public void Save<T>(T dataToSave, string filePath) {
            string jsonString = JsonUtility.ToJson(dataToSave, true);
            try {
                File.WriteAllText(filePath, jsonString);
                Debug.Log($"Saved File: {filePath}");
            }catch (Exception e) {
                Debug.LogError($"[SAVING ERROR]: {e}");
            }
        }

        public string LoadFile(string filePath) {
            if (File.Exists(filePath)) {
                string fileContent = File.ReadAllText(filePath);
            }else {
                Debug.Log($"[LOADING ERROR]: File Not Found: {filePath}");
                return "";
            }

            return "";
        }
    }
}