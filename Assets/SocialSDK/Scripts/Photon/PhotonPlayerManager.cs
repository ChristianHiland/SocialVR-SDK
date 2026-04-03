using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using SocialSDK;

public class PhotonPlayerManager : MonoBehaviourPun {
    [Header("Desktop Player")]
    public GameObject playerCamera;
    public FirstPersonMovement movement;
    public FirstPersonAudio personAudio;
    public GameObject socialSDK;
    public PlayerControl playerControl;


    void Start() {
        if (photonView.IsMine) {
            // Allow the Local Player to Control their own character
            playerCamera.SetActive(true);
            movement.enabled = true;
            personAudio.enabled = true;
            socialSDK.SetActive(true);
            playerControl.enabled = true;
        } else {
            playerCamera.SetActive(false);
            movement.enabled = false;
            personAudio.enabled = false;
            socialSDK.SetActive(false);
            playerControl.enabled = false;
        }
    }

    public void CheckAgain() {
        if (photonView.IsMine) {
            // Allow the Local Player to Control their own character
            playerCamera.SetActive(true);
            movement.enabled = true;
            personAudio.enabled = true;
        } else {
            playerCamera.SetActive(false);
            movement.enabled = false;
            personAudio.enabled = false;
            socialSDK.SetActive(false);
            playerControl.enabled = false;
        }
    }
}
