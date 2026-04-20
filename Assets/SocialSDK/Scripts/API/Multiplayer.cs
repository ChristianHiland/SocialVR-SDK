using System.Collections.Generic;
using System.Collections;
using Photon.Realtime;
using UnityEngine;
using Photon.Pun;


namespace SocialSDK.Online {
    public class Multiplayer : MonoBehaviourPunCallbacks {
        [Header("VR & Platform")]
        public PlatformType platformType;

        public Transform headVRTarget;
        public Transform leftVRTarget;
        public Transform rightVRTarget;

        [Header("Runtime Vars")]
        // Status
        bool isCreatingRoom = false;
        bool isLoadingNextWorld = false;
        // Data
        string room_Name = "";
        WorldInfoGet nextWorldInfo = new WorldInfoGet();
        RoomOptions roomOptions;
        Room roomInfo = new Room();

        [Header("Scripts")]
        private WorldHandler _worldHandler;
        private API _api;

        [Header("Social Player")]
        public SocialPlayer socialPlayer;
        public PlayerNotification playerNotification;

        void Awake() {
            // Finding Scripts
            _worldHandler = GameObject.Find("SocialSDK").GetComponent<WorldHandler>();
            _api = GameObject.Find("SocialSDK").GetComponent<API>();

            // Check if we're connected to the network.
            if (!PhotonNetwork.IsConnected) {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.AutomaticallySyncScene = false;
            }
        }

        // ---------------------
        // Public Functions
        // ---------------------

        /// <summary>
        /// Creates a Photon Pun Instance.
        /// </summary>
        /// <param name="worldName"></param>
        /// <param name="publisher"></param>
        /// <param name="room_name"></param>
        public void CreateInstance(string worldName, string publisher, string room_name = "") {
            // Check if we're already creating, or if not connected.
            if (isCreatingRoom || !PhotonNetwork.IsConnectedAndReady) return;

            // Setting WorldInfo.
            nextWorldInfo.name = worldName;
            nextWorldInfo.publisher = publisher;

            // Checking if we're already in a room.
            if (PhotonNetwork.InRoom) {
                room_Name = room_name;
                isLoadingNextWorld = true;
                PhotonNetwork.LeaveRoom();
            } else {            // Not in a room.
                SetupRoomOptions(worldName, publisher, room_name);
                _worldHandler.LoadWorld(publisher, worldName);
                isLoadingNextWorld = true;
            }
        }

        /// <summary>
        /// Join a existing Instance (Room).
        /// </summary>
        /// <param name="instance_name"></param>
        /// <param name="worldName"></param>
        /// <param name="publisher"></param>
        public void JoinInstance(string instance_name, string worldName, string publisher) {
            SetupRoomOptions(worldName, publisher, instance_name);
            _worldHandler.LoadWorld(publisher, worldName);
            isLoadingNextWorld = true;
        }

        /// <summary>
        /// Runs when the player decides to leave, and maybe join a new Instance (Room).
        /// </summary>
        public void LeaveRoom(string worldName, string publisher) {
            nextWorldInfo.name = worldName;
            nextWorldInfo.publisher = publisher;
            isLoadingNextWorld = true;
            PhotonNetwork.LeaveRoom();
        }

        // ---------------------
        // SocialSDK Callbacks
        // ---------------------

        public void PlayerLoadedWorld() {
            if (!PhotonNetwork.IsConnectedAndReady) return;
            isLoadingNextWorld = false;
            PhotonNetwork.JoinOrCreateRoom(roomInfo.InstanceName, roomOptions, null);
        }


        // ---------------------
        // Photon Callbacks
        // ---------------------

        /// <summary>
        /// Runs When the player connects to the Photon Pun Servers.
        /// </summary>
        public override void OnConnectedToMaster() { PhotonNetwork.JoinLobby(); }

        public override void OnLeftRoom() {
            if (isLoadingNextWorld) {
                isLoadingNextWorld = false;
                StartCoroutine(_api.RemoveWorldInstance(roomInfo.worldName, roomInfo.worldPublisher, roomInfo.InstanceID));
                SetupRoomOptions(nextWorldInfo.name, nextWorldInfo.publisher, room_Name);
                _worldHandler.LoadWorld(nextWorldInfo.publisher, nextWorldInfo.name);
                isLoadingNextWorld = true;
            }
        }

        /// <summary>
        /// Runs when a player joins a room.
        /// </summary>
        public override void OnJoinedRoom() {
            // Get Properties & Load in World.
            base.OnJoinedRoom();
            isCreatingRoom = false;
            if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom) {
                SetupAvatar();
            }
        }

        // ---------------------
        // Player Photon Callbacks
        // ---------------------

        public override void OnPlayerEnteredRoom(Player newPlayer) {
            newPlayer.CustomProperties.TryGetValue("DisplayName", out object name);
            playerNotification.notificationText.text = $"{name} has joined this instance";
            playerNotification.ShowMessage();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer) {
            otherPlayer.CustomProperties.TryGetValue("DisplayName", out object name);
            playerNotification.notificationText.text = $"{name} has left this instance";
            playerNotification.ShowMessage();
        }

        // ---------------------
        // Helpers
        // ---------------------

        public void SetupAvatar() {
            if (platformType == PlatformType.VR) {
                GameObject spawnPoint = GameObject.Find("Spawn Here");
                Vector3 pos = spawnPoint != null ? spawnPoint.transform.position : Vector3.zero;
                GameObject vrVisual = PhotonNetwork.Instantiate("VR Visual", pos, Quaternion.identity);
                IKTargetFollowVRRig ikManager = vrVisual.GetComponent<IKTargetFollowVRRig>();
                SocialAvatar avatarManager = vrVisual.GetComponent<SocialAvatar>();
                ikManager.head.vrTarget = headVRTarget;
                ikManager.leftHand.vrTarget = leftVRTarget;
                ikManager.rightHand.vrTarget = rightVRTarget;
            } else {
                GameObject spawnPoint = GameObject.Find("Spawn Here");
                Vector3 pos = spawnPoint != null ? spawnPoint.transform.position : Vector3.zero;
                PhotonNetwork.Instantiate("Player Visual", pos, Quaternion.identity);
            }
        }

        public void SetupRoomOptions(string worldName, string publisher, string room_name = "") {
            int instanceID = Random.Range(0, 348939202);
            string instanceID_str = $"{instanceID}";
            roomInfo = new Room();

            // Checking if we need to make a room name, or use the one given.
            roomInfo.InstanceName = "";
            if (room_name == "") {
                roomInfo.InstanceName = $"{publisher}_{worldName}_{instanceID}";
            } else {
                roomInfo.InstanceName = room_name;
            }


            roomInfo.InstanceID = instanceID_str;
            roomInfo.worldName = worldName;
            roomInfo.worldPublisher = publisher;

            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 20;

            // Storing metadata for world
            ExitGames.Client.Photon.Hashtable roomProps = new ExitGames.Client.Photon.Hashtable();
            roomProps.Add("w_name", worldName);
            roomProps.Add("w_pub", publisher);
            roomProps.Add("owner", socialPlayer.displayName.text);

            options.CustomRoomProperties = roomProps;
            options.CustomRoomPropertiesForLobby = new string[] { "w_name", "w_pub", "owner" };

            // Save room options in InstanceCreationData.
            roomOptions = options;

            // Storing metadata for player
            ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable();
            playerProps.Add("DisplayName", socialPlayer.displayName.text);
            playerProps.Add("Rank", socialPlayer.rankText.text);

            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);
            // Send Create Instance API request to server.
            StartCoroutine(_api.CreateWorldInstance(worldName, publisher, socialPlayer.displayName.text, roomInfo.InstanceName, instanceID_str));
        }
    }
}