using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialSDK {
    public class AttachCanvasCamera : MonoBehaviour {
        public List<Canvas> canvases;

        void Awake() {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) {
                PlayerControl playerControl = player.GetComponent<PlayerControl>();
                foreach (Canvas canvas in canvases) {
                    canvas.worldCamera = playerControl.vrCamera; 
                }
            }
        }
    }
}