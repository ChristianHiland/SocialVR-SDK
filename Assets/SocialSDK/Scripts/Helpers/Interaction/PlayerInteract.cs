using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour {
    [Header("Settings")] 
    public float interactRange = 3f;
    public LayerMask interactLayer;

    private Interactable currentInteractable;

    void Update() {
        CheckForInteractable();
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null) {
            if (currentInteractable.heldObj != null) {
                currentInteractable.DropObject();
                ClearCurrentInteractable();
            } else {
                currentInteractable.Interact();
            }
            
        }
    }

    void CheckForInteractable() {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, interactRange, interactLayer)) {
            Interactable newInteractable = hit.collider.GetComponent<Interactable>();
            if (newInteractable != currentInteractable) {
                ClearCurrentInteractable();
                currentInteractable = newInteractable;
                currentInteractable.OnLook();
            }
            return;
        }
        ClearCurrentInteractable();
    }

    void ClearCurrentInteractable() {
        if (currentInteractable != null) {
            currentInteractable.OnLookAway();
            currentInteractable = null;
        }
    }
}
