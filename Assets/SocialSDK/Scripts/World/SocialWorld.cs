using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialSDK {
    public class SocialWorld : MonoBehaviour {
        [Header("World Info & Settings")] 
        public string worldName;
        public string publisherUsername;
        
        [Header("Player Settings")]
        [SerializeField] private float playerSpeed = 5f;
        [SerializeField] private bool playerCanRun = true;
        [SerializeField] private float playerRunSpeed = 9f;

        [Header("Editor ONLY")]
        public GameObject localPlayerPrefab;
        public bool SpawnLocalPlayer = false;
        
        void Start() {
            if (SpawnLocalPlayer) {
                GameObject localPlayer = Instantiate(localPlayerPrefab);
                localPlayer.transform.position = transform.position;
            }
        }
    }
}