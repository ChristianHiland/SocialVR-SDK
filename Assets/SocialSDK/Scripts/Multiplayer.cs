using System.Collections.Generic;
using System.Collections;
using Photon.Realtime;
using UnityEngine;
using Photon.Pun;


namespace SocialSDK {
    public class Multiplayer : MonoBehaviourPunCallbacks {
        public SocialPlayer socialPlayer;
        public WorldHandler worldHandler;
        public PlayerNotification playerNotification;
        public string platformType = "Desktop";

        public Transform headTarget;
        public Transform leftTarget;
        public Transform rightTarget;

        bool isCreating = false;
        bool joinedRoom = false;

        // Private Data for Loading and Joining a instance.
        private string roomName;
        private WorldInfoGet nextWorldInfo;                 // World Info (Name, Publisher) for joining another world.
        private RoomOptions roomOptions;
        private bool isLoadingWorld = false;
        private bool isLoadingNextWorld = false;

        void Start() {
            nextWorldInfo = new WorldInfoGet();
            if (!PhotonNetwork.IsConnected) {
                Debug.Log("Connecting to NameServer...");
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.AutomaticallySyncScene = false;
            }
        }

        /// <summary>
        /// Creates a Photon Pun Instance (Room)
        /// </summary>
        /// <param name="worldName"></param>
        /// <param name="publisher"></param>
        public void CreateInstance(string worldName, string publisher) {
            if (isCreating || !PhotonNetwork.IsConnectedAndReady) return;

            nextWorldInfo.name = worldName;
            nextWorldInfo.publisher = publisher;

            if (PhotonNetwork.InRoom) {
                Debug.Log("In a room, leaving first...");
                isLoadingNextWorld = true;
                PhotonNetwork.LeaveRoom();
                // Logic will continue in OnLeftRoom()
            } else {
                Debug.Log("Not in a room, loading world directly...");
                SetupRoomOptions(worldName, publisher);
                worldHandler.LoadWorld(publisher, worldName);
                isLoadingWorld = true;
            }
        }


        /// <summary>
        /// Join a existing Instance (Room).
        /// </summary>
        /// <param name="roomName"></param>
        public void JoinInstance(string roomName) {
            PhotonNetwork.JoinRoom(roomName);
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

        /// <summary>
        /// Callback for when the world handler is done loading a world. and spawns a player visual (Desktop or VR soon to be added).
        /// </summary>
        public void PlayerLoadedWorld() {
            // Setup The Avatar (Capule currently)
            isLoadingWorld = false;
            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, null);
        }

        // ---------------------
        // EVENT CALLBACKS
        // ---------------------

        /// <summary>
        /// Runs When the player connects to the Photon Pun Servers.
        /// </summary>
        public override void OnConnectedToMaster() {
            Debug.Log("On Master Server. Joining Lobby...");
            PhotonNetwork.JoinLobby();
        }

        /// <summary>
        /// Runs when the player joins a lobby.
        /// </summary>
        public override void OnJoinedLobby() {
            Debug.Log("Ready for Matchmaking!");
        }

        public override void OnLeftRoom() {
            if (isLoadingNextWorld) {
                isLoadingNextWorld = false;
                SetupRoomOptions(nextWorldInfo.name, nextWorldInfo.publisher);
                worldHandler.LoadWorld(nextWorldInfo.publisher, nextWorldInfo.name);
                isLoadingWorld = true;
            }
        }

        /// <summary>
        /// Runs when a player joins a room.
        /// </summary>
        public override void OnJoinedRoom() {
            // Get Properties & Load in World.
            base.OnJoinedRoom();
            joinedRoom = true;
            isCreating = false;
            if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom) {
                if (platformType == "VR") {
                    GameObject spawnPoint = GameObject.Find("Spawn Here");
                    Vector3 pos = spawnPoint != null ? spawnPoint.transform.position : Vector3.zero;
                    GameObject vrVisual = PhotonNetwork.Instantiate("VR Visual", pos, Quaternion.identity);
                    IKTargetFollowVRRig ikManager = vrVisual.GetComponent<IKTargetFollowVRRig>();
                    SocialAvatar avatarManager = vrVisual.GetComponent<SocialAvatar>();
                    ikManager.head.vrTarget = headTarget;
                    ikManager.leftHand.vrTarget = leftTarget;
                    ikManager.rightHand.vrTarget = rightTarget;
                }
                else {
                    GameObject spawnPoint = GameObject.Find("Spawn Here");
                    Vector3 pos = spawnPoint != null ? spawnPoint.transform.position : Vector3.zero;
                    PhotonNetwork.Instantiate("Player Visual", pos, Quaternion.identity);
                }
            }
        }

        /// <summary>
        /// Try to fix the "can't see host" problem for 2nd player.
        /// </summary>
        /// <param name="newPlayer"></param>
        public override void OnPlayerEnteredRoom(Player newPlayer) {
            newPlayer.CustomProperties.TryGetValue("DisplayName", out object name);
            playerNotification.notificationText.text = $"{name} has joined this instance";
            playerNotification.ShowMessage();
            Debug.Log($"Player Joined: {newPlayer.NickName}");
        }

        public override void OnPlayerLeftRoom(Player otherPlayer) {
            otherPlayer.CustomProperties.TryGetValue("DisplayName", out object name);
            playerNotification.notificationText.text = $"{name} has left this instance";
            playerNotification.ShowMessage();
        }

        // ---------------------
        // ERROR EVENT CALLBACKS
        // ---------------------

        /// <summary>
        /// Runs when the player fails to create a run.
        /// </summary>
        /// <param name="returnCode"></param>
        /// <param name="message"></param>
        public override void OnCreateRoomFailed(short returnCode, string message) {
            // If it fails, reset your 'isCreating' flag so they can try again
            isCreating = false;
            Debug.LogError("Create Failed: " + message);
        }

        /// <summary>
        /// Handle disconnection on errors with server.
        /// </summary>
        /// <param name="cause"></param>
        public override void OnDisconnected(DisconnectCause cause) {
            Debug.LogError($"Disconnected: {cause}");
            // If disconnected, disable buttons again
        }

        // ---------------------
        // Helpers
        // ---------------------
    
        public bool CheckRoomStatus() { return PhotonNetwork.InRoom; }

        public void SetupRoomOptions(string worldName, string publisher) {
            int instanceID = 12321;
            roomName = $"{publisher}_{worldName}_{instanceID}";

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
        }

    }
}