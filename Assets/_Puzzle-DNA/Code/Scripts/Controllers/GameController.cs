using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

public class GameController : SingletonMonoBehaviour<GameController>
{
    [Header("Game Data")]
    [SerializeField] GameData gameData;
    [SerializeField] BoardController boardController;
    //[SerializeField] AudioListener gameListener;

    [Header("Game Settings")]
    public float swapSpeed;
    public float fallSpeed;
    public bool preventInitialMatches;
    public bool gemIsInteractable;
    public bool tutorialIsDone;
    Coroutine gameOver;

    [Header("Score Data")]
    [SerializeField] int _scoreTotal;
    [SerializeField] int _scoreTemp;
    public static int scoreTemp
    {
        get { return instance._scoreTemp; }
        set
        {
            instance._scoreTemp = value * instance._scoreMultiplier;
            instance._scoreTotal += instance._scoreTemp;
            UIController.UpdateScore(instance._scoreTotal);
            UIController.UpdateComboScore(
               instance._scoreTemp, BoardController.matchCounter
            );
        }
    }

    [SerializeField] int _scoreMultiplier;
    public static int multiplierScore
    {
        get { return instance._scoreMultiplier; }
        set
        {
            instance._scoreMultiplier = value;
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

    [Header("GameOver Event")]
    [SerializeField] UnityEvent whenGameOver;

    //private void Awake()
    //{
    //    List<AudioListener> audioListeners = FindObjectsOfType<AudioListener>().ToList();
    //    if (audioListeners.Count > 1) gameListener.enabled = false;
    //}

    void Start()
    {
        StartGame();
        SoundController.PlayMusic(GameData.GetAudioClip("bgm"), 1);
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

    IEnumerator TimerSystem()
    {
        timeLeft--;
        if (timeLeft <= 0) GameOver();
        yield return new WaitForSeconds(1f);
        StartCoroutine(TimerSystem());
    }

    public void StartGame()
    {
        StartCoroutine(IEStartGame());
    }

    IEnumerator IEStartGame()
    {
        Instantiate(boardController.gameObject).GetComponent<BoardController>();

        BoardController.instance.transform.SetParent(transform.parent);
        BoardController.instance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        BoardController.width = gameData.boardDimension.x;
        BoardController.height = gameData.boardDimension.y;

        BoardController.usingPowerUps = gameData.usingPowerUps;
        BoardController.usingUpgradedPowerUps = gameData.usingUpgradedPowerUps;
        BoardController.abilityDriveDuration = gameData.abilityDriveDuration;

        BoardController.emptyPositions = gameData.emptyGems;
        BoardController.matchCounter = 0;

        BoardController.usingTutorial = gameData.usingTutorial;
        SetTutorialState(gameData.usingTutorial);

        HintController.instance.hintDelay = gameData.hintDelay;
        UIController.ShowGameScreen();
        yield return new WaitForSeconds(1f);

        TouchController.cancel = true;
        yield return new WaitForSeconds(BoardController.CreateBoard());
        BoardController.UpdateBoard();
    }

    public void SetTutorialState(bool use)
    {
        if (use) GameTutorialHandler.instance.InitTutorial();
        else StartTimer();
    }

    public void StartTimer()
    {
        timeLeft = gameData.totalTime;
        StartCoroutine(TimerSystem());
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
        HintController.StopCurrentHint();
        HintController.StopHinting();
        UIController.ShowMsg("Game Over");

        yield return new WaitForSeconds(BoardController.DestroyGems() + .5f);
        whenGameOver.Invoke();
    }
}
