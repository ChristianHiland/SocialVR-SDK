using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AttachToLocalPlayer : MonoBehaviour {
    public GameObject prefab;

    void Start() {
        AttachToLocal();
    }

    public void AttachToLocal() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) {
            if (GameObject.Find($"[LOCAL] {prefab.name}") == null) {
                GameObject spawnedManager = Instantiate(prefab, player.transform);
                spawnedManager.name = $"[LOCAL] {prefab.name}";
            }
            
        }
    }
}
