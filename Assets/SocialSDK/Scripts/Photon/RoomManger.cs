using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomManger : MonoBehaviourPunCallbacks {
    public GameObject desktopPlayerPrefab;
    public Transform spawnPoint;

    void Start() {
        Debug.Log("Connecting...");

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
        base.OnConnectedToMaster();

        Debug.Log("Connected to Server");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() {
        base.OnJoinedLobby();

        PhotonNetwork.JoinOrCreateRoom("Testing", null, null);
        Debug.Log("Connected and in a Room");
    }

    public override void OnJoinedRoom() {
        base.OnJoinedRoom();

        Debug.Log("Joined Room!");

        GameObject _player = PhotonNetwork.Instantiate(desktopPlayerPrefab.name, spawnPoint.position, Quaternion.identity);
        
    }
}
