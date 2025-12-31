using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialSDK {
    public class WorldActionLoader : MonoBehaviour {
        private WorldHandler _worldHandler;
        private API _api;

        void Start() {
            _api = GameObject.Find("SocialSDK").GetComponent<API>();
            _worldHandler = GameObject.Find("SocialSDK").GetComponent<WorldHandler>();;
        }

        public void LaunchWorld(string worldName, string publisher) {
            _worldHandler.LoadWorld(worldName, publisher);
        }

    }
}