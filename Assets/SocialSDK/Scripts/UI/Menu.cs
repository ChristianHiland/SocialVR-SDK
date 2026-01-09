using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SocialSDK;

public class Menu : MonoBehaviour {

    [Header("Worlds Tab")] 
    public GameObject worldsGroup;
    public GameObject worldTilePrefab;
    public GetWorldData worldsList;
    public Texture2D defaultWorldTexture;

    private API _api;
    
    void Start() {
        _api = GameObject.Find("SocialSDK").GetComponent<API>();
    }

    public void PopulateWorlds() {
        _api.GetWorldsOnServer();
    }

    public void PopulateWorlds2(GetWorldData data) {
        foreach (WorldInfoGet worldInfoGet in data.Worlds) {
            // Download Thumbnail.
            //string path = _api.GetWorldThumbnail(worldInfoGet.name, worldInfoGet.publisher);
            // Make worldTilePrefab.
            GameObject newTile = Instantiate(worldTilePrefab, worldsGroup.transform);
            // Get World Tile Component.
            WorldTile newTileComponent = newTile.GetComponent<WorldTile>();
            // Get Thumbnail.
            //Texture2D worldTexture = GetWorldThumbnail(path);
            // Set Up World Tile Component.
            newTileComponent.SetWorld(worldInfoGet.name, worldInfoGet.publisher, defaultWorldTexture);
        }
    }
    
    

    public Texture2D GetWorldThumbnail(string path) {
        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
            byte[] imageData = new byte[fs.Length];
            fs.Read(imageData, 0, imageData.Length);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);
            return texture;
        }
    }
    
}
