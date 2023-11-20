using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public Camera camera;

    public TextMeshProUGUI playerName;

    public bool isRaycast;
    public GameObject turret;
    public GameObject HealthBar;

    // Start is called before the first frame update
    void Start()
    {
        this.camera = transform.Find("Camera").GetComponent<Camera>();
        if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
        {
            GetComponent<VehicleMovement>().enabled = photonView.IsMine;
            GetComponent<LapController>().enabled = photonView.IsMine;
            playerName.text = photonView.Owner.NickName;
            camera.enabled = photonView.IsMine;

            GetComponent<PlayerShooting>().enabled = false;
            GetComponent<BattleRoyaleController>().enabled = false;
            turret.SetActive(false);
            HealthBar.SetActive(false);
        }
        else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            GetComponent<VehicleMovement>().enabled = photonView.IsMine;
            GetComponent<PlayerShooting>().enabled = photonView.IsMine;
            playerName.text = photonView.Owner.NickName;
            camera.enabled = photonView.IsMine;

            GetComponent<LapController>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
