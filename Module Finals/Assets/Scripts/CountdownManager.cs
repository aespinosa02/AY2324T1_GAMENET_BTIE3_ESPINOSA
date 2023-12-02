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
        if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("vc"))
        {
            timerText = VanillaManager.instance.timeText;
        }
        else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("cc"))
        {
            timerText = CoinCollectionManager.instance.timeText;
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
        if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("vc"))
        {
            GetComponent<PlayerMovementController>().isControlEnabled = true;
        }
        else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("cc"))
        {
            GetComponent<PlayerMovementController>().isControlEnabled = true;
        }
        this.enabled = false;
    }
}
