using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SocialSDK {
    public class PlayerControl : MonoBehaviour {
        [Header("Player Info")] 
        public UserData userData;

        [Header("UI Controls")] 
        public GameObject mainMenu;
        public GameObject menu;
        public GameObject debugMenu;
        
        public FirstPersonLook firstPersonLook;

        void Update() {
            if (mainMenu != null) {
                if (mainMenu.activeSelf || menu.activeSelf) {
                    Cursor.lockState = CursorLockMode.Confined;
                    firstPersonLook.enabled = false;
                }else {
                    Cursor.lockState = CursorLockMode.Locked;
                    firstPersonLook.enabled = true;
                }
            }else {
                if (menu.activeSelf || debugMenu.activeSelf) {
                    Cursor.lockState = CursorLockMode.Confined;
                    firstPersonLook.enabled = false;
                } else {
                    Cursor.lockState = CursorLockMode.Locked;
                    firstPersonLook.enabled = true;
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                menu.SetActive(!menu.activeSelf);
            }

            if (Input.GetKeyDown(KeyCode.G)) {
                debugMenu.SetActive(!debugMenu.activeSelf);
            }
            
        }
    }
}