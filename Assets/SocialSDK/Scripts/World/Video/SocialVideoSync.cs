using RenderHeads.Media.AVProVideo;
using Photon.Realtime;
using UnityEngine;
using Photon.Pun;

namespace SocialSDK.Video {
    public class SocialVideoSync : MonoBehaviourPunCallbacks {
        [SerializeField] MediaPlayer mediaPlayer;

        private string _currentUrl;
        private SocialVideoPlayer _socialVideoPlayer;


        void Awake() {
            _socialVideoPlayer = GetComponent<SocialVideoPlayer>();
        }

        void Update() {
            if (PhotonNetwork.IsMasterClient)
            {
                InvokeRepeating(nameof(MasterCheckSync), 3f, 3f);
            }
        }

        void MasterCheckSync() {
            if (PhotonNetwork.IsMasterClient && mediaPlayer.Control.IsPlaying()) {
                SendSyncHeartbeatMaster();
            }
        }

        void SendSyncHeartbeatMaster() {
            float currentTime = (float)mediaPlayer.Control.GetCurrentTime();
            bool isPlaying = mediaPlayer.Control.IsPlaying();

            // Send to clients.
            photonView.RPC("ReceiveSyncClient", RpcTarget.Others, currentTime, isPlaying);
        }

        [PunRPC]
        public void ReceiveSyncClient(float masterTime, bool masterIsPlaying) {
            float myTime = (float)mediaPlayer.Control.GetCurrentTime();

            if (Mathf.Abs(myTime - masterTime) > 1.5f) {
                mediaPlayer.Control.Seek(masterTime);
            }

            if (masterIsPlaying && !mediaPlayer.Control.IsPlaying()) {
                mediaPlayer.Control.Play();
            } else if (!masterIsPlaying && mediaPlayer.Control.IsPlaying()) {
                mediaPlayer.Control.Pause();
            }
        }

        public void RequestNewVideo(string url) {
            photonView.RPC("SyncVideo", RpcTarget.All, url);
        }

        [PunRPC]
        public void SyncVideo(string url) {
            _currentUrl = url;
            _socialVideoPlayer.ResolveAndPlay(url);
        }

    }
}