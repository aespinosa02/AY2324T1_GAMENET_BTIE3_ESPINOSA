using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LaunchManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject enterGamePanel;
    [SerializeField] private GameObject connectionStatusPanel;
    [SerializeField] private GameObject lobbyPanel;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
       enterGamePanel.SetActive(true);
       connectionStatusPanel.SetActive(false);
       lobbyPanel.SetActive(false);
    }

    // connected photon server
    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.NickName + " connected to photon servers");
        connectionStatusPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    // photon detects connected to internet
    public override void OnConnected()
    {
        Debug.Log("connected to internet");
    }

    public void ConnectToPhotonServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            connectionStatusPanel.SetActive(true);
            enterGamePanel.SetActive(false);
        }
    }
    
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning(message);
        CreateAndJoinRoom();
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    private void CreateAndJoinRoom()
    {
        string randomRoomName = "Room " + Random.Range(0, 10000);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 20;

        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.NickName + " has entered " + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.LoadLevel("GameScene");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " has entered room " + PhotonNetwork.CurrentRoom.Name
        + " Room has now " + PhotonNetwork.CurrentRoom.PlayerCount + " players");
    } 
}
