using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialSDK {
    public class ErrorWorld : MonoBehaviour {
        private GameObject _WorldSelector;
        
        void Start() {
            _WorldSelector = GameObject.Find("Menu");
            if (_WorldSelector != null) {
                _WorldSelector.SetActive(false);
            }
        }
    }
}