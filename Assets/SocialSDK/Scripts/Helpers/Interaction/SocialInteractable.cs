using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Photon.Pun;

namespace SocialSDK.Interaction {

    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(PhotonTransformView))]
    [RequireComponent(typeof(PhotonRigidbodyView))]
    [RequireComponent(typeof(XRGrabInteractable))]
    [RequireComponent(typeof(Rigidbody))]
    public class SocialInteractable : MonoBehaviour {

        [Header("Network")]
        [SerializeField] OwnershipOption ownershipOption;
        [SerializeField] ViewSynchronization viewSync;


        private XRGrabInteractable grabInteractable;
        private PhotonView pv;

        void Awake() {
            grabInteractable = GetComponent<XRGrabInteractable>();
            pv = GetComponent<PhotonView>();

            // Set the tag for grabbable object.
            gameObject.tag = "grabbable";

            // Configure Photon for user
            pv.OwnershipTransfer = ownershipOption;
            pv.Synchronization = viewSync;

            // Auto-subscribe to grab event.
            grabInteractable.selectEntered.AddListener(OnGrab);
        }

        // When a player (VR) grabs
        void OnGrab(SelectEnterEventArgs args) {
            // Handle networking.
            if (!pv.IsMine) pv.RequestOwnership();
        }
    }
}