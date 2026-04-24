using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class SocialAvatar : MonoBehaviourPun {
    [Header("Transforms")]
    public Transform head_bone;

    [Header("Nametag")]
    public GameObject nametagUI;
    public TMP_Text displayNameLabel;
    public TMP_Text rankLabel;

    void Start() {
        if (photonView.IsMine) {
            head_bone.localScale = Vector3.zero;
        }

        // Setting nametag
        photonView.Owner.CustomProperties.TryGetValue("DisplayName", out object name);
        photonView.Owner.CustomProperties.TryGetValue("Rank", out object rank);
        displayNameLabel.text = (string)name;
        rankLabel.text = (string)rank;

        // Setting color
        if (rankLabel.text == "Admin" || rankLabel.text == "Dev") {
            rankLabel.color = new Color(153, 0, 255);
        }
    }

    void Update() {
        if (Camera.main != null) {
            nametagUI.transform.LookAt(nametagUI.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        }
    }
}
