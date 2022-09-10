using Photon.Pun;
using UnityEngine;
using TMPro;

public class BirdManager : MonoBehaviourPun, IPunObservable
{
    #region Fields
    [SerializeField] private float jumpPower = 1;
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource died;
    [SerializeField] private Rigidbody2D thisRB;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private PhotonView thisPhotonView;
    [SerializeField] private GameObject playerNameHolder;
    private Scoreboard scoreBoard;

    private gameManager gameManager;

    public bool someoneCollided = false;
    public SpriteRenderer birdSprite;
    public int meuScore;
    #endregion

    #region Start Update
    private void Start()
    {
        FindItAll();
        SharePlayerName();
    }
    void Update()
    {
        Jump();
        UpdateScore();
    }

  
    #endregion

    #region Inicializations
    private void FindItAll()
    {
        if (thisPhotonView.IsMine)
        {
            gameManager = GameObject.Find("-------------Manager--------------").GetComponent<gameManager>();
           
        }
    }
    #endregion

    #region Player Name

    private void SharePlayerName()
    {
        if (photonView.IsMine)
        {
            string playerName = PlayerPrefs.GetString("playerName");

            thisPhotonView.RPC("SetName", RpcTarget.AllBuffered, playerName);
        }
    }

    [PunRPC]
    private void SetName(string playername)
    {
        nameText.text = playername;
    }

    public void HidePlayerName()
    {
        playerNameHolder.gameObject.SetActive(false);
    }

    public void ShowPlayerName()
    {
        playerNameHolder.gameObject.SetActive(true);
    }

    #endregion

    #region Player Score

    private void UpdateScore()
    {
        if (thisPhotonView.IsMine)
        {
            meuScore = gameManager.score;

            gameManager = GameObject.Find("-------------Manager--------------").GetComponent<gameManager>();
            if (gameManager.gameIsOver == true)
            {
                SetPlayerScore();
            }
        }
    }

    private void SetPlayerScore()
    {
        meuScore = gameManager.score;

        if (photonView.IsMine)
        {
            thisPhotonView.RPC("SetScore", RpcTarget.All, meuScore);
        }
    }

    [PunRPC]
    private void SetScore(int myScore) 
    {
        meuScore = myScore;
        //Debug.Log(meuScore.ToString());
    }

    #endregion

    #region Player actions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Bird")
        {
            HidePlayerName();

            birdSprite = gameObject.GetComponent<SpriteRenderer>();
            birdSprite.enabled = false;

            if (thisPhotonView.IsMine)
            {
                gameManager.GameOver();
                died.Play();

                if (someoneCollided == false)
                {
                    //Debug.Log("Perdeste!");
                }
                someoneCollided = true;
            }
            else if (!thisPhotonView.IsMine)
            {
                someoneCollided = true;
                //Debug.Log("Ganhaste!");
            }
        }
    }
    
    private void Jump()
    {
        if (thisPhotonView.IsMine && !gameManager.gameIsOver && gameManager.gameStarted)
        {
            if (Input.GetMouseButtonDown(0))
            {
                thisPhotonView.RPC("ShareJump", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    private void ShareJump()
    {
        gameManager = GameObject.Find("-------------Manager--------------").GetComponent<gameManager>();
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.up * jumpPower;

        //Debug.Log("Jump shared!");

        if (gameManager.gameIsOver == false)
        {
            jumpSound.Play();
        }
    }
    #endregion

    #region Syncronization
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
            if (stream.IsWriting)
            {
                stream.SendNext(thisRB.position);
            }
            else
            {
                thisRB.position = (Vector2)stream.ReceiveNext();
        }

    }
    #endregion
}