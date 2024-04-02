using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

[Serializable]
public class EventAnswerData
{
    [TextArea(4, 4)] public string answer;
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
    [TextArea(7, 7)] public string question;
    public Sprite characterSprite;
    public List<EventAnswerData> answers;
}

[Serializable]
public class EventAnswerUI
{
    public TextMeshProUGUI answerText;
    public TextMeshProUGUI minusPointText;
    public TextMeshProUGUI plusPointText;
    public Button button;
}

public class EventHandler : MonoBehaviour
{
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
        questionText.text = currentEventData.question;
        eventPanel.SetActive(true);

        firstAnswer.answerText.text = currentEventData.answers[0].answer;
        if (currentEventData.answers[0].eventPointDatas[0].perkType == PerksType.Drive) firstAnswer.plusPointText.text = "D";
        else if (currentEventData.answers[0].eventPointDatas[0].perkType == PerksType.Network) firstAnswer.plusPointText.text = "N";
        else firstAnswer.plusPointText.text = "A";

        if (currentEventData.answers[0].eventPointDatas[1].perkType == PerksType.Drive) firstAnswer.minusPointText.text = "D";
        else if (currentEventData.answers[0].eventPointDatas[1].perkType == PerksType.Network) firstAnswer.minusPointText.text = "N";
        else firstAnswer.minusPointText.text = "A";

        secondAnswer.answerText.text = currentEventData.answers[1].answer;
        if (currentEventData.answers[1].eventPointDatas[0].perkType == PerksType.Drive) secondAnswer.plusPointText.text = "D";
        else if (currentEventData.answers[1].eventPointDatas[0].perkType == PerksType.Network) secondAnswer.plusPointText.text = "N";
        else secondAnswer.plusPointText.text = "A";

        if (currentEventData.answers[1].eventPointDatas[1].perkType == PerksType.Drive) secondAnswer.minusPointText.text = "D";
        else if (currentEventData.answers[1].eventPointDatas[1].perkType == PerksType.Network) secondAnswer.minusPointText.text = "N";
        else secondAnswer.minusPointText.text = "A";

        DataHandler.instance.GetUserSpecificPerksPoint().perks_story_type = 
            LevelDataHandler.instance.isPrologue ? UserDataSpace.StoryType.Prologue : UserDataSpace.StoryType.Epilogue;
        playerCharImage.sprite = currentEventData.characterSprite;
    }

    public void ChooseFirstAnswer()
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

        DataHandler.instance.GetUserSpecificPerksPoint().current_game_level = LevelDataHandler.instance.currentGameData.gameLevel;
        MainMenuHandler.instance.PatchPerksFromMenu(() =>
        {
            eventPanel.SetActive(false);
            currentEventData = new();

            if (!currentEventData.autoOpenPerks)
            {
                if (LevelDataHandler.instance.isPrologue) LevelDataHandler.instance.SetPrologueStory(1);
                if (LevelDataHandler.instance.isEpilogue) LevelDataHandler.instance.SetEpilogueStory(1);
                return;
            }
            
            customPerkPanel.OpenPerksPanel(true);
        }); 
    }

    public void ChooseSecondAnswer()
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

        DataHandler.instance.GetUserSpecificPerksPoint().current_game_level = LevelDataHandler.instance.currentGameData.gameLevel;
        MainMenuHandler.instance.PatchPerksFromMenu(() =>
        {
            eventPanel.SetActive(false);
            currentEventData = new();

            if (!currentEventData.autoOpenPerks)
            {
                if (LevelDataHandler.instance.isPrologue) LevelDataHandler.instance.SetPrologueStory(1);
                if (LevelDataHandler.instance.isEpilogue) LevelDataHandler.instance.SetEpilogueStory(1);
                return;
            }

            customPerkPanel.OpenPerksPanel(true);
        });
    }
}
