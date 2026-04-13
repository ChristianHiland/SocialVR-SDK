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

    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();

    public override void OnEnable() {
        base.OnEnable();
        // If we are already in a lobby, we might need to wait for the next update
        // but clearing the container is a good start.
        foreach (Transform child in container) Destroy(child.gameObject);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        Debug.Log("Updating world List");
        UpdateCachedRoomList(roomList);
        foreach (Transform child in container) Destroy(child.gameObject);

        if (worldInfoScreen.worldname != null) {
            string targetWorld = worldInfoScreen.worldname;

            foreach (RoomInfo info in cachedRoomList) {
                if (info.CustomProperties.ContainsKey("w_name") && (string)info.CustomProperties["w_name"] == targetWorld) {
                    GameObject entry = Instantiate(instanceEntryPrefab, container);
                    entry.GetComponent<InstanceEntry>().Setup(info);
                }
            }
        }
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList) {
        foreach (RoomInfo info in roomList) {
            if (info.RemovedFromList) {
                cachedRoomList.RemoveAll(r => r.Name == info.Name);
            } else {
                int index = cachedRoomList.FindIndex(r => r.Name == info.Name);
                if (index != -1) cachedRoomList[index] = info;
                else cachedRoomList.Add(info);
            }
        }
    }
}
