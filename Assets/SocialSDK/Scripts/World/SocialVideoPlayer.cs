using YoutubeExplode.Videos.Streams;
using RenderHeads.Media.AVProVideo;
using System.Threading.Tasks;
using System.Collections;
using YoutubeExplode;
using UnityEngine;
using TMPro;

namespace SocialSDK.Video {
    public class SocialVideoPlayer : MonoBehaviour {
        [Header("UI")]
        [SerializeField] MediaPlayer mediaPlayer;
        [SerializeField] TMP_Text statusText;
        [SerializeField] TMP_InputField urlInputField;

        [Header("Data")]
        public string youtubeUrl = "";
        public bool editorStart = false;

        [Header("API")]
        public Settings settings;
        private YoutubeClient _youtube = new YoutubeClient();

        void Awake() {
            if (statusText != null) statusText.gameObject.SetActive(false);
        }

        void Update() {
            if (editorStart) {
                editorStart = false;
                StartVideo();
            }
        }

        public void StartVideo() {
            if (statusText != null) statusText.gameObject.SetActive(true);
            if (youtubeUrl == "") { youtubeUrl = urlInputField.text; }
            ResolveAndPlay(youtubeUrl);
        }
    
        public async void ResolveAndPlay(string youtubeURL) {
            if (string.IsNullOrEmpty(youtubeURL)) return;

            try {
                statusText.text = "Getting Video Stream...";
                var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(youtubeURL);

                var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

                if (streamInfo != null) {
                    statusText.text = "Playing...";
                    mediaPlayer.OpenMedia(new MediaPath(streamInfo.Url, MediaPathType.AbsolutePathOrURL), autoPlay: true);
                    statusText.gameObject.SetActive(false);
                } else {
                    Debug.LogError("No compatible stream found for video");
                }
            } catch (System.Exception e) {
                Debug.LogError($"YoutubeExplode Error: {e}");
            }
        }
    }
}
