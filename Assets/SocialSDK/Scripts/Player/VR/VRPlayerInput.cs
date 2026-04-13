using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class VRPlayerInput : MonoBehaviour {
    public GameObject menuUI;


    public InputActionReference menuButton;

    void Start() {
        menuButton.action.started += ToggleMenu;
    }

    void Update() {
    }

    private void ToggleMenu(InputAction.CallbackContext context) {
        menuUI.SetActive(!menuUI.activeSelf);
    }

}
