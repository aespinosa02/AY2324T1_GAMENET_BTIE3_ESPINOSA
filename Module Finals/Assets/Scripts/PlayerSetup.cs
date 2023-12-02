using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI playerName;
    public Camera fpsCamera;
    
    public Transform currentSpawnPoint;

    public List<Transform> possibleSpawnPoints = new List<Transform>();

    public GameObject playerScoreUIPrefab;
    public GameObject myScoreTracker;

    void Start()
    {     
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("vc"))
        {
            GetComponent<CoinCollectorTracker>().enabled = false;
            GetComponent<RaceController>().enabled = photonView.IsMine;
            myScoreTracker = null;

            foreach (Transform tf in VanillaManager.instance.respawnPoints)
            {
                possibleSpawnPoints.Add(tf);
            }

            SetSpawnPoint(possibleSpawnPoints[Random.Range(0,3)]);
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("cc"))
        {
            GetComponent<RaceController>().enabled = false;
            GetComponent<CoinCollectorTracker>().enabled = photonView.IsMine;
            GameObject playerScoreUI = Instantiate(playerScoreUIPrefab);
            playerScoreUI.GetComponent<CoinSync>().SetPlayerOwner(this.gameObject);
            myScoreTracker = playerScoreUI;

            myScoreTracker.SetActive(photonView.IsMine);

            foreach (Transform tf in CoinCollectionManager.instance.startingPositions)
            {
                possibleSpawnPoints.Add(tf);
            }

            SetSpawnPoint(possibleSpawnPoints[Random.Range(0,3)]);
        }

        GetComponent<PlayerMovementController>().enabled = photonView.IsMine;
        GetComponent<RigidbodyFirstPersonController>().enabled = photonView.IsMine;
        playerName.text = photonView.Owner.NickName;
        fpsCamera.enabled = photonView.IsMine; 
    }

    public void SetSpawnPoint(Transform transformObject)
    {
        currentSpawnPoint = transformObject;
    }

    public void RespawnPlayer()
    {
        transform.position = currentSpawnPoint.transform.position;
    }
}
