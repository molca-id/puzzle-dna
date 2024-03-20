using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

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
                if (!DataHandler.instance.GetUserCheckpointData().
                    checkpoint_value[currentGameData.gameLevel].is_opened)
                {
                    storyPanel.SetActive(false);
                    tutorialParentPanel.SetActive(true);
                    SetTutorialStory(currentStoryData.tutorialKey);
                }
                else
                {
                    SetPrologueStory(1);
                }
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

        #region Setting Content
        DialogueStoryData dialogue = currentStoryData.dialogueStory.dialogueStories[dialogueIndex];
        if (dialogue.playerIsTalking)
        {
            playerDialogue.charImage.sprite = currentStoryData.dialogueStory.playerSprite;
            playerDialogue.nameText.text = DataHandler.instance.GetUserDataValue().username;

            if (DataHandler.instance.GetLanguage() == "id")
                SetStory(
                    playerDialogue.dialogueText, 
                    dialogue.contentData.clipId, 
                    dialogue.contentData.contentId
                    );
            else if (DataHandler.instance.GetLanguage() == "en")
                SetStory(
                    playerDialogue.dialogueText,
                    dialogue.contentData.clipEn,
                    dialogue.contentData.contentEn
                    );
            else
                SetStory(
                    playerDialogue.dialogueText,
                    dialogue.contentData.clipMy,
                    dialogue.contentData.contentMy
                    );
        }
        else
        {
            interlocutorDialogue.charImage.sprite = currentStoryData.dialogueStory.interlocutorSprite;
            interlocutorDialogue.nameText.text = currentStoryData.dialogueStory.interlocutorName;

            if (DataHandler.instance.GetLanguage() == "id")
                SetStory(
                    interlocutorDialogue.dialogueText,
                    dialogue.contentData.clipId,
                    dialogue.contentData.contentId
                    );
            else if (DataHandler.instance.GetLanguage() == "en")
                SetStory(
                    interlocutorDialogue.dialogueText,
                    dialogue.contentData.clipEn,
                    dialogue.contentData.contentEn
                    );
            else
                SetStory(
                    interlocutorDialogue.dialogueText,
                    dialogue.contentData.clipMy,
                    dialogue.contentData.contentMy
                    );
        }

        interlocutorDialogue.dialoguePanel.SetActive(!dialogue.playerIsTalking);
        playerDialogue.dialoguePanel.SetActive(dialogue.playerIsTalking);
        #endregion
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

        #region Setting Content
        if (DataHandler.instance.GetLanguage() == "id")
            SetStory(
                narrationText,
                currentStoryData.narrationStories[narrationIndex].clipId,
                currentStoryData.narrationStories[narrationIndex].contentId
                );
        else if (DataHandler.instance.GetLanguage() == "en")
            SetStory(
                narrationText,
                currentStoryData.narrationStories[narrationIndex].clipEn,
                currentStoryData.narrationStories[narrationIndex].contentEn
                );
        else
            SetStory(
                narrationText,
                currentStoryData.narrationStories[narrationIndex].clipMy,
                currentStoryData.narrationStories[narrationIndex].contentMy
                );
        #endregion
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

        #region Setting Content
        if (DataHandler.instance.GetLanguage() == "id")
            SetStory(
                popUpText,
                currentStoryData.popUpStories[narrationIndex].clipId,
                currentStoryData.popUpStories[narrationIndex].contentId
                );
        else if (DataHandler.instance.GetLanguage() == "en")
            SetStory(
                popUpText,
                currentStoryData.popUpStories[narrationIndex].clipEn,
                currentStoryData.popUpStories[narrationIndex].contentEn
                );
        else
            SetStory(
                popUpText,
                currentStoryData.popUpStories[narrationIndex].clipMy,
                currentStoryData.popUpStories[narrationIndex].contentMy
                );
        #endregion
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

        #region Setting Content
        if (DataHandler.instance.GetLanguage() == "id")
            SetStory(
                titleText,
                currentStoryData.titleStories[narrationIndex].clipId,
                currentStoryData.titleStories[narrationIndex].contentId
                );
        else if (DataHandler.instance.GetLanguage() == "en")
            SetStory(
                titleText,
                currentStoryData.titleStories[narrationIndex].clipEn,
                currentStoryData.titleStories[narrationIndex].contentEn
                );
        else
            SetStory(
                titleText,
                currentStoryData.titleStories[narrationIndex].clipMy,
                currentStoryData.titleStories[narrationIndex].contentMy
                );
        #endregion
    }

    public void SetTutorialStory(string key)
    {
        FindObjectsOfType<SequencePanelHandler>().ToList().
            Find(seq => seq.key == key).Init();
    }

    public void SetStory(TextMeshProUGUI text, AudioClip clip, string textValue)
    {
        AudioSource voAudioSource = MainMenuHandler.instance.GetVOSource();
        voAudioSource.Stop();
        text.text = textValue;
        
        if (clip == null) return;
        voAudioSource.clip = clip;
        voAudioSource.Play();
    }
}
