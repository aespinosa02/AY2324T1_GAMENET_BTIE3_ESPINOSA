using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;

    public static GameManager instance;

    public GameObject endScreenPanel;
    public TextMeshProUGUI winnerText;

    [Header("Kill Report Panel")]
    public GameObject killReportPanel;
    public GameObject killStatusPrefab;
    public GameObject killStatusParent;

    public List <GameObject> spawnPoints;
    
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
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, SpawnPlayer(), Quaternion.identity);
        }
        endScreenPanel.SetActive(false);
    }

    void Update()
    {
        
    }

    public void EndScreen(Player killingPlayer)
    {
        endScreenPanel.SetActive(true);
        winnerText.text = "Winner is " + killingPlayer.NickName;

        //PhotonNetwork.LeaveRoom();
    }

    public void KillReportUpdate(Player killingPlayer, Player dyingPlayer)
    {                
        //Debug.Log(info.Sender.NickName + " Killed " + info.photonView.Owner.NickName);

        GameObject listItem = Instantiate(killStatusPrefab);
        listItem.transform.SetParent(killStatusParent.transform);
        listItem.transform.localScale = Vector3.one;

        listItem.GetComponent<TextMeshProUGUI>().text = killingPlayer.NickName + 
            " has killed " + dyingPlayer.NickName;

        Destroy(listItem, 3.0f);
    }

    public Vector3 SpawnPlayer()
    {
        int randomPoint = Random.Range(0, spawnPoints.Count - 1);

        return spawnPoints[randomPoint].transform.position;
    }
}
