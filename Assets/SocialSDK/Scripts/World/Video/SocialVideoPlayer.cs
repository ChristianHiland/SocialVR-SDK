using YoutubeExplode.Videos.ClosedCaptions;
using RenderHeads.Media.AVProVideo.Demos;
using YoutubeExplode.Videos.Streams;
using RenderHeads.Media.AVProVideo;
using System.Threading.Tasks;
using System.Collections;
using Photon.Realtime;
using YoutubeExplode;
using UnityEngine;
using Photon.Pun;
using TMPro;

namespace SocialSDK.Video {
    public class SocialVideoPlayer : MonoBehaviourPunCallbacks {
        [Header("UI")]
        [SerializeField] MediaPlayer mediaPlayer;
        [SerializeField] MediaPlayerUI mediaPlayerUI;
        [SerializeField] TMP_Text statusText;
        [SerializeField] TMP_InputField urlInputField;

        [Header("Data")]
        public string youtubeUrl = "";
        public string _currentUrl = "";
        public bool playOnStart = false;

        [Header("Multiplayer Sync")]

        [Header("API")]
        private YoutubeClient _youtube = new YoutubeClient();

        void Awake() {
            if (statusText != null) statusText.gameObject.SetActive(false);
        }

        void Start() {
            if (playOnStart) {
                StartVideo();
            }
        }

        void Update() {
            if (PhotonNetwork.IsMasterClient && mediaPlayer.Control.IsPlaying()) {
                InvokeRepeating(nameof(SendSyncHeartbeatMaster), 6f, 6f);
            }
        }

        public void StartVideo()
        {
            if (statusText != null) statusText.gameObject.SetActive(true);
            if (youtubeUrl == "") { youtubeUrl = urlInputField.text; }
            if (PhotonNetwork.IsMasterClient) {
                RequestNewVideo(youtubeUrl);
            }
        }
    
        public async void ResolveAndPlay(string youtubeURL) {
            if (string.IsNullOrEmpty(youtubeURL)) return;

            try {
                statusText.text = "Getting Video Stream...";
                var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(youtubeURL);
                var trackManifest = await _youtube.Videos.ClosedCaptions.GetManifestAsync(youtubeURL);

                var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();
                var trackInfo = trackManifest.TryGetByLanguage("en") ?? trackManifest.GetByLanguage("en");

                if (streamInfo != null) {
                    statusText.text = "Playing...";
                    mediaPlayer.OpenMedia(new MediaPath(streamInfo.Url, MediaPathType.AbsolutePathOrURL), autoPlay: true);
                    statusText.gameObject.SetActive(false);
                    mediaPlayerUI.enabled = true;
                    // Setup to allow the next play of another video.
                    youtubeUrl = "";
                    urlInputField.text = "";
                } else {
                    Debug.LogError("No compatible stream found for video");
                }

                if (trackInfo != null) {
                    var captions = await _youtube.Videos.ClosedCaptions.GetAsync(trackInfo);
                    var captionDisplay = GetComponent<ScratchCaptionDisplay>();

                    if (captionDisplay != null) {
                        captionDisplay.captions.Clear();

                        foreach (var caption in captions.Captions) {
                            captionDisplay.captions.Add(new SubtitleEntry(
                                caption.Offset.TotalSeconds,
                                (caption.Offset + caption.Duration).TotalSeconds,
                                caption.Text
                            ));
                        }
                    }
                }
            } catch (System.Exception e) {
                Debug.LogError($"YoutubeExplode Error: {e}");
            }
        }


        //------------------------
        // Photon Network Syncing
        //------------------------



        void SendSyncHeartbeatMaster() {
            float currentTime = (float)mediaPlayer.Control.GetCurrentTime();
            bool isPlaying = mediaPlayer.Control.IsPlaying();

            // Send to clients.
            photonView.RPC("ReceiveSyncClient", RpcTarget.Others, currentTime, isPlaying);
        }

        [PunRPC]
        public void ReceiveSyncClient(float masterTime, bool masterIsPlaying) {
            float myTime = (float)mediaPlayer.Control.GetCurrentTime();

            if (Mathf.Abs(myTime - masterTime) > 1f) {
                mediaPlayer.Control.Seek(masterTime);
            }

            if (masterIsPlaying && !mediaPlayer.Control.IsPlaying()) {
                mediaPlayer.Control.Play();
            } else if (!masterIsPlaying && mediaPlayer.Control.IsPlaying()) {
                mediaPlayer.Control.Pause();
            }
        }


        public void RequestNewVideo(string url) { photonView.RPC("SyncVideo", RpcTarget.All, url); }

        [PunRPC]
        public void SyncVideo(string url) {
            _currentUrl = url;
            ResolveAndPlay(url);
        }
    }
}
