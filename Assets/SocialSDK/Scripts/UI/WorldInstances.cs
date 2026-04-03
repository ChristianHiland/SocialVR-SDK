using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using Photon.Pun;
using SocialSDK;

public class WorldInstances : MonoBehaviourPunCallbacks {
    public WorldInfoScreen worldInfoScreen;
    public GameObject instanceEntryPrefab;
    public Transform container;

    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        foreach (Transform child in container) Destroy(child.gameObject);

        string targetWorld = worldInfoScreen.worldname;

        foreach (RoomInfo info in roomList) {
            if (info.CustomProperties.ContainsKey("w_name") && (string)info.CustomProperties["w_name"] == targetWorld) {
                GameObject entry = Instantiate(instanceEntryPrefab, container);
                entry.GetComponent<InstanceEntry>().Setup(info);
            }
        }
    }
}
