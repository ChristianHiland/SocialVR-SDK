using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopInteracter : MonoBehaviour {
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float grabDistance = 4.0f;
    [SerializeField] private Transform holdPoint;
    private GameObject grabbedObject;

    void Update() {
        // Create the same ray used for grabbing
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        // Draw the ray in the Scene View for debugging
        // It will be red, and the length will match your grabDistance
        Debug.DrawRay(ray.origin, ray.direction * grabDistance, Color.red);

        if (Input.GetMouseButtonDown(0)) {
            if (grabbedObject == null) {
                TryGrab();
            } else {
                Drop();
            }
        }

        if (grabbedObject != null) {
            grabbedObject.transform.position = holdPoint.position;
        }
    }

    void TryGrab() {
        // Cast a ray from the center of the screen.
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, grabDistance)) {
            if (hit.collider.CompareTag("grabbable")) {
                grabbedObject = hit.collider.gameObject;

                Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;

                PhotonView pv = hit.collider.GetComponent<PhotonView>();
                if (pv != null && !pv.IsMine) {
                    pv.RequestOwnership();
                }
                // Parent to camera's hold point.
            }
        }
    }

    void Drop() {
        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        grabbedObject.transform.SetParent(null);
        grabbedObject = null;
    }
}
