using Unity.EditorCoroutines.Editor;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using System.Collections;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace SocialSDK {


    public class SocialSDK : EditorWindow {
        private Camera worldThumbnailCamera;
        private SceneAsset sceneToBundle;
        private TargetPlatform targetPlatform;
        private string serverURL = "http://170-187-201-130.ip.linodeusercontent.com:8001/";

        private string worldName = "";
        private string publisherName = "";
        private int cameraWidth = 1920;
        private int cameraHeight = 1080;
        private bool missingSocialWorld = false;

        [MenuItem("Social SDK/Control Panel")]
        public static void ShowWindow() {
            SocialSDK socialsdk = GetWindow<SocialSDK>();
            socialsdk.titleContent = new GUIContent("SocialVR SDK");
        }

        public void CreateGUI() {
            // Get the root element
            VisualElement root = rootVisualElement;

            // Load the UXML file and add it to root
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/SocialSDK/Scripts/Editor/SocialSDK Window.uxml");
            VisualElement labelFromUXML = visualTree.Instantiate();
            root.Add(labelFromUXML);

            // Get info from Social World Script in Scene
            SocialWorld socialWorld = GameObject.FindObjectOfType<SocialWorld>();
            if (socialWorld == null) {
                missingSocialWorld = true;
                Label checkProjectLabel = rootVisualElement.Q<Label>("VaildSocialWorldScript");
                Button buildUploadBtn = rootVisualElement.Q<Button>("BuildUploadWorld");
                checkProjectLabel.text = "You do not have a vaild Social World Script in this scene!";
                buildUploadBtn.SetEnabled(false);
            } else if (socialWorld != null) {
                // Set Scene
                string scenePath = SceneManager.GetActiveScene().path;
                sceneToBundle = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

                // Accessing Elements

                // World Info
                TextField worldNameInput = rootVisualElement.Q<TextField>("WorldName");
                TextField publisherNameInput = rootVisualElement.Q<TextField>("PublisherName");

                // Project Verify
                Label checkProjectLabel = rootVisualElement.Q<Label>("VaildSocialWorldScript");

                // Build Settings & Actions
                EnumField platformField = rootVisualElement.Q<EnumField>("PlatformField");
                targetPlatform = (TargetPlatform)platformField.value;

                Button buildUploadBtn = rootVisualElement.Q<Button>("BuildUploadWorld");

                worldNameInput.value = socialWorld.worldName;
                publisherNameInput.value = socialWorld.publisherUsername;
                worldName = socialWorld.worldName;
                publisherName = socialWorld.publisherUsername;

                // Actions
                buildUploadBtn.clicked += () => {
                    BuildAndUploadBundle(worldName, publisherName, (TargetPlatform)platformField.value);
                };
            }
        }

        private void BuildAndUploadBundle(string bundleName, string publisher, TargetPlatform platform) {
            if (sceneToBundle == null) return;

            // A. SETUP: Define where the bundle will be temporarily saved
            string bundleOutputPath = Path.Combine("Assets", "AssetBundles");
            if (!Directory.Exists(bundleOutputPath)) { Directory.CreateDirectory(bundleOutputPath); }

            // B. NAME THE SCENE FOR THE BUILD: Set the AssetBundle name for the scene file
            string scenePath = AssetDatabase.GetAssetPath(sceneToBundle);
            AssetImporter.GetAtPath(scenePath).assetBundleName = bundleName;

            // C. DEFINE BUILD MAPPING: Create the BuildAssetBundle array
            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
            buildMap[0].assetBundleName = bundleName;

            // The assetNames list must contain the full path to the scene asset
            buildMap[0].assetNames = new string[] { scenePath };

            BuildTarget buildTarget = BuildTarget.StandaloneWindows64;
            if (platform == TargetPlatform.Android) {
                buildTarget = BuildTarget.Android;
            } else if (platform == TargetPlatform.Windows) {
                buildTarget = BuildTarget.StandaloneWindows64;
            }

            // D. BUILD THE BUNDLE: Run the build pipeline
            BuildPipeline.BuildAssetBundles(
                bundleOutputPath,
                buildMap,
                BuildAssetBundleOptions.None,
                buildTarget // Build for the current platform
            );

            // E. CLEANUP: Clear the AssetBundle name after building to keep the project clean
            AssetImporter.GetAtPath(scenePath).assetBundleName = string.Empty;
            AssetDatabase.RemoveUnusedAssetBundleNames();

            // F. UPLOAD: Start the upload process
            string fullBundlePath = Path.Combine(bundleOutputPath, bundleName);

            if (File.Exists(fullBundlePath)) {
                Debug.Log($"Bundle '{bundleName}' created successfully. Starting upload...");
                // Start the upload Coroutine (using EditorCoroutines to run async code in the Editor)
                EditorCoroutineUtility.StartCoroutineOwnerless(UploadBundleFile(fullBundlePath, bundleName, publisher, platform));
            } else {
                Debug.LogError("Failed to find the built asset bundle file.");
            }
        }

        private IEnumerator UploadBundleFile(string filePath, string worldName, string publisher, TargetPlatform platform) {
            Debug.Log($"Target Platform: {platform}");
            WorldInfoGet payload = new WorldInfoGet { name = worldName, publisher = publisher, platform = platform.ToString() };

            string worldInfoJsonString = JsonUtility.ToJson(payload);
            // A. Create the form object
            WWWForm form = new WWWForm();

            // B. Add metadata fields
            form.AddField("worldInfo_str", worldInfoJsonString); // Key: 'name'

            // C. Read the file bytes and add them to the form
            byte[] fileBytes = File.ReadAllBytes(filePath);
            form.AddBinaryData("file", fileBytes, worldName, "application/octet-stream");

            string uploadUrl = serverURL + "game/assets/uploadWorld";

            // 2. Create the UnityWebRequest object (POST)
            using (UnityWebRequest uwr = UnityWebRequest.Post(uploadUrl, form)) {
                uwr.downloadHandler = new DownloadHandlerBuffer();

                yield return uwr.SendWebRequest();

                // E. Error checking unchanged
                if (uwr.result == UnityWebRequest.Result.Success) {
                    Debug.Log($"✅ Successfully uploaded '{worldName}'. Server Response: {uwr.downloadHandler.text}");
                } else {
                    Debug.LogError($"Upload failed! Error: {uwr.error}. Status: {uwr.responseCode}");
                }
            }
        }
    }
}