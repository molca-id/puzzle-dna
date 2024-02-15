using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.IK;
using Utilities;

public class BoardController : SingletonMonoBehaviour<BoardController>
{
    public GameObject gemPrefab;
    Coroutine updateBoard = null;

    [Header("Board Attributes")]
    [SerializeField] Vector2Int _boardDimension;
    public static int width
    {
        get { return instance._boardDimension.x; }
        set { instance._boardDimension.x = value; }
    }
    public static int height
    {
        get { return instance._boardDimension.y; }
        set { instance._boardDimension.y = value; }
    }

    [Header("PowerUps Attributes")]
    [SerializeField] float _abilityDriveDuration;
    public static float abilityDriveDuration
    {
        get { return instance._abilityDriveDuration; }
        set { instance._abilityDriveDuration = value; }
    }

    [SerializeField] bool _usingPowerUps;
    public static bool usingPowerUps
    {
        get { return instance._usingPowerUps; }
        set { instance._usingPowerUps = value; }
    }

    [SerializeField] bool _usingUpgradedPowerUps;
    public static bool usingUpgradedPowerUps
    {
        get { return instance._usingUpgradedPowerUps; }
        set { instance._usingUpgradedPowerUps = value; }
    }

    [SerializeField] bool _usingTutorial;
    public static bool usingTutorial
    {
        get { return instance._usingTutorial; }
        set { instance._usingTutorial = value; }
    }

    [Header("Gem Attributes")]
    [SerializeField] List<BaseGem> _allGems = new List<BaseGem>();
    public static List<BaseGem> allGems
    {
        get { return instance._allGems; }
        set { instance._allGems = value; }
    }

    [SerializeField] List<BaseGem> _currentGems = new List<BaseGem>();
    public static List<BaseGem> currentGems
    {
        get { return instance._currentGems; }
        set { instance._currentGems = value; }
    }

    [SerializeField] List<EmptyGemData> _emptyPositions = new List<EmptyGemData>();
    public static List<EmptyGemData> emptyPositions
    {
        get { return instance._emptyPositions; }
        set { instance._emptyPositions = value; }
    }

    [SerializeField] List<Vector2Int> _fallPositions;
    public static List<Vector2Int> fallPositions
    {
        get { return instance._fallPositions; }
        set { instance._fallPositions = value; }
    }
    
    int _matchCounter;
    public static int matchCounter
    {
        get { return instance._matchCounter; }
        set
        {
            instance._matchCounter = Mathf.Min(value, GameData.maxCombo);
        }
    }

    public static BaseGem[,] gemBoard;
    public static bool updatingBoard;
    public static event Action EndUpdatingBoard;

    public virtual Func<BaseGem, bool> validateGem
    {
        get { return gem => gem.type != GemType.Special; }
    }

    // Calculate Board Position into World
    public static Vector3 GetWorldPosition(Vector2Int position)
    {
        return new Vector2(
            position.x - ((width / 2f) - 0.5f),
            position.y - ((height / 2f) - 0.5f)
        );
    }

    public static Vector3 GetWorldPosition(int x, int y)
    {
        return GetWorldPosition(new Vector2Int(x, y));
    }

    public static float CreateBoard()
    {
        gemBoard = new BaseGem[width, height];
        float maxDuration = 0;
        float delayLine = 0;
        
        for (int j = height - 1; j >= 0; --j)
        {
            for (int i = 0; i < width; ++i)
            {
                BaseGem gem = instance.CreateRandomGem(
                    i, j, GetWorldPosition(new Vector2Int(i, j + 1)),
                    delayLine
                );

                if (GameController.instance.preventInitialMatches)
                {
                    while (gem.GetMatch().isValid)
                    {
                        gem.SetType(GameData.RandomGem());
                    }
                }

                float duration = gem.MoveTo(
                    GetWorldPosition(gem.position),
                    GameController.instance.fallSpeed,
                    delayLine
                );

                if (duration > maxDuration)
                    maxDuration = duration;
            }

            delayLine = maxDuration;
        }

        return maxDuration;
    }

    BaseGem CreateGem(
        int x, int y, GemData type,
        Vector3 worldPosition, float delay,
        out float creatingDuration, GameObject prefab = null)
    {
        bool isEmptyGem = false;
        Sprite emptySprite = null;
        Vector2Int pos = new(x, y);
        BaseGem gem = Instantiate(
            prefab ? prefab : gemPrefab,
            worldPosition,
            Quaternion.identity,
            transform
        ).GetComponent<BaseGem>();

        foreach (EmptyGemData data in _emptyPositions)
        {
            if (data.positions.Contains(pos))
            {
                emptySprite = data.emptySprite;
                isEmptyGem = true;
                break;
            }
        }

        gem.SetPosition(new Vector2Int(x, y));
        if (!prefab) gem.SetType(type, emptySprite, isEmptyGem);
        creatingDuration = gem.Creating(delay);

        return gem;
    }

    BaseGem CreateGem(int x, int y, GemData type, Vector3 worldPosition, float delay)
    {
        return CreateGem(x, y, type, worldPosition, delay, out float _);
    }

    BaseGem CreateGem(int x, int y, GemData type, Vector3 worldPosition)
    {
        return CreateGem(x, y, type, worldPosition, 0, out float _);
    }

    BaseGem CreateRandomGem(int x, int y, Vector3 worldPosition, float delay, out float creatingDuration)
    {
        return CreateGem(
            x, y, GameData.RandomGem(), worldPosition,
            delay, out creatingDuration
        );
    }

    BaseGem CreateRandomGem(int x, int y, Vector3 worldPosition, float delay)
    {
        return CreateRandomGem(x, y, worldPosition, delay, out float _);
    }

    BaseGem CreateRandomGem(int x, int y, Vector3 worldPosition)
    {
        return CreateRandomGem(x, y, worldPosition, 0);
    }

    // Check if position is valid, then returns a Gem
    public static BaseGem GetGem(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return null;

        return gemBoard[x, y];
    }

    public static BaseGem GetGem(Vector2Int position)
    {
        return GetGem(position.x, position.y);
    }

    // Swap position Gems
    public static void SwapGems(BaseGem from, BaseGem to)
    {
        Vector2Int fromPosition = from.position;
        from.SetPosition(to.position);
        to.SetPosition(fromPosition);
    }

    IEnumerator IESwapGems(BaseGem from, BaseGem to)
    {

        float durationFrom = from.MoveTo(
            GetWorldPosition(to.position), GameController.instance.swapSpeed
        );
        float durationTo = to.MoveTo(
            GetWorldPosition(from.position), GameController.instance.swapSpeed
        );

        yield return new WaitForSeconds(Mathf.Max(durationFrom, durationTo));

        SwapGems(from, to);
    }

    // Check if Swap results in a Match
    public static void TryMatch(BaseGem from, BaseGem to)
    {
        instance.StartCoroutine(instance.IETryMatch(from, to));
    }

    IEnumerator IETryMatch(BaseGem from, BaseGem to)
    {
        EnableUpdateBoard(true);
        yield return StartCoroutine(IESwapGems(from, to));

        MatchInfo matchFrom = from.GetMatch();
        MatchInfo matchTo = to.GetMatch();

        if (!(matchFrom.isValid || matchTo.isValid))
        {
            yield return StartCoroutine(IESwapGems(from, to));
            EnableUpdateBoard(false);
        }
        else
        {
            HintController.StopCurrentHint();
            HintController.StopHinting();

            List<MatchInfo> matches = new List<MatchInfo>();
            List<Vector2Int> fallPositions = new List<Vector2Int>();

            matches.Add(matchFrom);
            matches.Add(matchTo);

            if (from.type == GemType.Special)
            {
                GameController.instance.ShowPowerUpFX(GetSpecialMatch(true, !from.GetComponent<BombGem>(), from, validateGem).matches);
                foreach (MatchInfo specialMatch in matchFrom.specialMatches)
                {
                    matches.Add(specialMatch);
                }
            }

            if (to.type == GemType.Special)
            {
                GameController.instance.ShowPowerUpFX(GetSpecialMatch(true, !to.GetComponent<BombGem>(), from, validateGem).matches);
                foreach (MatchInfo specialMatch in matchTo.specialMatches)
                {
                    matches.Add(specialMatch);
                }
            }

            foreach (var matchInfo in new List<MatchInfo>(matches))
            {
                if (matchInfo.isValid)
                {
                    fallPositions = MatchInfo.JoinFallPositions(
                        fallPositions, matchInfo.GetFallPositions()
                    );
                }
                else
                {
                    matches.Remove(matchInfo);
                }
            }

            yield return StartCoroutine(DestroyMatchedGems(matches));
            yield return StartCoroutine(FallGems(fallPositions));

            UpdateBoard();
        }
    }

    public static void UpdateBoard()
    {
        if (instance.updateBoard != null)
            instance.StopCoroutine(instance.updateBoard);

        instance.updateBoard = instance.StartCoroutine(instance.IEUpdateBoard());
    }

    IEnumerator IEUpdateBoard()
    {
        EnableUpdateBoard(true);

        yield return StartCoroutine(FindChainMatches());

        if (GameController.timeLeft <= 0)
        {
            EnableUpdateBoard(false);
            yield break;
        }

        HintController.FindHints();
        if (!HintController.hasHints)
        {
            yield return StartCoroutine(ShuffleBoard());
            UpdateBoard();
        }
        else
        {
            EnableUpdateBoard(false);
            matchCounter = 0;

            if (!usingTutorial) HintController.StartHinting();
            if (EndUpdatingBoard != null) EndUpdatingBoard();
        }
    }

    static void EnableUpdateBoard(bool enable)
    {
        updatingBoard = enable;
        HintController.paused = enable;
        TouchController.cancel = enable;
    }

    // Check for matches in all Board
    IEnumerator FindChainMatches()
    {
        List<BaseGem> gems = gemBoard.GetList();
        List<MatchInfo> matchInfos = new List<MatchInfo>();

        while (gems.Count > 0)
        {
            BaseGem current = gems[0];
            gems.Remove(current);

            if (current.type == GemType.Special)
            {
                continue;
            }

            MatchInfo matchInfo = current.GetMatch();
            if (matchInfo.isValid)
            {
                matchInfo.matches.ForEach(gem => gems.Remove(gem));

                MatchInfo matchInfoSameType = matchInfos.Find(
                    mi => mi.pivot.type == matchInfo.pivot.type
                );

                if (matchInfoSameType != null)
                {
                    matchInfoSameType = MatchInfo.JoinCrossedMatches(
                        matchInfoSameType, matchInfo
                    );

                    if (matchInfoSameType.isValid)
                    {
                        matchInfos.Add(matchInfoSameType);
                        continue;
                    }
                }

                matchInfos.Add(matchInfo);
            }
        }

        if (matchInfos.Count > 0)
        {
            List<Vector2Int> fallPositions = new List<Vector2Int>();
            List<MatchInfo> matchesToDestroy = new List<MatchInfo>();

            foreach (MatchInfo matchInfo in matchInfos)
            {
                matchesToDestroy.Add(matchInfo);
                fallPositions = MatchInfo.JoinFallPositions(
                    fallPositions, matchInfo.GetFallPositions()
                );
            }

            yield return StartCoroutine(DestroyMatchedGems(matchesToDestroy));
            yield return StartCoroutine(FallGems(fallPositions));
            yield return StartCoroutine(FindChainMatches());
        }
    }

    // Update position of Gems and create new ones
    IEnumerator FallGems(List<Vector2Int> fallPositions)
    {
        float maxDuration = 0;
        instance._fallPositions = fallPositions;

        foreach (Vector2Int fall in instance._fallPositions)
        {
            int fallY = 0;
            for (int y = fall.y; y < height; y++)
            {
                BaseGem gem = GetGem(fall.x, y);
                Vector2Int target = new(fall.x, y - fallY);

                if (ExistInEmptyPositions(target)) continue;
                if (gem && !gem.isEmpty)
                {
                    float duration = gem.MoveTo(
                        GetWorldPosition(target),
                        GameController.instance.fallSpeed
                    );

                    gem.SetPosition(target);
                    if (duration > maxDuration)
                        maxDuration = duration;
                }
                else
                {
                    fallY++;
                }
            }

            float delay = 0;
            for (int y = height - 1; y >= height - fallY; y--)
            {
                BaseGem gemTemp = GetGem(fall.x, y);
                if (gemTemp && gemTemp.isEmpty) continue;

                BaseGem newGem = instance.CreateRandomGem(
                    fall.x, y,
                    GetWorldPosition(new Vector2Int(
                        fall.x, y + 1
                    )), delay
                );

                float duration = newGem.MoveTo(
                    GetWorldPosition(newGem.position),
                    GameController.instance.fallSpeed,
                    delay
                );

                delay = duration;
                if (duration > maxDuration)
                    maxDuration = duration;
            }
        }

        CheckAllDuplicatedGem();
        yield return new WaitForSeconds(maxDuration);
    }

    public static List<BaseGem> RemoveEmptyGem(bool isSpecialGem, List<BaseGem> baseGems)
    {
        if (isSpecialGem) return baseGems.FindAll(gem => !gem.isEmpty);
        else return baseGems;
    }

    public static MatchInfo GetSpecialMatch(bool isSpecialGem, bool isRocketGem, BaseGem gem, Func<BaseGem, bool> validateGem)
    {
        if (usingUpgradedPowerUps)
        {
            if (isRocketGem) return GetCrossMatch(gem, isSpecialGem, validateGem);
            else return GetBomb5x5Match(gem, validateGem);
        }
        else
        {
            if (isRocketGem) return GetHorizontalMatch(gem, isSpecialGem, validateGem);
            else return GetBomb3x3Match(gem, validateGem);
        }
    }

    public static MatchInfo GetHorizontalMatch(BaseGem gem, bool isSpecialGem, Func<BaseGem, bool> validateGem)
    {
        List<BaseGem> matches = new List<BaseGem>();

        matches.Add(gem);

        BaseGem gemToCheck = GetGem(gem.position.x - 1, gem.position.y);

        while (gemToCheck && validateGem(gemToCheck))
        {
            matches.Add(gemToCheck);
            gemToCheck = GetGem(gemToCheck.position.x - 1, gemToCheck.position.y);
        }

        gemToCheck = GetGem(gem.position.x + 1, gem.position.y);

        while (gemToCheck && validateGem(gemToCheck))
        {
            matches.Add(gemToCheck);
            gemToCheck = GetGem(gemToCheck.position.x + 1, gemToCheck.position.y);
        }

        return new MatchInfo(RemoveEmptyGem(isSpecialGem, matches));
    }

    public static MatchInfo GetVerticalMatch(BaseGem gem, bool isSpecialGem, Func<BaseGem, bool> validateGem)
    {
        List<BaseGem> matches = new List<BaseGem>();

        matches.Add(gem);

        BaseGem gemToCheck = GetGem(gem.position.x, gem.position.y - 1);

        while (gemToCheck && validateGem(gemToCheck))
        {
            matches.Add(gemToCheck);
            gemToCheck = GetGem(gemToCheck.position.x, gemToCheck.position.y - 1);
        }

        gemToCheck = GetGem(gem.position.x, gem.position.y + 1);

        while (gemToCheck && validateGem(gemToCheck))
        {
            matches.Add(gemToCheck);
            gemToCheck = GetGem(gemToCheck.position.x, gemToCheck.position.y + 1);
        }

        return new MatchInfo(RemoveEmptyGem(isSpecialGem, matches));
    }

    public static MatchInfo GetCrossMatch(BaseGem gem, bool isSpecialGem, Func<BaseGem, bool> validateGem)
    {
        List<BaseGem> matches = new List<BaseGem>();

        MatchInfo horizontal = GetHorizontalMatch(gem, isSpecialGem, validateGem);
        MatchInfo vertical = GetVerticalMatch(gem, isSpecialGem, validateGem);

        MatchInfo matchInfo = new MatchInfo();

        int crossCheck = 0;
        while (!horizontal.isValid && crossCheck < vertical.matches.Count)
        {
            if (vertical.isValid)
            {
                horizontal = GetHorizontalMatch(
                    vertical.matches[crossCheck], 
                    isSpecialGem, 
                    validateGem
                );
            }
            else
            {
                break;
            }
            crossCheck++;
        }

        crossCheck = 0;
        while (!vertical.isValid && crossCheck < horizontal.matches.Count)
        {
            if (horizontal.isValid)
            {
                vertical = GetVerticalMatch(
                    horizontal.matches[crossCheck], 
                    isSpecialGem, 
                    validateGem
                );
            }
            else
            {
                break;
            }
            crossCheck++;
        }

        MatchInfo cross = MatchInfo.JoinCrossedMatches(horizontal, vertical);

        if (!cross.isValid)
            if (horizontal.isValid) return horizontal;
            else return vertical;

        return new MatchInfo(RemoveEmptyGem(isSpecialGem, cross.matches));
    }

    public static MatchInfo GetBomb3x3Match(BaseGem gem, Func<BaseGem, bool> validateGem)
    {
        List<BaseGem> matches = new List<BaseGem>();

        matches.Add(gem);

        for (int x = gem.position.x - 1; x <= gem.position.x + 1; x++)
        {
            for (int y = gem.position.y - 1; y <= gem.position.y + 1; y++)
            {
                BaseGem gemToCheck = GetGem(x, y);
                if (gemToCheck) matches.Add(gemToCheck);
            }
        }

        return new MatchInfo(RemoveEmptyGem(true, matches));
    }

    public static MatchInfo GetBomb5x5Match(BaseGem gem, Func<BaseGem, bool> validateGem)
    {
        List<BaseGem> matches = new List<BaseGem>();

        matches.Add(gem);

        for (int x = gem.position.x - 2; x <= gem.position.x + 2; x++)
        {
            for (int y = gem.position.y - 2; y <= gem.position.y + 2; y++)
            {
                BaseGem gemToCheck = GetGem(x, y);
                if (gemToCheck) matches.Add(gemToCheck);
            }
        }

        return new MatchInfo(RemoveEmptyGem(true, matches));
    }

    public IEnumerator ShuffleBoard()
    {
        yield return new WaitForSeconds(.25f);

        float maxDuration = 0;
        List<Vector2Int> emptyPos = new List<Vector2Int>();

        _emptyPositions.ForEach(pos =>
        {
            pos.positions.ForEach(poss =>
            {
                emptyPos.Add(poss);
            });
        });

        gemBoard = Miscellaneous.ShuffleMatrix(gemBoard, emptyPos);

        for (int j = 0; j < height; ++j)
        {
            for (int i = 0; i < width; ++i)
            {
                BaseGem gemTemp = GetGem(i, j);
                gemTemp.SetPosition(new Vector2Int(i, j));
                float duration = gemTemp.MoveTo(
                    GetWorldPosition(gemTemp.position),
                    GameController.instance.fallSpeed * (
                        gemTemp.transform.position -
                        GetWorldPosition(gemTemp.position)
                    ).magnitude / 4
                );

                if (duration > maxDuration)
                    maxDuration = duration;
            }
        }

        yield return new WaitForSeconds(maxDuration);
    }

    IEnumerator DestroyMatchedGems(List<MatchInfo> matches)
    {
        float maxDuration = 0;
        int score = 0;

        foreach (MatchInfo matchInfo in matches)
        {
            bool specialGemExist = false;
            float duration = DestroyGems(matchInfo.matches, matchInfo.pivot);

            foreach (BaseGem gem in matchInfo.matches)
            {
                if (gem.type == GemType.Special)
                {
                    specialGemExist = true;
                    gem.DestroyGem();
                }
            }

            if (matchInfo.IsPowerUpMatch() &&
                matchInfo.matches.Count >= 4 &&
                usingPowerUps)
            {
                if (matchInfo.GetMatchType() == GemType.Drive)
                {
                    StartCoroutine(UIController.instance.DriveMultiplier());
                }
                else if (!specialGemExist)
                {
                    GameObject specialGem = null;

                    if (matchInfo.GetMatchType() == GemType.Action) 
                        specialGem = GameData.GetSpecialGem("Bomb");
                    else if (matchInfo.GetMatchType() == GemType.Network) 
                        specialGem = GameData.GetSpecialGem("Rocket");

                    float newGemDuration = 0.0f;
                    BaseGem newGem = CreateGem(
                        matchInfo.pivot.position.x,
                        matchInfo.pivot.position.y,
                        GameData.GemOfType(GemType.Special),
                        GetWorldPosition(matchInfo.pivot.position + Vector2Int.up),
                        0, out newGemDuration, specialGem
                    );

                    newGem.MoveTo(
                        GetWorldPosition(newGem.position),
                        GameController.instance.fallSpeed
                    );

                    duration += newGemDuration;
                }
            }

            if (duration > maxDuration)
                maxDuration = duration;

            score += matchInfo.GetScore() * GameController.multiplierScore * 100;
            matchCounter++;
        }

        int matchTemp;
        if (matchCounter <= 1) matchTemp = 0;
        else matchTemp = matchCounter;

        GameController.scoreTemp = score + matchTemp;
        UIController.ShowMsg($"{GameData.GetComboMessage(matchCounter - 1)}");
        SoundController.PlaySfx(GameData.GetAudioClip("match"));

        if (!GameController.instance.tutorialIsDone)
            GameTutorialHandler.instance.FinishTutorial();

        yield return new WaitForSeconds(maxDuration / 2);
    }

    public static float DestroyGems(List<BaseGem> matches = null, bool moveToPivot = false)
    {
        Vector3 pivotPosition = Vector3.zero;

        if (matches == null)
        {
            matches = gemBoard.GetList();
            moveToPivot = false;
        }
        else if (moveToPivot && matches.Count > 0)
        {
            pivotPosition = GetWorldPosition(matches[0].position);
        }

        float maxDuration = 0;
        foreach (BaseGem gem in matches)
        {
            gemBoard[gem.position.x, gem.position.y] = null;
            float duration = gem.Matched();

            if (moveToPivot)
                duration = Mathf.Max(duration, gem.MoveTo(
                    pivotPosition,
                    GameController.instance.fallSpeed
                ));

            if (duration > maxDuration)
                maxDuration = duration;

            Destroy(gem.gameObject, maxDuration);
        }

        return maxDuration;
    }

    public bool ExistInEmptyPositions(Vector2Int pos)
    {
        bool exist = false;
        foreach (EmptyGemData empty in emptyPositions)
        {
            if (empty.positions.Contains(pos))
            {
                exist = true;
                break;
            }
        }

        return exist;
    }

    void CheckAllDuplicatedGem()
    {
        currentGems.Clear();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                currentGems.Add(GetGem(x, y));
            }
        }

        DestroyChildrenWithoutComponentOfType<BaseGem>();
    }

    void DestroyChildrenWithoutComponentOfType<T>() where T : Component
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (!child.GetComponent<T>() || 
                !currentGems.Contains(child.GetComponent<BaseGem>()))
            {
                Destroy(child.gameObject);
            }
        }
    }
}
