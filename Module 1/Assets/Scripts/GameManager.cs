using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;

    public static GameManager instance;

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
        if (PhotonNetwork.IsConnected)
        {
            if (playerPrefab != null)
            {
                StartCoroutine(DelayedPlayerSpawn());
            }
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.NickName + " has joined the room!");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " has joined the room! " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Room has now " + PhotonNetwork.CurrentRoom.PlayerCount + " /20 players");
    }

    IEnumerator DelayedPlayerSpawn()
    {
        yield return new WaitForSeconds(3);
        int xRandom = Random.Range(-20, 20);
        int zRandom = Random.Range(-20, 20);
        PhotonNetwork.Instantiate(playerPrefab.name, 
        new Vector3(xRandom, 0, zRandom), Quaternion.identity);
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("GameLauncher");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
