using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class HoldClickReleaseButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    Action whenHolding;
    Action whenReleasing;
    Action whenClicking;
    Coroutine holdCoroutine;
    bool isPointerDown = false;
    bool isHold = false;

    public void SetEvent(Action hold, Action release, Action click)
    {
        whenHolding = hold;
        whenReleasing = release;
        whenClicking = click;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        isHold = false;
        holdCoroutine = StartCoroutine(HandleButtonHold());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (holdCoroutine != null)
            StopCoroutine(holdCoroutine);

        if (isHold)
        {
            StartCoroutine(HandleButtonRelease());
        }
        else
        {
            HandleButtonClick();
        }

        isPointerDown = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Do nothing here, click is handled in OnPointerUp
    }

    private IEnumerator HandleButtonHold()
    {
        yield return new WaitForSeconds(0.5f);

        if (isPointerDown)
        {
            whenHolding.Invoke();
            isHold = true;
        }
    }

    private IEnumerator HandleButtonRelease()
    {
        whenReleasing.Invoke();
        yield return new WaitForSeconds(0.5f);
        isHold = false;
    }

    private void HandleButtonClick()
    {
        whenClicking.Invoke();
    }
}
