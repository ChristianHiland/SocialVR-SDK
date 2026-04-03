using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AvatarSetup : MonoBehaviourPun {
    public GameObject visualModel;
    
    void Start() {
        if (!photonView.IsMine) {
            // This is another player.
            return;
        }

        // This is our player

    }

    void Update() {
        // Making sure that this is our player.
        if (photonView.IsMine) {
            Transform localPlayer = GameObject.FindGameObjectWithTag("Player").transform;
            transform.position = localPlayer.position;
            transform.rotation = localPlayer.rotation;
        }
    }
}
