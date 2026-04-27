using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class NametagSync : MonoBehaviourPun {
    public GameObject nametagUI;
    public TMP_Text displayNameLabel;
    public TMP_Text rankLabel;

    void Start() {
        photonView.Owner.CustomProperties.TryGetValue("DisplayName", out object name);
        photonView.Owner.CustomProperties.TryGetValue("Rank", out object rank);
        displayNameLabel.text = (string)name;
        rankLabel.text = (string)rank;

        if ((string)rank == "Admin" || (string)rank == "Dev") {
            rankLabel.color = new Color(153, 0, 255);
        }
    }

    // Update is called once per frame
    void Update() {
        if (Camera.main != null) {
            nametagUI.transform.LookAt(nametagUI.transform.position + Camera.main.transform.rotation * Vector3.forward,
                Camera.main.transform.rotation * Vector3.up);
        }
    }
}
