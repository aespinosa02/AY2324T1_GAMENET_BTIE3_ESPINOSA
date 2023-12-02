using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.ComponentModel;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Login UI")]
    public GameObject LoginUIPanel;
    public TMP_InputField PlayerNameInput;

    [Header("Connecting Info Panel")]
    public GameObject ConnectingInfoUIPanel;

    [Header("Creating Room Info Panel")]
    public GameObject CreatingRoomInfoUIPanel;

    [Header("GameOptions  Panel")]
    public GameObject GameOptionsUIPanel;

    [Header("Create Room Panel")]
    public GameObject CreateRoomUIPanel;
    public TMP_InputField RoomNameInputField;
    public string GameMode;

    [Header("Inside Room Panel")]
    public GameObject InsideRoomUIPanel;
    public TMP_Text RoomInfoText;
    public GameObject PlayerListPrefab;
    public GameObject PlayerListParent;
    public GameObject StartGameButton;
    public TMP_Text GameModeText;

   
    [Header("Join Random Room Panel")]
    public GameObject JoinRandomRoomUIPanel;

    private Dictionary<int, GameObject> playerListGO;

    #region Unity Methods
    void Start()
    {
        ActivatePanel(LoginUIPanel.name);
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    #endregion

    #region UI Callback Methods
    public void OnLoginButtonClicked()
    {
        string playerName = PlayerNameInput.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            ActivatePanel(ConnectingInfoUIPanel.name);

            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
            Debug.Log("Player Name is invalid!");
        }
    }
    
    public void onCancelButtonClicked()
    {
        ActivatePanel(GameOptionsUIPanel.name);
    }

    public void OnCreateRoomButtonClicked()
    {
        ActivatePanel(CreatingRoomInfoUIPanel.name);

        if (GameMode != null)
        {
            string roomName = RoomNameInputField.text;

            if (string.IsNullOrEmpty(roomName))
            {
                roomName = "Name " + Random.Range(100,1000);
            }

            RoomOptions roomOptions = new RoomOptions();
            string[] roomPropertiesInLobby = {"gm"};

            // vc = vanilla cc = coin collection
            ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() {{"gm", GameMode}};

            roomOptions.CustomRoomPropertiesForLobby = roomPropertiesInLobby;
            roomOptions.CustomRoomProperties = customRoomProperties;

            roomOptions.MaxPlayers = 4;

            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
    }

    public void OnJoinRandomRoomClicked(string gameMode)
    {
        GameMode = gameMode;

        ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable() {{"gm", gameMode}};
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    public void OnBackButtonClicked()
    {
        ActivatePanel(GameOptionsUIPanel.name);
    }

    public void OnLeaveButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnStartGameButtonClicked()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gm"))
        {
            if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("vc"))
            {
                PhotonNetwork.LoadLevel("VanillaCourseScene");
            }
            else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("cc"))
            {
                PhotonNetwork.LoadLevel("CoinCollectionCourseScene");
            }
        }
    }
    #endregion

    #region Photon Callbacks
    public override void OnConnected()
    {
        Debug.Log("Connected to Internet");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is Connected to Photon");

        ActivatePanel(GameOptionsUIPanel.name);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom + " room has been created");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has joined the " + PhotonNetwork.CurrentRoom.Name);

        ActivatePanel(InsideRoomUIPanel.name);

        object gameModeName;

        if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("gm", out gameModeName))
        {
            Debug.Log(gameModeName.ToString());

            RoomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + " ( " + PhotonNetwork.CurrentRoom.PlayerCount + " / "
                + PhotonNetwork.CurrentRoom.MaxPlayers + " )";

            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("vc"))
            {
                GameModeText.text = "Vanilla Course";
            }
            else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("cc"))
            {
                GameModeText.text = "Coin Collection Course";
            }
        }

        playerListGO = new Dictionary<int, GameObject>();

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerListItem = Instantiate(PlayerListPrefab);
            playerListItem.transform.SetParent(PlayerListParent.transform);
            playerListItem.transform.localScale = Vector3.one;
            
            playerListItem.GetComponent<PlayerListItemInitializer>().Initialize(player.ActorNumber, player.NickName);

            object isPlayerReady;

            if (player.CustomProperties.TryGetValue(Constants.PLAYER_READY, out isPlayerReady))
            {
                playerListItem.GetComponent<PlayerListItemInitializer>().SetPlayerReady((bool) isPlayerReady);
            }

            playerListGO.Add(player.ActorNumber, playerListItem); 
        }

        StartGameButton.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject playerListItem = Instantiate(PlayerListPrefab);
        playerListItem.transform.SetParent(PlayerListParent.transform);
        playerListItem.transform.localScale = Vector3.one;
        
        playerListItem.GetComponent<PlayerListItemInitializer>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

        playerListGO.Add(newPlayer.ActorNumber, playerListItem);
        
        RoomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + " ( " + PhotonNetwork.CurrentRoom.PlayerCount + " / "
            + PhotonNetwork.CurrentRoom.MaxPlayers + " )";

        StartGameButton.SetActive(CheckAllPlayerReady()); 
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(playerListGO[otherPlayer.ActorNumber].gameObject);
        playerListGO.Remove(otherPlayer.ActorNumber);
            
        RoomInfoText.text = "Room Name: " + PhotonNetwork.CurrentRoom.Name + " ( " + PhotonNetwork.CurrentRoom.PlayerCount + " / "
            + PhotonNetwork.CurrentRoom.MaxPlayers + " )";
    }

    public override void OnLeftRoom()
    {
        ActivatePanel(GameOptionsUIPanel.name);

        foreach (GameObject playerListGO in playerListGO.Values)
        {
            Destroy(playerListGO);
        }

        playerListGO.Clear();
        playerListGO = null;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        if (GameMode != null)
        {
            string roomName = RoomNameInputField.text;

            if (string.IsNullOrEmpty(roomName))
            {
                roomName = "Name " + Random.Range(100,1000);
            }

            RoomOptions roomOptions = new RoomOptions();
            string[] roomPropertiesInLobby = {"gm"};

            // vc = vanilla cc = coin collection
            ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() {{"gm", GameMode}};

            roomOptions.CustomRoomPropertiesForLobby = roomPropertiesInLobby;
            roomOptions.CustomRoomProperties = customRoomProperties;

            roomOptions.MaxPlayers = 4;

            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        GameObject playerListGOs;

        if (playerListGO.TryGetValue(targetPlayer.ActorNumber, out playerListGOs))
        {
            object isPlayerReady;
            
            if (changedProps.TryGetValue(Constants.PLAYER_READY, out isPlayerReady))
            {
                playerListGOs.GetComponent<PlayerListItemInitializer>().SetPlayerReady((bool) isPlayerReady);
            } 
        }

        StartGameButton.SetActive(CheckAllPlayerReady());
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            StartGameButton.SetActive(CheckAllPlayerReady());
        }
    }
    #endregion

    #region Public Methods
    public void ActivatePanel(string panelName)
    {
        LoginUIPanel.SetActive(LoginUIPanel.name.Equals(panelName));
        ConnectingInfoUIPanel.SetActive(ConnectingInfoUIPanel.name.Equals(panelName));
        CreatingRoomInfoUIPanel.SetActive(CreatingRoomInfoUIPanel.name.Equals(panelName));
        CreateRoomUIPanel.SetActive(CreateRoomUIPanel.name.Equals(panelName));
        GameOptionsUIPanel.SetActive(GameOptionsUIPanel.name.Equals(panelName));
        JoinRandomRoomUIPanel.SetActive(JoinRandomRoomUIPanel.name.Equals(panelName));
        InsideRoomUIPanel.SetActive(InsideRoomUIPanel.name.Equals(panelName));
    }

    public void SetGameMode(string gameMode)
    {
        GameMode = gameMode;
    }
    #endregion

    #region Private Methods
    private bool CheckAllPlayerReady()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }
        
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            
            if (player.CustomProperties.TryGetValue(Constants.PLAYER_READY, out isPlayerReady))
            {
                if (!(bool) isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        
        return true; 
    }
    #endregion
}
