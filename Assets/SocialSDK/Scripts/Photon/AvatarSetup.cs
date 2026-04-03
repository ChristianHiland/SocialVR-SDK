using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AvatarSetup : MonoBehaviourPun {
    private Transform _localPlayer;
    public GameObject visualModel;
    
    void Start() {
        if (!photonView.IsMine) {
            // This is another player.
            Transform localPlayer = GameObject.FindGameObjectWithTag("Player").transform;
            return;
        } else if (photonView.IsMine) {
            GameObject lp = GameObject.FindGameObjectWithTag("Player");
            if (lp != null) _localPlayer = lp.transform;
            visualModel.SetActive(true);
        }

        // This is our player

    }

    void Update() {
        // Making sure that this is our player.
        if (photonView.IsMine) {
            transform.position = _localPlayer.position;
            transform.rotation = _localPlayer.rotation;
        }
    }
}
