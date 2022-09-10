using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    #region Fields
    //Network
    PhotonView photonView;

    //Game phases related
    public bool gameStarted = false;
    public bool gameIsOver = false;
    int readysCheck = 0;

    [Space(10)] // 10 pixels of spacing here.

    //UI
    private GameObject gameOverUI;
    private GameObject ScoreUI;
    private GameObject W8forTheOtherPlayerText;
    [SerializeField] private GameObject ScoreCanvas;
    [SerializeField] private GameObject imReadyButton;

    //Countdown
    private GameObject countDownMenu;
    private GameObject countDownUI;
    private int countDownInt;
    [Space(10)] // 10 pixels of spacing here.

    //Pipes
    [SerializeField] private GameObject PipeGroup;
    private GameObject[] Pipes;
    Vector3 spwanPosition;
    [SerializeField] private float maxTime = 1;
    [SerializeField] private float height;
    private float timer = 0;
    [Space(10)] // 10 pixels of spacing here.

    //Players
    [SerializeField] private GameObject[] Birds;
    [SerializeField] private Vector3 BirdsInicialPos;
    [SerializeField] Vector3 PlayerInicialPos;
    public bool haveMyBird = false;
    [Space(10)] // 10 pixels of spacing here.

    //Score related
    static public int score = 0;
    private GameObject Score;
    public TextMeshProUGUI ScoreText;
    [SerializeField] Scoreboard ScoreBoard;
    [Space(10)] // 10 pixels of spacing here.

    //Sound related
    public AudioSource BackgroundMusic;

    #endregion

    #region Awake Start Update
    private void Awake()
    {
        FindAllUI();
    }

    private void Start()
    {
        score = 0;

        spwanPosition = new Vector3(8, 0, 0);
        photonView = GetComponent<PhotonView>();
       
        HideUIAfterFindIt();
    }

    private void Update()
    {
        InstantiatePipes();
        UpdateScore();
        DestroyPipesIfGameIsOver();
    }
    #endregion

    #region Inicializations
    private void FindAllUI()
    {
        gameOverUI = GameObject.Find("GameOver");
        ScoreUI = GameObject.Find("ScoreCanvas");
        Score = GameObject.Find("Score");
        countDownMenu = GameObject.Find("Countdown");
        countDownUI = GameObject.Find("CountdownText");
        W8forTheOtherPlayerText = GameObject.Find("w8 for the other player");
    }
    private void HideUIAfterFindIt()
    {
        gameOverUI.SetActive(false);
        ScoreUI.SetActive(false);
        countDownMenu.SetActive(false);
        W8forTheOtherPlayerText.SetActive(false);
    }
    #endregion

    #region Instantiates
    public void InstantiatePlayer()
    {
        if (haveMyBird == false)
        {
            int selectedCharacter = PlayerPrefs.GetInt("selectedCharacter");
            string prefab = Birds[selectedCharacter].name;
            PhotonNetwork.Instantiate(prefab, PlayerInicialPos, Quaternion.identity);
            haveMyBird = true;
        }
    }
    private void InstantiatePipes()
    {
        if (gameStarted == true && gameIsOver == false)
        {
            //instantiate the pipes
            if (timer > maxTime)
            {
                GameObject newPipeGroup = Instantiate(PipeGroup);
                newPipeGroup.transform.position = spwanPosition + new Vector3(0, Random.Range(-height, height), 0);
                Destroy(newPipeGroup, 10);
                timer = 0;
            }
            timer += Time.deltaTime;
        }
    }
    #endregion

    #region Before game start
    public void ImReady()
    {
        photonView = GetComponent<PhotonView>();
        photonView.RPC("CheckIfPlayersReady", RpcTarget.All);
        
    }

    [PunRPC]
    public void CheckIfPlayersReady()
    {
        readysCheck += 1;
        Debug.Log("mais um");
        if (readysCheck == 2)
        {
            Start4All();
            readysCheck = 0;
        }
        Debug.Log(readysCheck.ToString());
    }
    #endregion

    #region Startgame
    private void Start4All()
    {
        photonView = GetComponent<PhotonView>();
        photonView.RPC("StartTwo", RpcTarget.All);
    }

    [PunRPC]
    public void StartTwo()
    {
        StartCoroutine("CountdownToStart");
    }

    IEnumerator CountdownToStart()
    {
        W8forTheOtherPlayerText.SetActive(false);
        countDownMenu.SetActive(true);
        TextMeshProUGUI countDownText = countDownUI.GetComponent<TextMeshProUGUI>();
        countDownInt = 3;

        while (countDownInt > 0)
        {
            countDownText.text = countDownInt.ToString();

            yield return new WaitForSeconds(1f);

            countDownInt--;
        }

        countDownText.text = "GO!";

        //CorrectThePosition();
        //StartGame();

        yield return new WaitForSeconds(1f);

        StartGame();
        countDownMenu.SetActive(false);
    }

    public void CorrectThePosition()
    {
        Birds = GameObject.FindGameObjectsWithTag("Bird");

        Vector3 addSpaceBetween = new Vector3(3, 3, 0);

        foreach (var bird in Birds)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                bird.transform.position = BirdsInicialPos + addSpaceBetween;
            }
            else
            {
                bird.transform.position = new Vector3(0, 3, 0);
            }
        }
    }

    void StartGame()
    {
        gameStarted = true;
        gameIsOver = false;

        score = 0;

        ScoreCanvas.SetActive(true);
        gameOverUI.SetActive(false);
        ScoreUI.SetActive(true);
        countDownMenu.SetActive(true);

        Birds = GameObject.FindGameObjectsWithTag("Bird");

        foreach (var bird in Birds)
        {
            Rigidbody2D birdRb;
            birdRb = bird.GetComponent<Rigidbody2D>();

            birdRb.bodyType = RigidbodyType2D.Dynamic;

            BirdManager birdManager = bird.GetComponent<BirdManager>();
            birdManager.birdSprite.enabled = true;
            
            //reset bord score
            bird.GetComponentInChildren<BirdManager>().meuScore = 0;
        }
        Debug.Log("Sended start to all");
    }
    #endregion

    #region GameOver
    //Called when the player collide with something
    public void GameOver()
    {
        gameIsOver = true;

        BackgroundMusic.Stop();
        gameOverUI.SetActive(true);
        imReadyButton.SetActive(true);

        Birds = GameObject.FindGameObjectsWithTag("Bird");

        foreach (var bird in Birds)
        {
            Rigidbody2D birdRb;

            birdRb = bird.GetComponent<Rigidbody2D>();
            birdRb.bodyType = RigidbodyType2D.Kinematic;
        }

        ScoreBoard.SetTheScoreboard();

    }
    private void DestroyPipesIfGameIsOver()
    {
        if (gameIsOver == true)
        {
            Pipes = GameObject.FindGameObjectsWithTag("pipeGroup");

            foreach (var pipeGroup in Pipes)
            {
                Destroy(pipeGroup);
            }
        }
    }
    #endregion

    #region Score
    private void UpdateScore()
    {
        ScoreText = Score.GetComponent<TextMeshProUGUI>();
        ScoreText.text = score.ToString();
    }

    #endregion
}