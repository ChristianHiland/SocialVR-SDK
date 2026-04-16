using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocialSDK;

public class WorldInstances : MonoBehaviour {
    public WorldInfoScreen worldInfoScreen;
    public Multiplayer multiplayerManager;
    public GameObject instanceEntryPrefab;
    public Transform container;

    public void PopulateInstances(InstanceList instances) {
        foreach (Transform child in container) Destroy(child.gameObject);

        foreach (Room room in instances.Rooms) {
            GameObject obj = Instantiate(instanceEntryPrefab, container);
            InstanceEntry entry = obj.GetComponent<InstanceEntry>();
            entry.Setup(room.InstanceName, multiplayerManager);
        }
    }
    
}
