using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SocialSDK;

public class InstanceEntry : MonoBehaviour {
    [Header("UI")]
    public TMP_Text roomName;
    public TMP_Text roomOwner;
    public TMP_Text roomPlayerCount;
    
    [Header("Data")]
    public string room_name;
    string world_name;
    string world_publisher;
    string room_owner;
    string instanceID;

    [Header("Scripts")]
    public SocialSDK.Online.Multiplayer multiplayerManager;

    public void Setup(string room_Name, string owner, SocialSDK.Online.Multiplayer multiplayer_Manager) {
        multiplayerManager = multiplayer_Manager;
        room_name = room_Name;
        string[] parts = room_Name.Split('_');
        if (parts.Length >= 3) {
            // Saving Vars.
            world_publisher = parts[0];
            world_name = parts[1];
            instanceID = parts[2];
            room_owner = owner;
            // Setup UI
            roomName.text = $"#{instanceID}";
            roomOwner.text = $"{room_owner}";
        }
    }

    public void JoinInstance() {
        Debug.Log("Joining instance");
        multiplayerManager.JoinInstance(room_name, world_name, world_publisher, int.Parse(instanceID));
    }
}
