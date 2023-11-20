using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSelection : MonoBehaviour
{
    public GameObject[] SelectablePlayers;

    public int playerSelectionNumber;

    void Start()
    {
        playerSelectionNumber = 0;

        ActivatePlayer(playerSelectionNumber);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ActivatePlayer(int x)
    {
        foreach (GameObject go in SelectablePlayers)
        {
            go.SetActive(false);
        }

        SelectablePlayers[x].SetActive(true);

        ExitGames.Client.Photon.Hashtable playerSelectionProperties = new ExitGames.Client.Photon.Hashtable(){{Constants.PLAYER_SELECTION_NUMBER, playerSelectionNumber}};
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerSelectionProperties);
    }

    public void GoToNextPlayer(int x)
    {
        playerSelectionNumber++;

        if (playerSelectionNumber >= SelectablePlayers.Length)
        {
            playerSelectionNumber = 0;
        }

        ActivatePlayer(playerSelectionNumber);
    }

    public void GoToPreviousPlayer(int x)
    {
        playerSelectionNumber--;

        if (playerSelectionNumber < 0)
        {
            playerSelectionNumber = SelectablePlayers.Length -1;
        }

        ActivatePlayer(playerSelectionNumber);
    }
}
