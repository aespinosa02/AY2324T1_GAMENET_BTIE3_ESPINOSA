using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;
using Photon.Pun.Demo.PunBasics;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject fpsModel;
    public GameObject nonFpsModel;

    public GameObject playerUiPrefab;

    public PlayerMovementController playerMovementController;
    public Camera fpsCamera;

    private Animator animator;
    public Avatar fpsAvatar, nonFpsAvatar;

    private Shooting shooting;

    public TextMeshProUGUI playerName;

    void Start()
    {
        playerMovementController = this.GetComponent<PlayerMovementController>();
        animator = this.GetComponent<Animator>();

        fpsModel.SetActive(photonView.IsMine);
        nonFpsModel.SetActive(!photonView.IsMine);

        animator.SetBool("isLocalPlayer", photonView.IsMine);
        animator.avatar = photonView.IsMine ? fpsAvatar : nonFpsAvatar;

        shooting = this.GetComponent<Shooting>();
        
        playerName.text = photonView.Owner.NickName;

        if (photonView.IsMine)
        {
            GameObject playerUi = Instantiate(playerUiPrefab);
            playerMovementController.fixedTouchField = playerUi.transform.Find("RotationTouchFieldPanel").GetComponent<FixedTouchField>();
            playerMovementController.joystick = playerUi.transform.Find("Fixed Joystick").GetComponent<Joystick>();
            fpsCamera.enabled = true;

            playerUi.transform.Find("FireButton").GetComponent<Button>().onClick.AddListener(() => shooting.Fire());
        }
        else
        {
            playerMovementController.enabled = false;
            GetComponent<RigidbodyFirstPersonController>().enabled = false;
            fpsCamera.enabled = false;
        }
    }

    void Update()
    {
        
    }
}
