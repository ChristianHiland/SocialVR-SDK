using Unity.EditorCoroutines.Editor;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.IO;
using SocialSDK;

namespace SocialSDK {
    public class SceneUploaderEditor : EditorWindow {
        private SceneAsset sceneToBundle;
        private string worldName = "world";
        private string serverUploadUrl = "http://127.0.0.1:8002/game/assets/uploadWorld";
        private string publisherMetadata = "SocialSDK";
        private Camera worldThumbnailCamera;
        private int width = 1920;
        private int height = 1080;

        // --- Editor Window Setup ---
        [MenuItem("Social SDK/Bundle Scene")]
        public static void ShowWindow() {
            GetWindow<SceneUploaderEditor>("Scene Uploader");
        }

        private void OnGUI() {
            GUILayout.Label("Social World Uploader", EditorStyles.boldLabel);

            // 1. Scene Selection Field
            // Allows dragging a scene file into the window
            sceneToBundle = (SceneAsset)EditorGUILayout.ObjectField(
                "Scene to Upload",
                sceneToBundle,
                typeof(SceneAsset),
                false
            );

            // 2. World Name Field
            worldName = EditorGUILayout.TextField("World Name", worldName);
            // Publisher Name
            publisherMetadata = EditorGUILayout.TextField("Publisher (Metadata)", publisherMetadata);

            // 3. Camera for World Thumbnail capture Field
            worldThumbnailCamera = (Camera)EditorGUILayout.ObjectField("World Thumbnail Camera", worldThumbnailCamera, typeof(Camera), true);
            

            GUILayout.Space(15);

            // 4. Action Button
            GUI.enabled = sceneToBundle != null; // Disable button if no scene is selected
            if (GUILayout.Button("Build & Upload World")) {
                BuildAndUploadBundle(sceneToBundle, worldName, serverUploadUrl, publisherMetadata);
            }
            
            if (GUILayout.Button("Upload World Thumbnail")) {
                MakeWorldThumbnail();
            }

            GUI.enabled = true;
        }

        private void BuildAndUploadBundle(SceneAsset sceneToBundle, string bundleName, string serverURL, string publisher) {
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

            // D. BUILD THE BUNDLE: Run the build pipeline
            BuildPipeline.BuildAssetBundles(
                bundleOutputPath,
                buildMap,
                BuildAssetBundleOptions.None,
                EditorUserBuildSettings.activeBuildTarget // Build for the current platform
            );

            // E. CLEANUP: Clear the AssetBundle name after building to keep the project clean
            AssetImporter.GetAtPath(scenePath).assetBundleName = string.Empty;
            AssetDatabase.RemoveUnusedAssetBundleNames();

            // E.1 Make PNG from World Thumbnail camera.
            MakeWorldThumbnail();
            
            // F. UPLOAD: Start the upload process
            string fullBundlePath = Path.Combine(bundleOutputPath, bundleName);

            if (File.Exists(fullBundlePath)) {
                Debug.Log($"Bundle '{bundleName}' created successfully. Starting upload...");
                // Start the upload Coroutine (using EditorCoroutines to run async code in the Editor)
                EditorCoroutineUtility.StartCoroutineOwnerless(UploadBundleFile(fullBundlePath, bundleName, serverURL,
                    publisher));
            }else {
                Debug.LogError("Failed to find the built asset bundle file.");
            }
        }

        private static IEnumerator UploadBundleFile(string filePath, string worldName, string uploadUrl, string publisher) {

            WorldInfoGet payload = new WorldInfoGet { name = worldName, publisher = publisher };

            string worldInfoJsonString = JsonUtility.ToJson(payload);
            // A. Create the form object
            WWWForm form = new WWWForm();

            // B. Add metadata fields
            form.AddField("worldInfo_str", worldInfoJsonString); // Key: 'name'

            // C. Read the file bytes and add them to the form
            byte[] fileBytes = File.ReadAllBytes(filePath);
            // Key: 'file' (This key MUST match the FastAPI File(...) parameter name!)
            form.AddBinaryData("file", fileBytes, worldName, "application/octet-stream");

            // 2. Create the UnityWebRequest object (POST)
            using (UnityWebRequest uwr = UnityWebRequest.Post(uploadUrl, form)) {
                // UnityWebRequest.Post(url, form) automatically sets the method to POST 
                // and Content-Type to multipart/form-data, so we don't set headers manually.

                uwr.downloadHandler = new DownloadHandlerBuffer();

                yield return uwr.SendWebRequest();

                // E. Error checking unchanged
                if (uwr.result == UnityWebRequest.Result.Success) {
                    Debug.Log($"✅ Successfully uploaded '{worldName}'. Server Response: {uwr.downloadHandler.text}");
                }else {
                    Debug.LogError($"Upload failed! Error: {uwr.error}. Status: {uwr.responseCode}");
                }
            }
        }

        private static IEnumerator UploadWorldThumbnail(string worldName, string publisher, string uploadUrl) {
            string savePath = Path.Combine("Assets", $"{worldName}_{publisher}.png");
            WWWForm form = new WWWForm();
            
            form.AddField("worldName", worldName);
            form.AddField("publisher", publisher);
            
            byte[] fileBytes = File.ReadAllBytes(savePath);
            form.AddBinaryData("file", fileBytes, worldName, "application/octet-stream");
            
            // 2. Create the UnityWebRequest object (POST)
            using (UnityWebRequest uwr = UnityWebRequest.Post(uploadUrl, form)) {
                // UnityWebRequest.Post(url, form) automatically sets the method to POST 
                // and Content-Type to multipart/form-data, so we don't set headers manually.

                uwr.downloadHandler = new DownloadHandlerBuffer();

                yield return uwr.SendWebRequest();

                // E. Error checking unchanged
                if (uwr.result == UnityWebRequest.Result.Success) {
                    Debug.Log($"✅ Successfully uploaded '{worldName}'. Server Response: {uwr.downloadHandler.text}");
                }else {
                    Debug.LogError($"Upload failed! Error: {uwr.error}. Status: {uwr.responseCode}");
                }
            }
        }

        void WorldDoneUploading() {
            EditorCoroutineUtility.StartCoroutineOwnerless(UploadWorldThumbnail(worldName, publisherMetadata, "http://127.0.0.1:8002/game/assets/uploadWorldThumbnail"));
        }

        void MakeWorldThumbnail() {
            RenderTexture thumbnailTexture = new  RenderTexture(width, height, 24);
            worldThumbnailCamera.targetTexture = thumbnailTexture;

            Texture2D thumbnail = new Texture2D(width, height, TextureFormat.RGB24, false);
            worldThumbnailCamera.Render();
            
            RenderTexture.active = thumbnailTexture;
            thumbnail.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            
            worldThumbnailCamera.targetTexture = null;
            RenderTexture.active = null;
            DestroyImmediate(thumbnailTexture);

            byte[] bytes = thumbnail.EncodeToPNG();
            string savePath = Path.Combine("Assets", $"{worldName}_{publisherMetadata}.png");
            
            File.WriteAllBytes(savePath, bytes);
            AssetDatabase.Refresh();
            WorldDoneUploading();
        }

    }
}