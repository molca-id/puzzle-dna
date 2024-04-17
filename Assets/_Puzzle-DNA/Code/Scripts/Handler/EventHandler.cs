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
    public int answerChosen;
    public PerksHandler customPerkPanel;
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

        //set first answer
        if (currentEventData.answers[0].eventPointDatas[0].perkType == PerksType.Drive) firstAnswer.plusPointText.text = "D";
        else if (currentEventData.answers[0].eventPointDatas[0].perkType == PerksType.Network) firstAnswer.plusPointText.text = "N";
        else firstAnswer.plusPointText.text = "A";

        if (currentEventData.answers[0].eventPointDatas[1].perkType == PerksType.Drive) firstAnswer.minusPointText.text = "D";
        else if (currentEventData.answers[0].eventPointDatas[1].perkType == PerksType.Network) firstAnswer.minusPointText.text = "N";
        else firstAnswer.minusPointText.text = "A";

        //set second answer
        if (currentEventData.answers[1].eventPointDatas[0].perkType == PerksType.Drive) secondAnswer.plusPointText.text = "D";
        else if (currentEventData.answers[1].eventPointDatas[0].perkType == PerksType.Network) secondAnswer.plusPointText.text = "N";
        else secondAnswer.plusPointText.text = "A";

        if (currentEventData.answers[1].eventPointDatas[1].perkType == PerksType.Drive) secondAnswer.minusPointText.text = "D";
        else if (currentEventData.answers[1].eventPointDatas[1].perkType == PerksType.Network) secondAnswer.minusPointText.text = "N";
        else secondAnswer.minusPointText.text = "A";

        DataHandler.instance.GetUserSpecificPerksPoint().perks_story_type = 
            LevelDataHandler.instance.isPrologue ? UserDataSpace.StoryType.Prologue : UserDataSpace.StoryType.Epilogue;

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
        if (answerChosen == 0)
        {
            if (DataHandler.instance.GetSpecificPerksPoint(currentEventData.answers[0].eventPointDatas[0].perkType).perks_point_plus == 0)
            {
                DataHandler.instance.GetSpecificPerksPoint(currentEventData.answers[0].eventPointDatas[0].perkType).perks_point_plus += 2;
                DataHandler.instance.GetPerksData().perks_point_data.total_perks_point_plus +=
                    DataHandler.instance.GetSpecificPerksPoint(currentEventData.answers[0].eventPointDatas[0].perkType).perks_point_plus;

                DataHandler.instance.GetUserSpecificPerksPoint().perks_point_plus = DataHandler.instance.GetSpecificPerksPoint(currentEventData.answers[0].eventPointDatas[0].perkType).perks_point_plus;
                DataHandler.instance.GetUserSpecificPerksPoint().perks_plus_type = currentEventData.answers[0].eventPointDatas[0].perkType;
            }
            if (DataHandler.instance.GetSpecificPerksPoint(currentEventData.answers[0].eventPointDatas[1].perkType).perks_point_minus == 0)
            {
                DataHandler.instance.GetSpecificPerksPoint(currentEventData.answers[0].eventPointDatas[1].perkType).perks_point_minus += 2;
                DataHandler.instance.GetPerksData().perks_point_data.total_perks_point_minus +=
                    DataHandler.instance.GetSpecificPerksPoint(currentEventData.answers[0].eventPointDatas[1].perkType).perks_point_minus;

                DataHandler.instance.GetUserSpecificPerksPoint().perks_point_minus = DataHandler.instance.GetSpecificPerksPoint(currentEventData.answers[0].eventPointDatas[1].perkType).perks_point_minus;
                DataHandler.instance.GetUserSpecificPerksPoint().perks_minus_type = currentEventData.answers[0].eventPointDatas[1].perkType;
            }
        }
        else
        {
            if (DataHandler.instance.GetSpecificPerksPoint(currentEventData.answers[1].eventPointDatas[0].perkType).perks_point_plus == 0)
            {
                DataHandler.instance.GetSpecificPerksPoint(currentEventData.answers[1].eventPointDatas[0].perkType).perks_point_plus += 2;
                DataHandler.instance.GetPerksData().perks_point_data.total_perks_point_plus +=
                    DataHandler.instance.GetSpecificPerksPoint(currentEventData.answers[1].eventPointDatas[0].perkType).perks_point_plus;

                DataHandler.instance.GetUserSpecificPerksPoint().perks_point_plus = DataHandler.instance.GetSpecificPerksPoint(currentEventData.answers[1].eventPointDatas[0].perkType).perks_point_plus;
                DataHandler.instance.GetUserSpecificPerksPoint().perks_plus_type = currentEventData.answers[1].eventPointDatas[0].perkType;
            }
            if (DataHandler.instance.GetSpecificPerksPoint(currentEventData.answers[1].eventPointDatas[1].perkType).perks_point_minus == 0)
            {
                DataHandler.instance.GetSpecificPerksPoint(currentEventData.answers[1].eventPointDatas[1].perkType).perks_point_minus += 2;
                DataHandler.instance.GetPerksData().perks_point_data.total_perks_point_minus +=
                    DataHandler.instance.GetSpecificPerksPoint(currentEventData.answers[1].eventPointDatas[1].perkType).perks_point_minus;

                DataHandler.instance.GetUserSpecificPerksPoint().perks_point_minus = DataHandler.instance.GetSpecificPerksPoint(currentEventData.answers[1].eventPointDatas[1].perkType).perks_point_minus;
                DataHandler.instance.GetUserSpecificPerksPoint().perks_minus_type = currentEventData.answers[1].eventPointDatas[1].perkType;
            }
        }

        DataHandler.instance.GetUserSpecificPerksPoint().current_game_level = LevelDataHandler.instance.currentGameData.gameLevel;
        MainMenuHandler.instance.PatchPerksFromMenu(() =>
        {
            if (!currentEventData.autoOpenPerks)
            {
                if (LevelDataHandler.instance.isPrologue) LevelDataHandler.instance.SetPrologueStory(1);
                if (LevelDataHandler.instance.isEpilogue) LevelDataHandler.instance.SetEpilogueStory(1);
            }
            else
            {
                customPerkPanel.OpenPerksPanel(true);
            }

            StartCoroutine(SetDisableEventPanel());
            currentEventData = new();
        });
    }

    IEnumerator SetDisableEventPanel()
    {
        yield return new WaitUntil(() => customPerkPanel.GetPanelState());
        eventPanel.SetActive(false);
    }
}
