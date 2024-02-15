using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

[System.Serializable]
public class HintInfo
{
    public BaseGem gem;
    public BaseGem currentSwap;
    public List<BaseGem> swaps = new List<BaseGem>();

    public HintInfo(BaseGem gem)
    {
        this.gem = gem;
    }
}

public class HintController : SingletonMonoBehaviour<HintController>
{
    [SerializeField] List<HintInfo> hints = new List<HintInfo>();
    [SerializeField] GameObject handAnimObj;
    [SerializeField] float _hintDelay = 30f;
    [SerializeField] bool _needHandAnim;

    public static float hintDelay
    {
        set { instance._hintDelay = value; }
        get { return instance._hintDelay; }
    }

    public static bool needHandAnim
    {
        set { instance._needHandAnim = value; }
        get { return instance._needHandAnim; }
    }

    public static bool hasHints
    {
        get { return instance.hints.Count > 0; }
    }

    public static bool isShowing
    {
        get { return instance.currentHint != null; }
    }

    public static bool paused;

    HintInfo currentHint;
    Coroutine hinting;

    HintInfo GetHint(BaseGem gem, BaseGem otherGem)
    {
        if (!(gem && otherGem))
            return null;

        HintInfo hintInfo = null;

        HintInfo hintA = hints.Find(h => h.gem == gem);
        HintInfo hintB = hints.Find(h => h.gem == otherGem);

        BoardController.SwapGems(gem, otherGem);

        MatchInfo matchA = gem.GetMatch();
        MatchInfo matchB = otherGem.GetMatch();

        if (matchA.isValid)
        {
            hintInfo = hintA != null ? hintA : new HintInfo(gem);
            hintInfo.swaps.Add(otherGem);
        }
        else if (matchB.isValid)
        {
            hintInfo = hintB != null ? hintB : new HintInfo(otherGem);
            hintInfo.swaps.Add(gem);
        }

        BoardController.SwapGems(gem, otherGem);

        return hintInfo;
    }

    public static void FindHints()
    {
        instance.hints.Clear();

        for (int j = 0; j < BoardController.height; ++j)
        {
            for (int i = 0; i < BoardController.width; ++i)
            {
                BaseGem gem = BoardController.GetGem(i, j);
                BaseGem otherGem1 = BoardController.GetGem(i + 1, j);
                BaseGem otherGem2 = BoardController.GetGem(i, j + 1);

                if ((gem && gem.isEmpty) ||
                    (otherGem1 && otherGem1.isEmpty) ||
                    (otherGem2 && otherGem2.isEmpty))
                    continue;

                if (otherGem1 && otherGem1.type != gem.type)
                {
                    HintInfo hintInfo = instance.GetHint(gem, otherGem1);
                    if (hintInfo != null && !instance.hints.Contains(hintInfo))
                        instance.hints.Add(hintInfo);
                }

                if (otherGem2 && otherGem2.type != gem.type)
                {
                    HintInfo hintInfo = instance.GetHint(gem, otherGem2);
                    if (hintInfo != null && !instance.hints.Contains(hintInfo))
                        instance.hints.Add(hintInfo);
                }
            }
        }
    }

    public static void ShowHint()
    {
        if (hasHints && !isShowing)
        {
            HintInfo hintInfo = instance.hints[Random.Range(0, instance.hints.Count)];
            hintInfo.gem.Hint();
            hintInfo.currentSwap = hintInfo.swaps[Random.Range(0, hintInfo.swaps.Count)];
            hintInfo.currentSwap.Hint();
            instance.currentHint = hintInfo;

            if (needHandAnim) SetupHandHint();
        }
    }

    public static void StopCurrentHint()
    {
        if (isShowing)
        {
            instance.handAnimObj.SetActive(false);
            instance.currentHint.gem.Hint(false);
            instance.currentHint.currentSwap.Hint(false);
            instance.currentHint = null;
        }
    }

    public static void StartHinting()
    {
        if (instance.hinting == null && !isShowing)
            instance.hinting = instance.StartCoroutine(
                instance.IEStartHinting()
            );
    }

    public static void StopHinting()
    {
        if (instance.hinting != null)
            instance.StopCoroutine(instance.hinting);

        instance.handAnimObj.SetActive(false);
        instance.hinting = null;
    }

    static void SetupHandHint()
    {
        HintInfo info = instance.currentHint;
        BaseGem currGem = instance.currentHint.gem;
        BaseGem currSwap = instance.currentHint.currentSwap;
        Animator hand = instance.handAnimObj.GetComponent<Animator>();

        instance.handAnimObj.SetActive(true);
        instance.handAnimObj.transform.SetLocalPositionAndRotation(
            info.gem.transform.position,
            Quaternion.identity
            );

        if (currGem.position.x == currSwap.position.x)
        {
            if (currGem.position.y > currSwap.position.y)
            {
                hand.Play("Down");
            }
            else
            {
                hand.Play("Up");
            }
        }
        else
        {
            if (currGem.position.x > currSwap.position.x)
            {
                hand.Play("Left");
            }
            else
            {
                hand.Play("Right");
            }
        }
    }

    IEnumerator IEStartHinting()
    {
        paused = false;
        yield return new WaitForSecondsAndNotPaused(hintDelay, () => paused);

        ShowHint();
        instance.hinting = null;
    }
}
