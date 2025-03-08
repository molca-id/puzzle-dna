using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class EventAnswerData
{
    [TextArea(4, 4)] public string answerId;
    [TextArea(4, 4)] public string answerEn;
    [TextArea(4, 4)] public string answerMy;
}

[Serializable]
public class EventData
{
    public bool autoOpenPerks;
    [Space]
    [TextArea(7, 7)] public string questionId;
    [TextArea(7, 7)] public string questionEn;
    [TextArea(7, 7)] public string questionMy;
    [Space]
    public ExpressionType playerExpression;
    public Sprite characterSprite;
    public List<EventAnswerData> answers;
}

[Serializable]
public class EventAnswerUI
{
    public TextMeshProUGUI answerText;
    public TextMeshProUGUI plusPointText;
    public TextMeshProUGUI minusPointText;
    public GameObject checkObject;
    public Animator animator;
}

public class EventHandler : MonoBehaviour
{
    public int splashSpeed;
    public int answerChosen;
    public GameObject eventPanel;
    public EventData currentEventData;

    [Header("UI Attribute")]
    public Image playerCharImage;
    public TextMeshProUGUI questionText;
    public EventAnswerUI firstAnswer;
    public EventAnswerUI secondAnswer;

    public void Init(EventData data)
    {
        currentEventData = data;
        eventPanel.SetActive(true);
        StartCoroutine(IEOpenScreen(eventPanel.GetComponent<CanvasGroup>()));

        //set question text
        if (DataHandler.instance.GetLanguage() == "id")
        {
            questionText.text = currentEventData.questionId;
            firstAnswer.answerText.text = currentEventData.answers[0].answerId;
            secondAnswer.answerText.text = currentEventData.answers[1].answerId;
        }
        else if (DataHandler.instance.GetLanguage() == "en")
        {
            questionText.text = currentEventData.questionEn;
            firstAnswer.answerText.text = currentEventData.answers[0].answerEn;
            secondAnswer.answerText.text = currentEventData.answers[1].answerEn;
        }
        else if (DataHandler.instance.GetLanguage() == "my")
        {
            questionText.text = currentEventData.questionMy;
            firstAnswer.answerText.text = currentEventData.answers[0].answerMy;
            secondAnswer.answerText.text = currentEventData.answers[1].answerMy;
        }

        if (currentEventData.playerExpression == ExpressionType.Unknown)
            playerCharImage.sprite = currentEventData.characterSprite;
        else
            playerCharImage.sprite = DataHandler.instance.GetPlayerSprite(currentEventData.playerExpression);
        
        ChooseFirstAnswer();
    }

    public void ChooseFirstAnswer()
    {
        firstAnswer.checkObject.SetActive(true);
        firstAnswer.animator.SetBool("isAnimated", true);

        secondAnswer.checkObject.SetActive(false);
        secondAnswer.animator.SetBool("isAnimated", false);
        
        answerChosen = 0;
    }

    public void ChooseSecondAnswer()
    {
        firstAnswer.checkObject.SetActive(false);
        firstAnswer.animator.SetBool("isAnimated", false);

        secondAnswer.checkObject.SetActive(true);
        secondAnswer.animator.SetBool("isAnimated", true);

        answerChosen = 1;
    }

    public void SubmitAnswer()
    {
        
    }

    #region OpenClosePanel
    IEnumerator IEOpenScreen(CanvasGroup screen, Action executeAfter = null)
    {
        screen.gameObject.SetActive(true);
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
        screen.gameObject.SetActive(false);
    }
    #endregion
}
