using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using SocialSDK;
using TMPro;

namespace SocialSDK.UI {
    public class WorldDownloader : MonoBehaviour {
        public TMPro.TMP_InputField worldName;
        public TMPro.TMP_InputField publisher;

        private SocialSDK.API _api;

        private void Start() {
            _api = GameObject.Find("SocialSDK").GetComponent<SocialSDK.API>();
        }

        public void DownloadWorld() {
            _api.DownloadWorld(worldName.text, publisher.text);
        }
    }
}