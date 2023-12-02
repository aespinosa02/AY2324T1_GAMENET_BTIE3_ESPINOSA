using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using Photon.Pun;

public class CoinSync : MonoBehaviour
{
    [SerializeField] private int myScore = 0;
    [SerializeField] private GameObject playerOwner;
    public TextMeshProUGUI myScoreText;


    void Start()
    {
        myScore = playerOwner.GetComponent<CoinCollectorTracker>().GetScore();
        myScoreText.text = "Score : " + myScore;
    }

    public void UpdateScore()
    {
        myScore = playerOwner.GetComponent<CoinCollectorTracker>().GetScore();
        myScoreText.text = "Score : " + myScore;
    }

    public void SetPlayerOwner(GameObject playerGO)
    {
        playerOwner = playerGO;
    }
}
