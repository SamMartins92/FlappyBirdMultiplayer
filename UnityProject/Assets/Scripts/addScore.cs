using UnityEngine;
using Photon.Pun;

public class addScore : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PhotonView BirdPV = collision.gameObject.GetComponent<PhotonView>();

        if (BirdPV.IsMine)
        {
            gameManager.score++;
        }
    }
}
