using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;

public class CountdownManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI timerText;

    public float timeToStartRace = 5.0f;

    void Start()
    {        
        if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
        {
            timerText = RacingGameManager.instance.timeText;
        }
        else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            timerText = DeathRaceManager.instance.timeText;
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (timeToStartRace > 0)
            {
                timeToStartRace -= Time.deltaTime;
                photonView.RPC("SetTime", RpcTarget.AllBuffered, timeToStartRace);
            }
            else if (timeToStartRace < 0)
            {
                photonView.RPC("StartRace", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    public void SetTime(float time)
    {
        if (time > 0)
        {
            timerText.text = time.ToString("F1");
        }
        else
        {
            timerText.text = " ";
        }
    }

    [PunRPC]
    public void StartRace()
    {        
        if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
        {
            GetComponent<VehicleMovement>().isControlEnabled = true;
        }
        else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            GetComponent<VehicleMovement>().isControlEnabled = true;
            GetComponent<PlayerShooting>().isControlEnabled = true;
        }
        this.enabled = false;
    }
}
