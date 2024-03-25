using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PreloadManager : MonoBehaviour
{
    public static PreloadManager instance;

    public bool sessionIsValid;
    public float splashSpeed;
    public float delayInScreen;
    public GameObject invalidPanel;
    public List<TextMeshProUGUI> loadingTexts;
    public List<CanvasGroup> screens;
    public UnityEvent whenLoadingIsOver;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartCoroutine(IESplashScreen());
    }

    public void SetValidState(bool cond)
    {
        sessionIsValid = cond;
        if (sessionIsValid) DataHandler.instance.IEGetUserData();
        else invalidPanel.SetActive(true);
    }

    public void SetLoadingText(string text)
    {
        for (int i = 0; i < loadingTexts.Count; i++)
        {
            string dots = string.Empty;
            
            for(int j = 0; j < i; j++)
            {
                dots += ".";
            }

            loadingTexts[i].text = text + dots;
        }
    }

    IEnumerator GetCurrentUser()
    {
        yield return new WaitUntil(() => 
        !string.IsNullOrEmpty(SessionCodeHooker.instance.GetSessionCode()));

        DataHandler.instance.IEValidateGameSession();
        StartCoroutine(IEOpenScreen(screens[^1]));

        yield return new WaitUntil(() => 
        DataHandler.instance.currentUserData.success && 
        DataHandler.instance.validateData.success
        );

        StartCoroutine(IECloseScreen(screens[^1], true));
    }

    IEnumerator IESplashScreen()
    {
        for (int i = 0; i < screens.Count; i++)
        {
            if (i > 0) yield return new WaitForSeconds(splashSpeed);

            StartCoroutine(IEOpenScreen(screens[i]));
            yield return new WaitForSeconds(delayInScreen);

            if (i == screens.Count - 1) StartCoroutine(GetCurrentUser());
            else StartCoroutine(IECloseScreen(screens[i], false));
        }
    }

    IEnumerator IEOpenScreen(CanvasGroup screen)
    {
        if (screen.GetComponent<Animator>() != null)
            screen.GetComponent<Animator>().enabled = true;

        while (screen.alpha < 1)
        {
            screen.alpha += Time.deltaTime * splashSpeed;
            yield return null;
        }
    }

    IEnumerator IECloseScreen(CanvasGroup screen, bool userIsExist)
    {
        while (screen.alpha > 0)
        {
            screen.alpha -= Time.deltaTime * splashSpeed;
            yield return null;
        }

        if (userIsExist) 
            whenLoadingIsOver.Invoke();
    }
}
