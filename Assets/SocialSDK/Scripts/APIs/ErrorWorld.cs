using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SocialSDK {
    public class ErrorWorld : MonoBehaviour {
        public TMP_Text reason;
        private GameObject _WorldSelector;

        void Awake() {
            API _api = Object.FindAnyObjectByType<API>();
            reason.text = _api.errorReason;
        }

        void Start() {
            _WorldSelector = GameObject.Find("Menu");
            if (_WorldSelector != null) {
                _WorldSelector.SetActive(false);
            }
        }
    }
}