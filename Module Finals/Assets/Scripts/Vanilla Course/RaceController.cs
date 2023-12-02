using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;
using UnityStandardAssets.Characters.FirstPerson;

public class RaceController : MonoBehaviourPunCallbacks
{
    public GameObject finishPlatform;

    public List<Transform> respawnPoints = new List<Transform>();
    [SerializeField] private int respawnIndex = 0;

    public enum RaiseEventsCode
    {
        whoFinishedEventCode = 0
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

    void Start()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("vc"))
        {
            foreach (Transform tf in VanillaManager.instance.respawnPoints)
            {
                respawnPoints.Add(tf);
            }
        }
    }

    void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)RaiseEventsCode.whoFinishedEventCode)
        {
            object[] data = (object[]) photonEvent.CustomData;

            string nicknameOfFinishedPlayer = (string)data[0];
            finishOrder = (int)data[1];
            int viewId = (int)data[2];

            Debug.Log(nicknameOfFinishedPlayer + " " + finishOrder);

            GameObject orderTMPro = VanillaManager.instance.finisherTextTMPRO[finishOrder -1];
            orderTMPro.SetActive(true);

            if (viewId == photonView.ViewID)
            {
                orderTMPro.GetComponent<TextMeshProUGUI>().text = finishOrder + " " + nicknameOfFinishedPlayer + " (YOU)";
                orderTMPro.GetComponent<TextMeshProUGUI>().color = Color.red;
            }
            else if (viewId != photonView.ViewID)
            {
                orderTMPro.GetComponent<TextMeshProUGUI>().text = finishOrder + " " + nicknameOfFinishedPlayer;
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "FinishTrigger")
        {
            GameFinish();
        }
    }

    public void GameFinish()
    {
        GetComponent<PlayerSetup>().fpsCamera.transform.parent = null;
        GetComponent<PlayerMovementController>().enabled = false;
        GetComponent<RigidbodyFirstPersonController>().enabled = false;

        finishOrder++;

        string nickname = photonView.Owner.NickName;
        int viewId = photonView.ViewID;

        object[] data = new object[] {nickname, finishOrder, viewId};

        RaiseEventOptions raisedEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOption = new SendOptions
        {
            Reliability = false
        };

        PhotonNetwork.RaiseEvent((byte) RaiseEventsCode.whoFinishedEventCode, data, raisedEventOptions, sendOption);
    }

    public void SetRespawnIndex(int x)
    {
        respawnIndex = x;
        SetRespawnPoint();
    }

    private void SetRespawnPoint()
    {
        if (respawnIndex == 1)
        {
            int randomFour = Random.Range(0,3) + 4;
            GetComponent<PlayerSetup>().SetSpawnPoint(respawnPoints[randomFour]);
        }
        else if (respawnIndex == 2)
        {
            int randomFour = Random.Range(0,3) + 8;
            GetComponent<PlayerSetup>().SetSpawnPoint(respawnPoints[randomFour]);
        }
        else if (respawnIndex == 3)
        {
            int randomFour = Random.Range(0,3) + 12;
            GetComponent<PlayerSetup>().SetSpawnPoint(respawnPoints[randomFour]);
        }
    }
}
