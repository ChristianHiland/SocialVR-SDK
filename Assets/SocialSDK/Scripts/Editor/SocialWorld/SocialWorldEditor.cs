using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;
using UnityEditor;
using SocialSDK.Interaction;

namespace SocialSDK {
    public class SocialWorldEditor : EditorWindow {

        [MenuItem("Social SDK/World Creation")]
        public static void ShowWindow() {
            GetWindow<SocialWorldEditor>("SocialVR World");
        }

        void OnGUI() {
            GUILayout.Label("World Creation Tools", EditorStyles.boldLabel);
            GUILayout.Space(10);
            if (GUILayout.Button("Add SocialVR Component...")) {
                ShowComponentMenu();
            }
        }

        void ShowComponentMenu() {
            // 1. Create the menu
            GenericMenu menu = new GenericMenu();

            // Format: "Category/ItemName" creates sub-menus automatically
            menu.AddItem(new GUIContent("Add SocialVR World"), false, AddSpawnPoint);
            menu.AddItem(new GUIContent("Interaction/Make Interactable"), false, AddInteractable);
            menu.AddItem(new GUIContent("Network/Sync Transform"), false, AddSyncComponent);

            // 3. Show the menu under the mouse cursor
            menu.ShowAsContext();
        }

        // --- Callback Methods ---

        void AddSpawnPoint() {
            if (Selection.activeGameObject != null) {
                // Undo.AddComponent allows the user to Ctrl+Z the action
                Undo.AddComponent<SocialWorld>(Selection.activeGameObject);
            } else {
                Debug.LogWarning("Please select an object in the Hierarchy first!");
            }
        }

        void AddInteractable() {
            if (Selection.activeGameObject != null) {
                SocialInteractable socialInteract = Undo.AddComponent<SocialInteractable>(Selection.activeGameObject);
                if (Selection.activeGameObject.GetComponent<Collider>() == null) {
                    MeshCollider meshCol = Undo.AddComponent<MeshCollider>(Selection.activeGameObject);
                    meshCol.convex = true;
                }
                // Setting up rigidbody.
                Rigidbody rb = Selection.activeGameObject.GetComponent<Rigidbody>();
                if (rb != null) {
                    rb.useGravity = false;
                    rb.isKinematic = true;
                }
            }
        }

        void AddSyncComponent() {
            if (Selection.activeGameObject != null) {
                // You can even add multiple components at once
                Undo.AddComponent<Photon.Pun.PhotonView>(Selection.activeGameObject);
                Undo.AddComponent<Photon.Pun.PhotonTransformView>(Selection.activeGameObject);
            }
        }
    }
}
