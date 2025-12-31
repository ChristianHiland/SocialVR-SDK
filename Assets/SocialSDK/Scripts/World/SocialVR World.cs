using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialSDK {
    public class SocialVRWorld : MonoBehaviour {
        [Header("World Info & Settings")] 
        public string worldName;
        public string publisherUsername;
        
        [Header("Player Settings")]
        [SerializeField] private float playerSpeed = 5f;
        [SerializeField] private bool playerCanRun = true;
        [SerializeField] private float playerRunSpeed = 9f;
        
        void Start() {
            FirstPersonMovement playerMovement = GameObject.Find("Player").GetComponent<FirstPersonMovement>();
            playerMovement.canRun = playerCanRun;
            playerMovement.runSpeed = playerRunSpeed;
            playerMovement.speed = playerSpeed;
        }
    }
}