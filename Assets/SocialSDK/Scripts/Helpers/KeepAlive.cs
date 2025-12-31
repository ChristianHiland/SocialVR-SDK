using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepAlive : MonoBehaviour {
    void Start() {
        DontDestroyOnLoad(gameObject);
    }
}
