using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class EventAnswerData
{
    public string answer;
    public List<EventPointData> eventPointDatas;
}

[Serializable]
public class EventPointData
{
    public int point;
    public PerksType perkType;
}

[Serializable]
public class EventData
{
    public string question;
    public ExpressionType playerExpression;
    public List<EventAnswerData> answers;
}

[Serializable]
public class EventAnswerUI
{
    public TextMeshProUGUI answerText;
    public TextMeshProUGUI minusPointText;
    public TextMeshProUGUI plusPointText;
}

public class EventHandler : MonoBehaviour
{
    public EventData currentEventData;
    public bool eventExist;

    [Header("UI Attribute")]
    public Image playerCharImage;
    public TextMeshProUGUI questionText;
    public EventAnswerUI firstAnswer;
    public EventAnswerUI secondAnswer;

    public void Init(EventData data)
    {
        currentEventData = data;
        questionText.text = data.question;

        firstAnswer.answerText.text = data.answers[0].answer;
        if (data.answers[0].eventPointDatas[0].perkType == PerksType.Drive) firstAnswer.plusPointText.text = "D";
        else if (data.answers[0].eventPointDatas[0].perkType == PerksType.Network) firstAnswer.plusPointText.text = "N";
        else if (data.answers[0].eventPointDatas[0].perkType == PerksType.Action) firstAnswer.plusPointText.text = "A";

        if (data.answers[0].eventPointDatas[1].perkType == PerksType.Drive) firstAnswer.minusPointText.text = "D";
        else if (data.answers[0].eventPointDatas[1].perkType == PerksType.Network) firstAnswer.minusPointText.text = "N";
        else if (data.answers[0].eventPointDatas[1].perkType == PerksType.Action) firstAnswer.minusPointText.text = "A";

        secondAnswer.answerText.text = data.answers[1].answer;
        if (data.answers[1].eventPointDatas[0].perkType == PerksType.Drive) secondAnswer.plusPointText.text = "D";
        else if (data.answers[1].eventPointDatas[0].perkType == PerksType.Network) secondAnswer.plusPointText.text = "N";
        else if (data.answers[1].eventPointDatas[0].perkType == PerksType.Action) secondAnswer.plusPointText.text = "A";

        if (data.answers[1].eventPointDatas[1].perkType == PerksType.Drive) secondAnswer.minusPointText.text = "D";
        else if (data.answers[1].eventPointDatas[1].perkType == PerksType.Network) secondAnswer.minusPointText.text = "N";
        else if (data.answers[1].eventPointDatas[1].perkType == PerksType.Action) secondAnswer.minusPointText.text = "A";

        playerCharImage.sprite = DataHandler.instance.currPlayerSpriteData.expressionDatas.
            Find(res => res.expressionType == data.playerExpression).sprite;

        eventExist = true;
    }

    public void ChooseFirstAnswer()
    {

    }

    public void ChooseSecondAnswer()
    {

    }
}
