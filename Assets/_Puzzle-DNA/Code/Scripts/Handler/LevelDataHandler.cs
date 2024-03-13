using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Linq;

[Serializable]
public class DialogueStoryUI
{
    public Image charImage;
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
}

public class LevelDataHandler : MonoBehaviour
{
    public static LevelDataHandler instance;

    [Header("General Story Attributes")]
    public GameObject storyPanel;
    public Image backgroundImage;

    [Header("Dialogue Attributes")]
    public GameObject dialoguePanel;
    public DialogueStoryUI playerDialogue;
    public DialogueStoryUI interlocutorDialogue;

    [Header("Narration Attributes")]
    public GameObject narrationPanel;
    public TextMeshProUGUI narrationText;

    [Header("PopUp Attributes")]
    public GameObject popUpPanel;
    public TextMeshProUGUI popUpText;

    [Header("Title Attributes")]
    public GameObject titlePanel;
    public TextMeshProUGUI titleText;

    [Header("Tutorial Attributes")]
    public GameObject tutorialParentPanel;

    int prologueIndex, epilogueIndex;
    int dialogueIndex, narrationIndex, popUpIndex, titleIndex;
    bool isPrologue, isEpilogue;

    [Header("Current Story Attributes")]
    [HideInInspector] public GameData currentGameData;
    [HideInInspector] public LevelData currentLevelData;
    [HideInInspector] public StoryData currentStoryData;
    [HideInInspector] public List<StoryData> prologueStoryData;
    [HideInInspector] public List<StoryData> epilogueStoryData;

    private void Awake()
    {
        instance = this;
    }

    public void Init(LevelData levelData)
    {
        currentLevelData = levelData;
        currentGameData = levelData.gameData;
        prologueStoryData = levelData.prologueStoryData;
        epilogueStoryData = levelData.epilogueStoryData;
        SetPrologueStory(0);
    }

    public void SetPrologueStory(int factor)
    {
        isPrologue = true;
        prologueIndex += factor;
        if (prologueIndex == prologueStoryData.Count)
        {
            prologueIndex = 0;
            isPrologue = false;
            if (epilogueStoryData.Count == 0) storyPanel.SetActive(false);
            GameGenerator.instance.GenerateLevel(currentGameData);
            return;
        }

        currentStoryData = prologueStoryData[prologueIndex];
        backgroundImage.gameObject.SetActive(false);
        storyPanel.SetActive(true);

        if (currentStoryData != null)
        {
            backgroundImage.sprite = currentStoryData.backgroundSprite;
            backgroundImage.gameObject.SetActive(true);
        }

        switch (currentStoryData.storyType)
        {
            case StoryData.StoryType.Dialogue:
                dialoguePanel.SetActive(true);
                SetDialogueStory(0);
                break;
            case StoryData.StoryType.Narration:
                narrationPanel.SetActive(true);
                SetNarrationStory(0);
                break;
            case StoryData.StoryType.PopUp:
                popUpPanel.SetActive(true);
                SetPopUpStory(0);
                break;
            case StoryData.StoryType.Title:
                titlePanel.SetActive(true);
                SetTitleStory(0);
                break;
            case StoryData.StoryType.Tutorial:
                storyPanel.SetActive(false);
                tutorialParentPanel.SetActive(true);
                SetTutorialStory(currentStoryData.tutorialKey);
                break;
        }
    }

    public void SetDialogueStory(int factor)
    {
        dialogueIndex += factor;
        if (dialogueIndex == currentStoryData.dialogueStory.dialogueStories.Count)
        {
            dialogueIndex = 0;
            dialoguePanel.SetActive(false);
            
            if (isPrologue) SetPrologueStory(1);
            return;
        } 

        DialogueStoryData dialogue = currentStoryData.dialogueStory.dialogueStories[dialogueIndex];
        if (dialogue.playerIsTalking)
        {
            playerDialogue.charImage.sprite = currentStoryData.dialogueStory.playerSprite;
            playerDialogue.nameText.text = DataHandler.instance.GetUserDataValue().username;

            if (DataHandler.instance.GetLanguage() == "id")
                playerDialogue.dialogueText.text = dialogue.contentData.contentId;
            else
                playerDialogue.dialogueText.text = dialogue.contentData.contentEn;
        }
        else
        {
            interlocutorDialogue.charImage.sprite = currentStoryData.dialogueStory.interlocutorSprite;
            interlocutorDialogue.nameText.text = currentStoryData.dialogueStory.interlocutorName;

            if (DataHandler.instance.GetLanguage() == "id")
                playerDialogue.dialogueText.text = dialogue.contentData.contentId;
            else
                playerDialogue.dialogueText.text = dialogue.contentData.contentEn;
        }

        interlocutorDialogue.dialoguePanel.SetActive(!dialogue.playerIsTalking);
        playerDialogue.dialoguePanel.SetActive(dialogue.playerIsTalking);
    }

    public void SetNarrationStory(int factor)
    {
        narrationIndex += factor;
        if (narrationIndex == currentStoryData.narrationStories.Count)
        {
            narrationIndex = 0;
            narrationPanel.SetActive(false);

            if (isPrologue) SetPrologueStory(1);
            return;
        }

        if (DataHandler.instance.GetLanguage() == "id")
            narrationText.text = currentStoryData.narrationStories[narrationIndex].contentId;
        else
            narrationText.text = currentStoryData.narrationStories[narrationIndex].contentEn;
    }

    public void SetPopUpStory(int factor)
    {
        popUpIndex += factor;
        if (popUpIndex == currentStoryData.popUpStories.Count)
        {
            popUpIndex = 0;
            popUpPanel.SetActive(false);

            if (isPrologue) SetPrologueStory(1);
            return;
        }

        if (DataHandler.instance.GetLanguage() == "id")
            popUpText.text = currentStoryData.popUpStories[popUpIndex].contentId;
        else
            popUpText.text = currentStoryData.popUpStories[popUpIndex].contentEn;
    }

    public void SetTitleStory(int factor)
    {
        titleIndex += factor;
        if (titleIndex == currentStoryData.titleStories.Count)
        {
            titleIndex = 0;
            titlePanel.SetActive(false);

            if (isPrologue) SetPrologueStory(1);
            return;
        }

        if (DataHandler.instance.GetLanguage() == "id")
            titleText.text = currentStoryData.titleStories[titleIndex].contentId;
        else
            titleText.text = currentStoryData.titleStories[titleIndex].contentEn;
    }

    public void SetTutorialStory(string key)
    {
        FindObjectsOfType<SequencePanelHandler>().ToList().
            Find(seq => seq.key == key).Init();
    }
}
