using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;
using UnityStandardAssets.Characters.FirstPerson;

public class CoinCollectorTracker : MonoBehaviourPunCallbacks
{
    public GameObject finishPlatform;

    public List<Transform> coinSpawnPoints = new List<Transform>();

    [SerializeField] private int score = 0;

    public enum RaiseEventsCode
    {
        WhoEarnedEventCode = 0
    }

    private int finishOrder = 0;

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)RaiseEventsCode.WhoEarnedEventCode)
        {
            object[] data = (object[]) photonEvent.CustomData;

            string nicknameOfFinishedPlayer = (string)data[0];
            finishOrder = (int)data[1];
            int viewId = (int)data[2];
            int score = (int)data[3];

            Debug.Log(nicknameOfFinishedPlayer + " " + finishOrder);

            GameObject orderTMPro = CoinCollectionManager.instance.finisherTextTMPRO[finishOrder -1];
            orderTMPro.SetActive(true);

            if (viewId == photonView.ViewID)
            {
                orderTMPro.GetComponent<TextMeshProUGUI>().text = finishOrder + " " + nicknameOfFinishedPlayer + " (YOU)" + " Score: " + score;
                orderTMPro.GetComponent<TextMeshProUGUI>().color = Color.red;
            }
            else
            {
                
                orderTMPro.GetComponent<TextMeshProUGUI>().text = finishOrder + " " + nicknameOfFinishedPlayer + " Score: " + score;
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Coin")
        {
            if (GetComponent<PhotonView>().IsMine)
            {
                col.gameObject.GetComponent<MeshRenderer>().enabled = false;
                GetComponent<PhotonView>().RPC("IncrementScore", RpcTarget.AllBuffered);
                col.gameObject.GetComponent<CoinBehaviour>().StartReset();
            }
        }
    }

    [PunRPC]
    public void IncrementScore()
    {
        score++;
        GetComponent<PlayerSetup>().myScoreTracker.GetComponent<CoinSync>().UpdateScore();
        if (score == 15)
        {
            GameFinish();
        }
    }

    public int GetScore()
    {
        return score;
    }

    public void GameFinish()
    {
        GetComponent<PlayerSetup>().fpsCamera.transform.parent = null;
        GetComponent<PlayerMovementController>().enabled = false;
        GetComponent<RigidbodyFirstPersonController>().enabled = false;

        finishOrder++;

        string nickname = photonView.Owner.NickName;
        int viewId = photonView.ViewID;

        object[] data = new object[] {nickname, finishOrder, viewId, score};

        RaiseEventOptions raisedEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOption = new SendOptions
        {
            Reliability = false
        };

        PhotonNetwork.RaiseEvent((byte) RaiseEventsCode.WhoEarnedEventCode, data, raisedEventOptions, sendOption);
    }
}
