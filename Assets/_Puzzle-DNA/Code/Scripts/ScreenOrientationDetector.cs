using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScreenOrientationDetector : MonoBehaviour
{
    [SerializeField] bool isAnimated;
    [SerializeField] float splashSpeed;
    [SerializeField] UnityEvent whenLandscape;
    [SerializeField] UnityEvent whenPortrait;

    void Start()
    {
        StartCoroutine(DetectOrientation());
    }

    IEnumerator DetectOrientation()
    {
        if (Screen.width > Screen.height)
        {
            whenLandscape.Invoke();
            isAnimated = false;
        }
        else if (Screen.width < Screen.height &&
            !isAnimated)
        {
            whenPortrait.Invoke();
            isAnimated = true;
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(DetectOrientation());
    }

    public void OpenScreen(CanvasGroup canvas)
    {
        StartCoroutine(IEOpenScreen(canvas));
    }

    public void CloseScreen(CanvasGroup canvas)
    {
        StartCoroutine(IECloseScreen(canvas));
    }

    IEnumerator IEOpenScreen(CanvasGroup screen, Action executeAfter = null)
    {
        while (screen.alpha < 1)
        {
            screen.alpha += Time.deltaTime * splashSpeed;
            yield return null;
        }

        executeAfter?.Invoke();
    }

    IEnumerator IECloseScreen(CanvasGroup screen, Action executeAfter = null)
    {
        while (screen.alpha > 0)
        {
            screen.alpha -= Time.deltaTime * splashSpeed;
            yield return null;
        }

        executeAfter?.Invoke();
    }
}
