using System.Collections.Generic;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.Video;
using UnityEngine;
using TMPro;

namespace SocialSDK.Video {
    public class SocialVideoPlayer : MonoBehaviour {
        [Header("UI")]
        [SerializeField] VideoPlayer videoPlayer;
        [SerializeField] TMP_Text statusText;
        [SerializeField] TMP_InputField urlInputField;

        [Header("Data")]
        public string youtubeUrl = "";
        public string youtubeRedirUrl = "";

        [Header("API")]
        public Settings settings;

        void Awake() {
            statusText.gameObject.SetActive(false);

        }

        public void StartVideo() {
            statusText.gameObject.SetActive(true);
            if (youtubeUrl == "") {
                youtubeUrl = urlInputField.text;
            }

            StartCoroutine(GetDirectLink(youtubeUrl));
        }
    
        IEnumerator GetDirectLink(string ytUrl) {
            string url = settings.serverURL + "game/video/resolve?url=";
            // 1. Create the form data
            WWWForm form = new WWWForm();
            form.AddField("url", ytUrl);

            // 2. Create the Request
            using (UnityWebRequest www = UnityWebRequest.Get(url + UnityWebRequest.EscapeURL(ytUrl))) {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError || www.result != UnityWebRequest.Result.Success) { statusText.text = "Failed. Server API."; }

                if (www.result == UnityWebRequest.Result.Success) {
                    string jsonResponse = www.downloadHandler.text;
                    YouTubeResolverResponse data = JsonUtility.FromJson<YouTubeResolverResponse>(jsonResponse);
                    PlayDirect(data.url);
                }
            }
        }

        void PlayDirect(string url) {
            if (videoPlayer != null) {
                string finalUrl = url.Replace("https://", "http://");

                videoPlayer.source = VideoSource.Url;
                videoPlayer.url = finalUrl;

                // CRITICAL: Clear the previous state
                videoPlayer.Stop();

                videoPlayer.Prepare();
                videoPlayer.prepareCompleted += (vp) => {
                    Debug.Log("Video Prepared successfully!");
                    vp.Play();
                };

                videoPlayer.errorReceived += (vp, msg) => {
                    Debug.LogError("Unity Video Error: " + msg);
                };
            }
        }

        void OnVideoPlayerReady() {
            statusText.gameObject.SetActive(false);
            videoPlayer.Play();
        }
    
    }
}
