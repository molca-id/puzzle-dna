using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

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
    [Header("General Story Attributes")]
    public GameObject storyPanel;
    public Image backgroundImage;

    [Header("Current Story Attributes")]
    public GameData gameData;
    public StoryData currentStoryData;
    public List<StoryData> prologueStoryData;
    public List<StoryData> epilogueStoryData;

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

    [HideInInspector] public int prologueIndex;
    [HideInInspector] public int epilogueIndex;

    [HideInInspector] public int dialogueIndex;
    [HideInInspector] public int narrationIndex;
    [HideInInspector] public int popUpIndex;

    [HideInInspector] public bool isPrologue;
    [HideInInspector] public bool isEpilogue;

    public void Init(LevelData levelData)
    {
        gameData = levelData.gameData;
        prologueStoryData = levelData.prologueStoryData;
        epilogueStoryData = levelData.epilogueStoryData;

        if (prologueStoryData.Count == 0)
            GameGenerator.instance.GenerateLevel(gameData);
        else
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
            GameGenerator.instance.GenerateLevel(gameData);
            return;
        }

        currentStoryData = prologueStoryData[prologueIndex];
        backgroundImage.gameObject.SetActive(false);

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
        }

        storyPanel.SetActive(true);
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
            playerDialogue.dialogueText.text = dialogue.dialogueContent;
        }
        else
        {
            interlocutorDialogue.charImage.sprite = currentStoryData.dialogueStory.interlocutorSprite;
            interlocutorDialogue.nameText.text = currentStoryData.dialogueStory.interlocutorName;
            interlocutorDialogue.dialogueText.text = dialogue.dialogueContent;
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

        string text = currentStoryData.narrationStories[narrationIndex];
        narrationText.text = text;
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

        string text = currentStoryData.popUpStories[popUpIndex];
        popUpText.text = text;
    }
}
