 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public enum GameState
{
    Menu,
    Playing
}

public enum RoleState
{
    Drive,
    Network,
    Action
}

public class GameController : SingletonMonoBehaviour<GameController>
{
    [SerializeField] GameData gameData;
    public RoleState roleState;

    [Header("Game Settings")]
    public float swapSpeed;
    public float fallSpeed;
    public bool preventInitialMatches;
    Coroutine gameOver;

    [Header("Score Data")]
    [SerializeField] int _score;
    public static int score
    {
        get { return instance._score; }
        set
        {
            instance._score = value * instance._multiplierScore;
            instance._scoreTotal += instance._score;
            UIController.UpdateScore(instance._scoreTotal);
        }
    }

    [SerializeField] int _multiplierScore;
    public static int multiplierScore
    {
        get { return instance._multiplierScore; }
        set
        {
            instance._multiplierScore = value;
        }
    }

    [SerializeField] float _timeLeft;
    public static float timeLeft
    {
        get { return instance._timeLeft; }
        set
        {
            instance._timeLeft = Mathf.Max(value, 0);
            UIController.UpdateTimeLeft(instance._timeLeft);
        }
    }

    [SerializeField] int _scoreTotal;

    public static GameState state = GameState.Menu;

    void Start()
    {
        StartGame();
        SoundController.PlayMusic(GameData.GetAudioClip("bgm"), 1);
        StartCoroutine(TimerSystem());
        RoleChanged(0);
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(BoardController.instance.ShuffleBoard());
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            if (!HintController.isShowing)
                HintController.ShowHint();
            else
                HintController.StopCurrentHint();
        }
#endif
    }

    public void RoleChanged(int value)
    {
        roleState = (RoleState)value;
    }

    IEnumerator TimerSystem()
    {
        if (state == GameState.Playing)
        {
            timeLeft--;
            if (timeLeft <= 0) GameOver();
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(TimerSystem());
    }

    public void StartGame()
    {
        StartCoroutine(IEStartGame());
    }

    IEnumerator IEStartGame()
    {
        BoardController.matchCounter = 0;
        UIController.ShowGameScreen();
        yield return new WaitForSeconds(1f);

        TouchController.cancel = true;
        yield return new WaitForSeconds(BoardController.CreateBoard());
        state = GameState.Playing;
        BoardController.UpdateBoard();
    }

    void GameOver()
    {
        if (gameOver == null)
            gameOver = StartCoroutine(IEGameOver());
    }

    IEnumerator IEGameOver()
    {

        yield return new WaitUntil(() => !BoardController.updatingBoard);

        if (timeLeft > 0)
        {
            gameOver = null;
            yield break;
        }

        TouchController.cancel = true;
        state = GameState.Menu;
        HintController.StopCurrentHint();
        HintController.StopHinting();
        UIController.ShowMsg("Game Over");
        yield return new WaitForSeconds(BoardController.DestroyGems() + .5f);
        UIController.ShowMainScreen();
        gameOver = null;
    }
}
