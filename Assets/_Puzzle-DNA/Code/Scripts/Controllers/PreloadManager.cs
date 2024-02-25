using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PreloadManager : MonoBehaviour
{
    public float splashSpeed;
    public float delayInScreen;
    public List<CanvasGroup> screens;
    public UnityEvent whenLoadingIsOver;

    private void Start()
    {
        StartCoroutine(IESplashScreen());
    }

    IEnumerator GetCurrentUser()
    {
        Debug.Log("Getting User Data...");

        StartCoroutine(IEOpenScreen(screens[^1]));
        StartCoroutine(APIManager.instance.GetDataCoroutine(DataHandler.instance.codeTemp, res =>
        {
            DataHandler.instance.userData = JsonUtility.FromJson<UserData>(res);
        }));

        yield return new WaitUntil(() => DataHandler.instance.userData.success);
        StartCoroutine(IECloseScreen(screens[^1], true));
    }

    IEnumerator IESplashScreen()
    {
        for (int i = 0; i < screens.Count; i++)
        {
            StartCoroutine(IEOpenScreen(screens[i]));
            yield return new WaitForSeconds(delayInScreen + splashSpeed);

            if (i == screens.Count - 1)
            {
                StartCoroutine(GetCurrentUser());
            }
            else
            {
                StartCoroutine(IECloseScreen(screens[i], false));
            }
        }
    }

    IEnumerator IEOpenScreen(CanvasGroup screen)
    {
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
