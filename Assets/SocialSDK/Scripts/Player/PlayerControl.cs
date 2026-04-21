using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace SocialSDK {
    public class PlayerControl : MonoBehaviour {
        public PlatformType platform;

        [Header("Player Info")] 
        public UserData userData;

        [Header("UI Controls")] 
        public GameObject mainMenu;
        public GameObject menu;
        public GameObject debugMenu;

        [Header("Desktop")]
        public FirstPersonMovement firstPersonMovement;
        public FirstPersonLook firstPersonLook;

        [Header("VR")]
        public CharacterController cc;
        public GameObject locomotionSystem;

        void Update() {
            if (platform == PlatformType.Desktop) {
                if (mainMenu != null) {
                    if (mainMenu.activeSelf || menu.activeSelf) {
                        Cursor.lockState = CursorLockMode.Confined;
                        ToggleDesktopMovement(false);
                    } else {
                        Cursor.lockState = CursorLockMode.Locked;
                        ToggleDesktopMovement(true);
                    }
                } else {
                    if (menu.activeSelf || debugMenu.activeSelf) {
                        Cursor.lockState = CursorLockMode.Confined;
                        ToggleDesktopMovement(false);
                    } else {
                        Cursor.lockState = CursorLockMode.Locked;
                        ToggleDesktopMovement(true);
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

        public void ToggleMovement(bool state) {
            if (platform == PlatformType.Desktop) {
                ToggleDesktopMovement(state);
            } else if (platform == PlatformType.VR) {
                locomotionSystem.SetActive(state);
                if (state) {
                    cc.enabled = false;
                    cc.transform.position += Vector3.up * 0.1f;
                    cc.enabled = true;
                } else {
                    cc.enabled = false;
                }
            }
        }

        void ToggleDesktopMovement(bool state) {
            firstPersonLook.enabled = state;
            firstPersonMovement.enabled = state;
        }
    }
}