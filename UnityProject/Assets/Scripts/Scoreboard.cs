using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class Scoreboard : MonoBehaviour
{
    #region Fields
    PhotonView thisPhotonView;
    GameObject[] birdsInScene;
    GameObject[] scoresInScene;

    GameObject childGameObject1;
    GameObject childGameObject2;

    private Vector3 playerScorePos = new Vector3(0, 0, 0);
    #endregion

    public void SetTheScoreboard()
    {
        //Individual scores and birds in scene 
        scoresInScene = GameObject.FindGameObjectsWithTag("IndividualScore");
        birdsInScene = GameObject.FindGameObjectsWithTag("Bird");

        thisPhotonView = GetComponent<PhotonView>();
        
        GameObject playerScore;

        //Erase the scores in the scene if they are in scene
        if (scoresInScene.Length > 0)
        {
            DestroyScores();
        }
        
        //instantiate players scores
        if (thisPhotonView.IsMine)
        {
            foreach (var bird in birdsInScene)
            {
                playerScore = PhotonNetwork.Instantiate("PlayerScore", playerScorePos, Quaternion.identity);
            }
        }
        thisPhotonView.RPC("EqualizeTheScore", RpcTarget.All);

    }
    public void DestroyScores()
    {
        GameObject[] playersScores = GameObject.FindGameObjectsWithTag("IndividualScore");

        if (this.thisPhotonView.IsMine)
        {
            foreach (var playerScore in playersScores)
            {
                PhotonNetwork.Destroy(playerScore);
            }
        }
    }

    [PunRPC]
    public void EqualizeTheScore()
    {
        scoresInScene = GameObject.FindGameObjectsWithTag("IndividualScore");
        birdsInScene = GameObject.FindGameObjectsWithTag("Bird");

        int birdID = 0;

        foreach (var playerscore in scoresInScene)
        {
            playerscore.transform.SetParent(gameObject.transform);

            //get the childs and change them
            childGameObject1 = playerscore.transform.GetChild(0).gameObject;
            childGameObject2 = playerscore.transform.GetChild(1).gameObject;

            //Make the player names visible again and make the values of the scoreboard equals the scores of the birds
            birdsInScene[birdID].GetComponent<BirdManager>().ShowPlayerName();

            childGameObject1.GetComponent<TextMeshProUGUI>().text = birdsInScene[birdID].GetComponentInChildren<TextMeshProUGUI>().text;
            childGameObject2.GetComponent<TextMeshProUGUI>().text = birdsInScene[birdID].GetComponent<BirdManager>().meuScore.ToString();

            birdID++;
        }
    }
}
