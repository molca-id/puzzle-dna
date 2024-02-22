using ActionCode.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities;

public class GameController : SingletonMonoBehaviour<GameController>
{
    [Header("Game Data")]
    [SerializeField] GameData gameData;
    [SerializeField] BoardController boardController;

    [Header("PowerUps VFX")]
    [SerializeField] GameObject bombAnimParent;
    [SerializeField] GameObject bombAnimPrefab;
    [SerializeField] List<Animator> bombAnims;

    [Header("Game Settings")]
    [SerializeField] bool openGameSettings;
    [ShowIf("openGameSettings")] public float swapSpeed;
    [ShowIf("openGameSettings")] public float fallSpeed;
    [ShowIf("openGameSettings")] public bool preventInitialMatches;
    [ShowIf("openGameSettings")] public bool unloadWhenGameOver;
    [ShowIf("openGameSettings")] public bool standalone;

    [HideInInspector] public Sprite characterPlayerSprite;
    [HideInInspector] public bool gemIsInteractable;
    [HideInInspector] public bool tutorialIsDone;

    Coroutine gameOver;
    int _scoreTotal = 0;
    int _scoreTemp = 0;
    int _scoreMultiplier = 1;
    float _timeLeft = 120;

    public static int scoreTemp
    {
        get { return instance._scoreTemp; }
        set
        {
            instance._scoreTemp = value * instance._scoreMultiplier;
            instance._scoreTotal += instance._scoreTemp;

            DialogueBonusHandler.instance.StartDialogue(instance._scoreTotal);
            UIController.UpdateScore(instance._scoreTotal);
            UIController.UpdateComboScore(
               instance._scoreTemp, BoardController.matchCounter
            );
        }
    }
    public static int multiplierScore
    {
        get { return instance._scoreMultiplier; }
        set
        {
            instance._scoreMultiplier = value;
        }
    }
    public static float timeLeft
    {
        get { return instance._timeLeft; }
        set
        {
            instance._timeLeft = Mathf.Max(value, 0);
            UIController.UpdateTimeLeft(instance._timeLeft);
        }
    }

    void Start()
    {
        if (standalone) 
            Init(gameData);    
    }

    public void Init(GameData data)
    {
        gameData = data;
        SoundController.PlayMusic(GameData.GetAudioClip("bgm"), 1);
        StartGame();
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
        UIController.instance.SetupImages(gameData.backgroundGame, gameData.backgroundLayout);
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
        GameTutorialHandler.instance.InitTutorial(gameData.usingTutorial);

        HintController.needHandAnim = gameData.usingHandHint;
        HintController.hintDelay = gameData.hintDelay;
        UIController.instance.ShowGameScreen();

        yield return new WaitForSeconds(1f);
        TouchController.cancel = true;

        yield return new WaitForSeconds(BoardController.CreateBoard());
        gameData.dialogueBonus.ForEach(data =>
        {
            data.isDone = false;
        });

        DialogueBonusHandler.instance.InitDialogue(
            characterPlayerSprite,
            gameData.characterInterlocutor,
            gameData.dialogueBonus
            );

        BoardController.UpdateBoard();
        PoolingMatchEffect();
    }

    public void PoolingMatchEffect()
    {
        int total = BoardController.width * BoardController.height;
        for (int i = 0; i < total; i++)
        {
            var fx = Instantiate(bombAnimPrefab);
            bombAnims.Add(fx.GetComponent<Animator>());

            fx.transform.SetParent(bombAnimParent.transform);
            fx.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }

    public void StartTimer()
    {
        timeLeft = gameData.totalTime;
        StartCoroutine(TimerSystem());
        gemIsInteractable = true;
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

        UnityEvent events = new();
        events.AddListener(() => CommonHandler.instance.UnloadSceneAdditive("GameScene"));
        UIController.instance.CloseAllCanvases(events);
    }

    public void ShowPowerUpFX(List<BaseGem> gems)
    {
        for (int i = 0; i < gems.Count; i++)
        {
            bombAnims[i].transform.SetLocalPositionAndRotation(
                gems[i].transform.position,
                Quaternion.identity
                );

            bombAnims[i].Play("Play");
        }
    }
}
