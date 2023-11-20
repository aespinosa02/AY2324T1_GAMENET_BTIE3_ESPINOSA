using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using ExitGames.Client.Photon.StructWrapping;
using Unity.VisualScripting;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Connection Status Panel")]
    public Text connectionStatusText;

    [Header("Login UI Panel")]
    public InputField playerNameInput;
    public GameObject loginUIPanel;

    [Header("Game Options Panel")]
    public GameObject gameOptionsPanel;

    [Header("Create Room Panel")]
    public GameObject createRoomPanel;
    public InputField roomNameInputField;
    public InputField playerCountInputField;

    [Header("Join Random Room Panel")]
    public GameObject joinRandomRoomPanel;

    [Header("Show Room List Panel")]
    public GameObject showRoomListPanel;

    [Header("Inside Room Panel")]
    public GameObject insideRoomPanel;
    public Text roomInfoText;
    public GameObject playerListItemPrefab;
    public GameObject playerListParent;
    public GameObject startGameButton;

    [Header("Room List Panel")]
    public GameObject roomListPanel;
    public GameObject roomItemPrefab;
    public GameObject roomListParent;

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListGO;
    private Dictionary<int, GameObject> playerListGO;

    public static NetworkManager instance;

    #region Unity Functions
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    void Start()
    {
        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListGO = new Dictionary<string, GameObject>();
        ActivatePanel(loginUIPanel);

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Update()
    {
        connectionStatusText.text = "Connection status: " + PhotonNetwork.NetworkClientState;
    }
    #endregion
    #region UI Callbacks
    public void OnLoginButtonClicked()
    {
        string playerName = playerNameInput.text;

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("Name Empty!");
        }
        else
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void OnCreateRoomButtonClicked()
    {
        string roomName = roomNameInputField.text;

        if (string.IsNullOrEmpty (roomName)) 
        {
            roomName = "Room " + Random.Range(1000, 10000);
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)int.Parse(playerCountInputField.text);
        
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void OnCancelButtonClicked()
    {
        ActivatePanel(gameOptionsPanel);
    }

    public void OnShowRoomListButtonClicked()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        
        ActivatePanel(showRoomListPanel);
    }
    
    public void OnBackButtonClicked()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        ActivatePanel(gameOptionsPanel);
    }
    
    public void OnLeaveButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnJoinRandomRoomClicked()
    {
        ActivatePanel(joinRandomRoomPanel);
        PhotonNetwork.JoinRandomRoom();
    }
    
    public void OnStartGameButtonClicked()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }
    #endregion
    #region PUN Callbacks
    public override void OnConnected()
    {
        Debug.Log("Connected To Internet");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has connected to Photon");
        ActivatePanel(gameOptionsPanel);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " created");
        ActivatePanel(insideRoomPanel);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has joined " + PhotonNetwork.CurrentRoom.Name);
        ActivatePanel(insideRoomPanel);

        roomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + " Current Player Count "
            + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;

        if (playerListGO == null)
        {
            playerListGO = new Dictionary<int, GameObject>();
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerItem = Instantiate(playerListItemPrefab);
            playerItem.transform.SetParent(playerListParent.transform);
            playerItem.transform.localScale = Vector3.one;

            playerItem.transform.Find("PlayerNameText").GetComponent<Text>().text = player.NickName;
            playerItem.transform.Find("PlayerIndicator").gameObject.SetActive(player.ActorNumber == 
                PhotonNetwork.LocalPlayer.ActorNumber);

            playerListGO.Add(player.ActorNumber, playerItem);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListGO();

        startGameButton.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient);

        foreach (RoomInfo info in roomList)
        {
            Debug.Log(info.Name);

            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }
            }
            else
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList[info.Name] = info;
                }
                else
                {
                    cachedRoomList.Add(info.Name, info);
                }
            }
        }

        foreach (RoomInfo info in cachedRoomList.Values)
        {
            GameObject listItem = Instantiate(roomItemPrefab);
            listItem.transform.SetParent(roomListParent.transform);
            listItem.transform.localScale = Vector3.one;

            listItem.transform.Find("RoomNameText").GetComponent<Text>().text = info.Name;
            listItem.transform.Find("RoomPlayersText").GetComponent<Text>().text = "Player Count + "
                + info.PlayerCount + " / " + info.MaxPlayers;
            listItem.transform.Find("JoinRoomButton").GetComponent<Button>().onClick.AddListener(() =>
                OnJoinRoomClicked(info.Name));

            roomListGO.Add(info.Name, listItem);
        }
    }

    public override void OnLeftLobby()
    {
        ClearRoomListGO();
        cachedRoomList.Clear();
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        roomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + " Current Player Count "
            + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;

        GameObject playerItem = Instantiate(playerListItemPrefab);
        playerItem.transform.SetParent(playerListParent.transform);
        playerItem.transform.localScale = Vector3.one;

        playerItem.transform.Find("PlayerNameText").GetComponent<Text>().text = player.NickName;
        playerItem.transform.Find("PlayerIndicator").gameObject.SetActive(player.ActorNumber == 
            PhotonNetwork.LocalPlayer.ActorNumber);

        playerListGO.Add(player.ActorNumber, playerItem);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        startGameButton.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient);

        roomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + " Current Player Count "
            + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
        
        Destroy(playerListGO[otherPlayer.ActorNumber]);
        playerListGO.Remove(otherPlayer.ActorNumber);
    }

    public override void OnLeftRoom()
    {
        foreach (var gameObject in playerListGO.Values)
        {
            Destroy(gameObject);
        }
        playerListGO.Clear();
        playerListGO = null;
        ActivatePanel(gameOptionsPanel);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning(message);

        string roomName = "Room " + Random.Range(1000, 1000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }
    #endregion
    #region Private Methods
    private void OnJoinRoomClicked(string roomName)
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        PhotonNetwork.JoinRoom(roomName);
    }

    private void ClearRoomListGO()
    {
        foreach (var item in roomListGO.Values)
        {
            Destroy(item);
        }

        roomListGO.Clear();
    }
    #endregion
    #region Public Methods
    public void ActivatePanel(GameObject panelToActivate)
    {
        loginUIPanel.SetActive(panelToActivate.Equals(loginUIPanel));
        gameOptionsPanel.SetActive(panelToActivate.Equals(gameOptionsPanel));
        createRoomPanel.SetActive(panelToActivate.Equals(createRoomPanel));
        joinRandomRoomPanel.SetActive(panelToActivate.Equals(joinRandomRoomPanel));
        showRoomListPanel.SetActive(panelToActivate.Equals(showRoomListPanel));
        insideRoomPanel.SetActive(panelToActivate.Equals(insideRoomPanel));
        roomListPanel.SetActive(panelToActivate.Equals(roomListPanel));
    }
    #endregion

}
