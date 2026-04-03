using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using SocialSDK;
using UnityEngine.Events;

public class WorldInfoScreen : MonoBehaviour {
    public Transform instanceContent;
    public TMP_Text worldName;
    public TMP_Text worldPublisher;
    public RawImage worldThumbnail;
    public Button createPrivateButton;
    public Button createNewInstance;
    public WorldTile worldTile;
    public string worldname;
    public Multiplayer multiplayerManager;


    public void PopulateWorldInfo(string world_name, string world_publisher, Texture2D world_thumbnail) {
        worldName.text = world_name;
        worldname = world_name;
        worldPublisher.text = world_publisher;
        worldThumbnail.texture = world_thumbnail;
        createNewInstance.onClick.RemoveAllListeners();
        createPrivateButton.onClick.RemoveAllListeners();
        UnityAction action = worldTile.CreatePrivateInstance();
        createPrivateButton.onClick.AddListener(action);
        createNewInstance.onClick.AddListener(CreateNewInstancePublic(world_name, world_publisher));
    }

    public UnityAction CreateNewInstancePublic(string world_Name, string world_publisher) {
        return () => multiplayerManager.CreateInstance(world_Name, world_publisher);
    }
}
