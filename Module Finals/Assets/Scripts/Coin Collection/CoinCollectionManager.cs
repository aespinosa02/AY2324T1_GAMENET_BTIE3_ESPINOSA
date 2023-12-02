using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class CoinCollectionManager : MonoBehaviour
{
    public GameObject[] playerPrefabs;
    public Transform[] startingPositions;
    public GameObject[] finisherTextTMPRO;

    public static CoinCollectionManager instance = null;

    public TextMeshProUGUI timeText;

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
                GameObject player = PhotonNetwork.Instantiate(playerPrefabs[(int)playerSelectionNumber].name, instantiatePosition, Quaternion.identity);
                player.GetComponent<PlayerSetup>().SetSpawnPoint(startingPositions[actorNumber-1].transform);
            }
        }

        foreach (GameObject go in finisherTextTMPRO)
        {
            go.SetActive(false);
        }
    }
}
