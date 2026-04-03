using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class NetworkAuthenticator : MonoBehaviourPunCallbacks
{
    public Button createRoomButton;
    public Button joinRoomButton;

    void Start()
    {
        // Disable buttons initially so user can't click them too early
        if (createRoomButton) createRoomButton.interactable = false;
        if (joinRoomButton) joinRoomButton.interactable = false;

        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connecting to NameServer...");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("On Master Server. Joining Lobby...");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Ready for Matchmaking!");
        // NOW enable the buttons
        if (createRoomButton) createRoomButton.interactable = true;
        if (joinRoomButton) joinRoomButton.interactable = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError($"Disconnected: {cause}");
        // If disconnected, disable buttons again
        if (createRoomButton) createRoomButton.interactable = false;
    }
}