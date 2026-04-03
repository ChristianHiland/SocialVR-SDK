using System.Collections.Generic;
using System.Collections;
using Photon.Realtime;
using UnityEngine;
using Photon.Pun;


namespace SocialSDK {
    public class Multiplayer : MonoBehaviourPunCallbacks {
        public SocialPlayer socialPlayer;
        public WorldHandler worldHandler;
        bool isCreating = false;
        bool joinedRoom = false;

        void Start() {
            if (!PhotonNetwork.IsConnected) {
                Debug.Log("Connecting to NameServer...");
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public void CreateInstance(string worldName, string publisher) {
            if (isCreating || !PhotonNetwork.IsConnectedAndReady) return;
            isCreating = true;
            int instanceID = 12321;
            string roomName = $"{publisher}_{worldName}_{instanceID}";

            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 20;

            // Storing metadata for world
            ExitGames.Client.Photon.Hashtable roomProps = new ExitGames.Client.Photon.Hashtable();
            roomProps.Add("w_name", worldName);
            roomProps.Add("w_pub", publisher);
            roomProps.Add("owner", socialPlayer.displayName.text);

            options.CustomRoomProperties = roomProps;
            options.CustomRoomPropertiesForLobby = new string[] { "w_name", "w_pub", "owner" };

            // Storing metadata for player
            ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable();
            playerProps.Add("DisplayName", socialPlayer.displayName.text);

            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);

            PhotonNetwork.JoinOrCreateRoom(roomName, options, null);
        }

        public void JoinInstance(string roomName) {
            PhotonNetwork.JoinRoom(roomName);
        }

        public override void OnConnectedToMaster() {
            Debug.Log("On Master Server. Joining Lobby...");
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby() {
            Debug.Log("Ready for Matchmaking!");
        }

        public override void OnDisconnected(DisconnectCause cause) {
            Debug.LogError($"Disconnected: {cause}");
            // If disconnected, disable buttons again
        }

        public override void OnJoinedRoom() {

            // Get Properties & Load in World.
            base.OnJoinedRoom();
            joinedRoom = true;
            StartCoroutine(DelayedLoad());

        }

        IEnumerator DelayedLoad() {
            yield return new WaitForSeconds(1.0f);

            var props = PhotonNetwork.CurrentRoom.CustomProperties;
            string worldName = (string)props["w_name"];
            string worldPublisher = (string)props["w_pub"];

            worldHandler.LoadWorld(worldPublisher, worldName);
        }

        public override void OnCreateRoomFailed(short returnCode, string message) {
            // If it fails, reset your 'isCreating' flag so they can try again
            isCreating = false;
            Debug.LogError("Create Failed: " + message);
        }

        public void PlayerLoadedWorld() {
            // Setup The Avatar (Capule currently)
            GameObject spawnPoint = GameObject.Find("Spawn Here");
            Vector3 pos = spawnPoint != null ? spawnPoint.transform.position : Vector3.zero;
            if (PhotonNetwork.IsConnectedAndReady && joinedRoom) {
                GameObject player = PhotonNetwork.Instantiate("Player", pos, Quaternion.identity);

            }
        }
    }
}