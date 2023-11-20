using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using ExitGames.Client.Photon.StructWrapping;

public class DeathRaceManager : MonoBehaviour
{
    public GameObject[] vehiclePrefabs;
    public Transform[] startingPositions;
    public GameObject[] finisherTextTMPro;
    
    public static DeathRaceManager instance = null;

    public int remainingPlayers = 0;

    public TextMeshProUGUI timeText;

    [Header("Kill Report Panel")]
    public GameObject killReportPanel;
    public GameObject killStatusPrefab;
    public GameObject killStatusParent;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            object playerSelectionNumber;

            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                Debug.Log((int)playerSelectionNumber);

                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

                Vector3 instantiatePosition = startingPositions[actorNumber-1].position;
                PhotonNetwork.Instantiate(vehiclePrefabs[(int)playerSelectionNumber].name, instantiatePosition, Quaternion.identity);
            }
        }

        foreach (GameObject go in finisherTextTMPro)
        {
            go.SetActive(false);
        }

        if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            remainingPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        }
    }

    public void KillReportUpdate(Player killingPlayer, Player dyingPlayer)
    {
        GameObject listItem = Instantiate(killStatusPrefab);
        listItem.transform.SetParent(killStatusParent.transform);
        listItem.transform.localScale = Vector3.one;

        listItem.GetComponent<TextMeshProUGUI>().text = killingPlayer.NickName + " has killed " + dyingPlayer.NickName;

        Destroy(listItem, 3.0f);
    }
}
