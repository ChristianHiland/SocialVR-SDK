using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SocialSDK {
    public class WorldTile : MonoBehaviour {
        public TMP_Text worldName;
        public TMP_Text worldPublisher;
        public RawImage worldImage;

        private WorldHandler _worldHandler;

        private void Start() {
            _worldHandler = GameObject.Find("SocialSDK").GetComponent<WorldHandler>();
        }
        
        public void SetWorld(string WorldName, string WorldPublisher, Texture2D WorldImage) {
            this.worldName.text = WorldName;
            this.worldPublisher.text = WorldPublisher;
            this.worldImage.texture = WorldImage;
        }

        public void OnClicked() {
            _worldHandler.LoadWorld(worldPublisher.text, worldName.text);
        }
    }
}