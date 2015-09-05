using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    public enum State { PreGame, Playing, PostGame }

    static State currentState = State.PreGame;
    public static State CurrentState
    {
        get { return currentState; }
    }


    void Start()
    {
        PlayerController.e_catchBall += HandleCatchBall;
        Enemy.e_gotBall += HandleEnemyGotBall;
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
        currentState = State.Playing;
    }

    void GameOver()
    {
        BallFollow.following = false;
        currentState = State.PostGame;
    }
}
