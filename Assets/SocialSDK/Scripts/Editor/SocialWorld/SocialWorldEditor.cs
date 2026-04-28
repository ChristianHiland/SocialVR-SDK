using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;
using SocialSDK.Interaction;
using System.Collections;
using UnityEngine.Audio;
using SocialSDK.Audio;
using UnityEngine;
using UnityEditor;


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
                GameObject audioMixer = new GameObject("World Audio Mixer");

                audioMixer.transform.SetParent(Selection.activeGameObject.transform);
                audioMixer.transform.localPosition = Vector3.zero;
                audioMixer.transform.localRotation = Quaternion.identity;

                AudioMixer template = AssetDatabase.LoadAssetAtPath<AudioMixer>("Assets/SocialSDK/Internal/SocialMixerTemplate.mixer");
                
                if (template != null) {
                    var manager = Undo.AddComponent<SocialWorldVolume>(audioMixer);
                    manager.mainMixer = template;
                }
            
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
