using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject ballPrefab;

    GameplayGUI gameplayGUI;
    CameraController cameraController;
    float gameTime = 0f;

    public enum State { PreGame, Playing, PostGame }

    static State currentState = State.PreGame;
    public static State CurrentState
    {
        get { return currentState; }
    }

    void Awake()
    {
        gameplayGUI = FindObjectOfType<GameplayGUI>();
        SetUpGame();
    }

    void Start()
    {
        PlayerController.e_catchBall += HandleCatchBall;
        Enemy.e_gotBall += HandleEnemyGotBall;
    }

    void OnDestroy()
    {
        PlayerController.e_catchBall -= HandleCatchBall;
        Enemy.e_gotBall -= HandleEnemyGotBall;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Playing:
                gameTime += Time.deltaTime;
                float roundedScore = (Mathf.Floor(gameTime * 10) / 10);
                string scoreString = roundedScore == (int)roundedScore ? roundedScore.ToString() + ".0" : roundedScore.ToString();
                gameplayGUI.SetScoreText(scoreString);
                break;
        }
    }

    void SetUpGame()
    {
        currentState = State.PreGame;
        gameplayGUI.ShowGameOverMessage(false);
        Time.timeScale = 1f;

        GameObject ballStartTile = GameObject.FindGameObjectWithTag("BallStartPos");

        if (ballStartTile)
        {
            Instantiate(ballPrefab, ballStartTile.transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("No BallStartPos found in scene");
        }

        GameObject[] playerStartTiles = GameObject.FindGameObjectsWithTag("PlayerStartPos");

        if(playerStartTiles.Length >= 2)
        {
            for(int i = 0; i < 2; i++)
            {
                GameObject player = (GameObject)Instantiate(playerPrefab, playerStartTiles[i].transform.position, Quaternion.identity);
                player.GetComponent<PlayerController>().playerNo = i + 1;
            }

            gameplayGUI.InitAmmoCounters();
        }
        else
        {
            Debug.LogError(playerStartTiles.Length + " playerStartPos objects found");
        }

        cameraController = Camera.main.gameObject.GetComponent<CameraController>();

        if(cameraController == null)
        {
            cameraController = Camera.main.gameObject.AddComponent<CameraController>();
        }

        cameraController.Init();
    }

    void HandleCatchBall(PlayerController player)
    {
        switch (currentState)
        {
            case State.PreGame:
                StartGame();
                break;
        }
    }

    void HandleEnemyGotBall(Enemy enemy)
    {
        switch (CurrentState)
        {
            case State.Playing:
                GameOver();
                break;
        }
    }

    void StartGame()
    {
        BallFollow.following = true;
        SpawnController.StartSpawning();
        gameTime = 0f;
        currentState = State.Playing;
        Debug.Log("GameController: Start Game");
    }

    void GameOver()
    {
        BallFollow.Stop();
        SpawnController.StopSpawning();
        gameplayGUI.ShowGameOverMessage(true);
        currentState = State.PostGame;
        Time.timeScale = 0f;
        Debug.Log("GameController: Game Over");
    }
}
