using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using TMPro;

public class LapController : MonoBehaviourPunCallbacks
{
    public List<GameObject> lapTriggers = new List<GameObject>();

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

    void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)RaiseEventsCode.whoFinishedEventCode)
        {
            object[] data = (object[]) photonEvent.CustomData;

            string nicknameOfFinishedPlayer = (string)data[0];
            finishOrder = (int)data[1];
            int viewId = (int)data[2];

            Debug.Log(nicknameOfFinishedPlayer + " " + finishOrder);

            GameObject orderTMPro = RacingGameManager.instance.finisherTextTMPro[finishOrder -1];
            orderTMPro.SetActive(true);

            if (viewId == photonView.ViewID)
            {
                orderTMPro.GetComponent<TextMeshProUGUI>().text = finishOrder + " " + nicknameOfFinishedPlayer + " (YOU)";
                orderTMPro.GetComponent<TextMeshProUGUI>().color = Color.red;
            }
            else
            {
                orderTMPro.GetComponent<TextMeshProUGUI>().text = finishOrder + " " + nicknameOfFinishedPlayer;
            }
        }
    }

    void Start()
    {       
        if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
        {
            foreach (GameObject go in RacingGameManager.instance.lapTriggers)
            {
                lapTriggers.Add(go);
            }  
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (lapTriggers.Contains(col.gameObject))
        {
            int indexOfTrigger = lapTriggers.IndexOf(col.gameObject);

            lapTriggers[indexOfTrigger].SetActive(false);
        }
        if (col.gameObject.tag == "FinishTrigger")
        {
            GameFinish();
        }
    }

    public void GameFinish()
    {
        GetComponent<PlayerSetup>().camera.transform.parent = null;
        GetComponent<VehicleMovement>().enabled = false;

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
