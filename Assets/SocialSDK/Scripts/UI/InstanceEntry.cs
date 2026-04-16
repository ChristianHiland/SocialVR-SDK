using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SocialSDK;

public class InstanceEntry : MonoBehaviour {
    public TMP_Text roomName;
    public string room_name;
    public Multiplayer multiplayerManager;

    public void Setup(string room_Name, Multiplayer multiplayer_Manager) {
        multiplayerManager = multiplayer_Manager;
        roomName.text = room_Name; 
    }

    public void JoinInstance() {
        Debug.Log("Joining instance");
        multiplayerManager.JoinInstance(roomName.text);
    }
}
