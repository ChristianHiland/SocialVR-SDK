using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;
using SocialSDK;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SocialSDK {
    public class WorldHandler : MonoBehaviour {
        public float progress;

        [Header("Menu Objects")] 
        public GameObject mainMenu;
        public GameObject menu;
        public GameObject sceneLoadingUI;
        public GameObject ProgressGroupUI;
        public GameObject NowLoadingSceneUI;
        public TMP_Text progressText;
        public TMP_Text dataDownloadedStatus;
        public TMP_Text worldName;
        public RawImage worldThumbnail;
        public Slider loadingProgress;
        public AudioSource loadingMusic;
        
        private API _api;
        private AssetBundle _assetBundle;
        private WorldData _worldData;

        private void Start() {
            _api = gameObject.GetComponent<API>();
        }

        public void LoadWorld(string publisher, string worldName) {
            menu.SetActive(false);
            sceneLoadingUI.SetActive(true);
            StartCoroutine(FadeInAudio(loadingMusic, 2.5f, .6f));
            progressText.text = "Fetching...";
            // Download World & Thumbnail.
            string imagePath = _api.GetWorldThumbnail(worldName, publisher);
            _api.DownloadWorld(publisher, worldName);
        }

        public void FinishLoading(string publisher, string worldName, string WorldPath) {
            // Load World.
            string worldPath = WorldPath;
            string bundlePath = Path.Combine(worldPath, $"{publisher}_{worldName}/");
            
            // Loading World Info.
            string worldInfoAsset = File.ReadAllText(bundlePath + $"world.info");
            _worldData = JsonUtility.FromJson<WorldData>(worldInfoAsset);
            // Start Coroutine for loading world.
            StartCoroutine(LoadSceneFromBundle(bundlePath, _worldData.scene));
        }

        private IEnumerator LoadSceneFromBundle(string bundlePath, string scene) {
            ProgressGroupUI.SetActive(false);
            NowLoadingSceneUI.SetActive(true);
            Debug.Log("Loading Scene...");
            
            // Getting Asset Bundle
            AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(bundlePath + $"world.socialWorld");
            yield return bundleRequest;
            _assetBundle = bundleRequest.assetBundle;
            
            foreach (string name in _assetBundle.GetAllAssetNames())
            {
                Debug.Log(name);
            }
            
            // Loading Scene
            string[] scenePaths = bundleRequest.assetBundle.GetAllScenePaths();
            AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(scenePaths[0], LoadSceneMode.Single);
            if (sceneLoad == null) { yield break; }
            sceneLoad.allowSceneActivation = false;

            while (!sceneLoad.isDone) {
                progress = Mathf.Clamp01(sceneLoad.progress / 0.9f);
                if (sceneLoad.progress >= 0.9f) { sceneLoad.allowSceneActivation = true; }
                progressText.text = progress + "%";
                loadingProgress.value = progress;
                yield return null;
            }
            _assetBundle.Unload(false);
            sceneLoadingUI.SetActive(false);
            DoneLoadingWorld();
        }
        
        private void DoneLoadingWorld() {
            ProgressGroupUI.SetActive(true);
            NowLoadingSceneUI.SetActive(false);
            loadingMusic.Stop();
            loadingMusic.time = 0f;
            Transform spawnHere = GameObject.Find("Spawn Here").transform;
            Transform player = GameObject.Find("Player").transform;
            if (spawnHere != null) {
                player.position = spawnHere.position;
            }

        }

        public void LoadThumbnail(string path) {
            if (File.Exists(path)) {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    byte[] imageData = new byte[fs.Length];
                    fs.Read(imageData, 0, imageData.Length);
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(imageData);
                    worldThumbnail.texture = texture;
                }
            }
        }
        
        public static IEnumerator FadeInAudio(AudioSource audioSource, float duration, float targetVolume) {
            float startVolume = 0f; // Start from 0 volume
            audioSource.volume = startVolume;
            audioSource.Play(); // Start playing the audio

            float timeElapsed = 0f;
            while (timeElapsed < duration) {
                // Gradually increase the volume using Mathf.Lerp
                audioSource.volume = Mathf.Lerp(startVolume, targetVolume, timeElapsed / duration);
                timeElapsed += Time.deltaTime;
                yield return null; // Wait for the next frame
            }

            // Ensure the volume reaches the exact target volume at the end
            audioSource.volume = targetVolume;
        }

        public static IEnumerator FadeInUI() {
            yield return new WaitForEndOfFrame();
        }
    }
}