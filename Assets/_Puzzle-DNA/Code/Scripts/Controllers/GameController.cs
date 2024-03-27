﻿using ActionCode.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UserDataSpace;
using Utilities;

public class GameController : SingletonMonoBehaviour<GameController>
{
    [Header("Game Data")]
    [SerializeField] GameData gameData;
    [SerializeField] BoardController boardController;
    public GameTutorialHandler gameTutorialHandler;
    [SerializeField] List<Sprite> layoutBackgrounds;

    [Header("PowerUps VFX")]
    [SerializeField] GameObject bombAnimParent;
    [SerializeField] GameObject bombAnimPrefab;
    [HideInInspector] public List<Animator> bombAnims;

    [Header("Game Settings")]
    [SerializeField] bool openGameSettings;
    [ShowIf("openGameSettings")] public bool standalone;
    [ShowIf("openGameSettings")] public bool preventInitialMatches;
    [ShowIf("openGameSettings")] public float swapSpeed;
    [ShowIf("openGameSettings")] public float fallSpeed;

    [HideInInspector] public Sprite characterPlayerSprite;
    [HideInInspector] public bool gemIsInteractable;
    [HideInInspector] public bool tutorial_is_done;

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
            if (!instance.gemIsInteractable) return;
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
        Instantiate(boardController.gameObject).GetComponent<BoardController>();
        BoardController.instance.gameData = gameData;
        SoundController.PlayMusic(BoardController.instance.gameData.GetAudioClip("bgm"), 1);
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
        Sprite currLayout = null;
        if (gameData.layoutGame != null) currLayout = gameData.layoutGame;
        else currLayout = layoutBackgrounds.Find(x => x.name == $"{gameData.boardDimension.x}x{gameData.boardDimension.y}");
        
        UIController.instance.SetupImages(gameData.backgroundGame, currLayout);
        BoardController.instance.transform.SetParent(transform.parent);
        BoardController.instance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        BoardController.width = gameData.boardDimension.x;
        BoardController.height = gameData.boardDimension.y;

        if (!standalone)
        {
            BoardController.usingUpgradedPowerUpsD = DataHandler.instance.GetPerksData().perks_ability_data.driveUpgraded;
            BoardController.usingUpgradedPowerUpsN = DataHandler.instance.GetPerksData().perks_ability_data.networkUpgraded;
            BoardController.usingUpgradedPowerUpsA = DataHandler.instance.GetPerksData().perks_ability_data.actionUpgraded;
        }

        BoardController.abilityDriveDuration = gameData.abilityDriveDuration;
        BoardController.emptyPositions = gameData.emptyGems;
        BoardController.matchCounter = 0;

        HintController.needHandAnim = gameData.usingHandHint;
        HintController.hintDelay = gameData.hintDelay;
        UIController.instance.ShowGameScreen();

        yield return new WaitForSeconds(1f);
        TouchController.cancel = true;

        yield return new WaitForSeconds(BoardController.CreateBoard());
        if (gameData.usingDialogueBonus)
        {
            DialogueBonusHandler.instance.InitDialogue(gameData.dialogues);
            gameData.dialogues.dialogueBonus.ForEach(data =>
            {
                data.isDone = false;
            });
        }

        BoardController.UpdateBoard();
        BoardController.usingTutorial = gameData.tutorialKey != string.Empty;
        if (BoardController.usingTutorial)
        {
            gameTutorialHandler = FindObjectsOfType<GameTutorialHandler>().ToList().
                Find(res => res.tutorialKey == gameData.tutorialKey);
            gameTutorialHandler.InitTutorial(BoardController.usingTutorial);
        }
        else
        {
            FindObjectOfType<GameTutorialHandler>().InitTutorial(false);
        }

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

        AfterGameOver();
        GameGenerator.instance.SetScoreGameLevel(_scoreTotal);

        yield return new WaitForSeconds(BoardController.DestroyGems() + .5f);

        UnityEvent events = new();
        events.AddListener(() =>
        {
            CommonHandler.instance.UnloadSceneAdditive("GameScene");
            DataHandler.instance.GetAudioHandler("MainMenu").SetAudiosState();
        });

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

    public void AfterGameOver()
    {
        int score = DataHandler.instance.GetUserCheckpointData().
            checkpoint_value[LevelDataHandler.instance.currentGameData.gameLevel].
            checkpoint_level_score;

        if (!DataHandler.instance.GetUserCheckpointData().
            checkpoint_value[LevelDataHandler.instance.currentGameData.gameLevel].
            game_is_done)
        {
            PerksValue perks = DataHandler.instance.GetPerksData();
            LevelData level = LevelDataHandler.instance.currentLevelData;

            perks.perks_point_data.perks_point_plus += level.perksPoinPlus;
            perks.perks_point_data.perks_point_minus += level.perksPoinMinus;
            perks.perks_point_data.total_perks_point_plus += level.perksPoinPlus;
            perks.perks_point_data.total_perks_point_minus += level.perksPoinMinus;

            if (level.usingPerkUnlocking)
            {
                foreach (UserDataSpace.PerksStage data in perks.perks_stage_datas)
                {
                    foreach (LevelData.PerksStage stage in level.perkStageForUnlocking)
                    {
                        if (data.perks_types != stage.perks_types) continue;
                        data.perks_stage_locks = stage.perks_stage_locks;
                    }
                }
            }

            MainMenuHandler.instance.PatchPerksFromMenu(delegate
            {
                if (!level.openPerksPanelAfterGame) return;
                PerksHandler.instance.OpenPerksPanel(true);
            });

            DataHandler.instance.GetUserCheckpointData().
            checkpoint_value[LevelDataHandler.instance.currentGameData.gameLevel].
            game_is_done = true;
        }
    }
}
