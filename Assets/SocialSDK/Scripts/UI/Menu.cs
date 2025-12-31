using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocialSDK;

public class Menu : MonoBehaviour {

    [Header("Worlds Tab")] 
    public GameObject worldsGroup;
    public GameObject worldTilePrefab;
    public GetWorldData worldsList;

    private API _api;
    
    void Start() {
        _api = GameObject.Find("SocialSDK").GetComponent<API>();
    }

    public void PopulateWorlds() {
        _api.GetWorldsOnServer();
    }

    public void PopulateWorlds2(GetWorldData data) {
        foreach (WorldInfoGet worldInfoGet in data.Worlds) {
            // Make worldTilePrefab.
            GameObject newTile = Instantiate(worldTilePrefab, worldsGroup.transform);
            // Get World Tile Component.
            WorldTile newTileComponent = newTile.GetComponent<WorldTile>();
            // Set Up World Tile Component.
            newTileComponent.SetWorld(worldInfoGet.name, worldInfoGet.publisher);
        }
    }
    
}
