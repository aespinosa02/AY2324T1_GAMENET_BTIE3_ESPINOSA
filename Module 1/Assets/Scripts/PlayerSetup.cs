using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject camera;
    [SerializeField] private TextMeshProUGUI playerName;

    void Start()
    {
        if (photonView.IsMine)
        {
            transform.GetComponent<MovementController>().enabled = true;
            camera.GetComponent<Camera>().enabled = true;
        }
        else
        {
            transform.GetComponent<MovementController>().enabled = false;
            camera.GetComponent<Camera>().enabled = false;
        }

        playerName.text = photonView.Owner.NickName;
    }
}
