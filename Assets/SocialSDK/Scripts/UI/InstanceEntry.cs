using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using Photon.Pun;
using TMPro;
using SocialSDK;

public class InstanceEntry : MonoBehaviour {
    public TMP_Text roomName;
    public string room_name;
    public RoomInfo roomInfo;

    public void Setup(RoomInfo info) {
        room_name = (string)info.CustomProperties["w_name"];
        roomInfo = info;
        roomName.text = room_name; 
    }

    public void JoinInstance() {
        Multiplayer multiplayerManager = GameObject.Find("Player").GetComponent<Multiplayer>();
        multiplayerManager.JoinInstance(roomInfo.Name);
    }
}
