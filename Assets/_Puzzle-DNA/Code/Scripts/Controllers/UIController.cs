using System.Collections;
using System.Collections.Generic;
using Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIController : SingletonMonoBehaviour<UIController>
{
    [Header("Screens")]
    [SerializeField] List<CanvasGroup> allCanvases;

    [Header("HUD Images")]
    [SerializeField] Image backgroundGame;
    [SerializeField] Image backgroundLayout;

    [Header("HUD Screen")]
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI comboScoreText;
    [SerializeField] TextMeshProUGUI comboMultiplierText;
    [SerializeField] TextMeshProUGUI timeLeftText;
    [SerializeField] TextMeshProUGUI msgText;

    [Header("Add On For Multiplier Score")]
    [SerializeField] float elapsedTime;
    [SerializeField] GameObject multiplierUI;
    [SerializeField] TextMeshProUGUI multiplierText;
    [SerializeField] Image timerFilledImage;

    CanvasGroup currentScreen;
    float timePulse;

    public void ShowGameScreen()
    {
        UpdateScore(GameController.scoreTemp);
        UpdateTimeLeft(GameController.timeLeft);
        allCanvases.ForEach(canvas =>
        {
            StartCoroutine(IEOpenScreen(canvas));
        });
    }

    public void CloseAllCanvases(UnityEvent events)
    {
        for (int i = 0; i < allCanvases.Count; i++)
        {
            if (i == allCanvases.Count - 1)
                StartCoroutine(IECloseScreen(allCanvases[i], events));
            else
                StartCoroutine(IECloseScreen(allCanvases[i]));
        }
    }

    public static void UpdateScore(int score)
    {
        instance.scoreText.text = $"{score}";
        instance.scoreText.transform.parent
            .GetComponent<Animator>().SetTrigger("pulse");
    }

    public static void UpdateComboScore(int comboScore, int multiplier)
    {
        instance.comboScoreText.text = $"+{comboScore / Mathf.Max(multiplier, 1)}";
        instance.comboMultiplierText.text = multiplier > 1 ? $" x{multiplier}" : "";

        instance.comboScoreText.GetComponent<Animator>().SetTrigger("pulse");
    }

    public static void UpdateTimeLeft(float timeLeft)
    {
        if (timeLeft <= 10)
        {
            if (Time.time - instance.timePulse > 1f)
            {
                instance.timeLeftText.GetComponent<Animator>().SetTrigger("pulse");
                instance.timePulse = Time.time;
                SoundController.PlaySfxInstance(BoardController.instance.gameData.GetAudioClip("click"));
            }
        }
        else
        {
            instance.timePulse = 0;
        }

        System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(timeLeft);
        string mm = timeSpan.Minutes.ToString("D2");
        string ss = timeSpan.Seconds.ToString("D2");
        instance.timeLeftText.text = $"{mm}:{ss}";
    }

    public static void ShowMsg(string msg)
    {
        instance.msgText.text = $"{msg}";
        instance.msgText.transform.GetComponent<Animator>().SetTrigger("pulse");
    }

    public void SetupImages(Sprite bgGame, Sprite bgLayout)
    {
        backgroundGame.sprite = bgGame;
        backgroundLayout.sprite = bgLayout;
        backgroundLayout.SetNativeSize();
    }

    public void SetMultiplierScoreState(bool cond)
    {
        int multiplier = BoardController.usingUpgradedPowerUpsD ? 4 : 2;
        GameController.multiplierScore = cond ? multiplier : 1;

        multiplierText.text = $"x{GameController.multiplierScore}";
        multiplierUI.SetActive(cond);
    }

    public IEnumerator DriveMultiplier()
    {
        float fillDuration = BoardController.abilityDriveDuration;
        SetMultiplierScoreState(true);
        elapsedTime = 0f;

        while (elapsedTime <= fillDuration)
        {
            elapsedTime += Time.deltaTime * 1;
            timerFilledImage.fillAmount = elapsedTime / fillDuration;
            yield return null;
        }

        timerFilledImage.fillAmount = 0f;
        SetMultiplierScoreState(false);
        yield return null;
    }

    IEnumerator IEChangeScreen(
        CanvasGroup screen,
        System.Action executeBefore = null, System.Action executeAfter = null
    )
    {
        if (executeBefore != null)
            executeBefore();

        screen.alpha = 0;
        screen.gameObject.SetActive(false);

        if (currentScreen)
        {
            while (currentScreen.alpha > 0)
            {
                currentScreen.alpha -= Time.deltaTime * 2;
                yield return null;
            }
            currentScreen.gameObject.SetActive(false);
        }

        currentScreen = screen;
        currentScreen.gameObject.SetActive(true);

        while (currentScreen.alpha < 1)
        {
            currentScreen.alpha += Time.deltaTime * 2;
            yield return null;
        }

        if (executeAfter != null)
            executeAfter();
    }

    public IEnumerator IEOpenScreen(CanvasGroup screen)
    {
        while (screen.alpha < 1)
        {
            screen.alpha += Time.deltaTime * 2;
            yield return null;
        }
    }

    public IEnumerator IECloseScreen(CanvasGroup screen, UnityEvent events = null)
    {
        while (screen.alpha > 0)
        {
            screen.alpha -= Time.deltaTime * 2;
            yield return null;
        }

        if (events != null)
            events.Invoke();
    }
}