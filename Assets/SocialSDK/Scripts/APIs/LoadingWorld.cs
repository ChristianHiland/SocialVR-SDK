using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingWorld : MonoBehaviour {
    void Awake() {
        Transform playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        playerTrans.position = transform.position;
    }

}
