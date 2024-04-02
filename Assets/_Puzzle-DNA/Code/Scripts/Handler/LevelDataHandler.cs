using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.Events;

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

    [Header("Current Story Attributes")]
    [HideInInspector] public GameData currentGameData;
    [HideInInspector] public LevelData currentLevelData;
    public StoryData currentStoryData;
    public List<StoryData> prologueStoryData;
    public List<StoryData> epilogueStoryData;

    [Header("Current Index Attributes")]
    [HideInInspector] public int prologueIndex;
    [HideInInspector] public int epilogueIndex;
    [HideInInspector] public int dialogueIndex;
    [HideInInspector] public int narrationIndex;
    [HideInInspector] public int popUpIndex;
    [HideInInspector] public int titleIndex;
    public bool isPrologue;
    public bool isEpilogue;

    [Header("Dialogue Attributes")]
    public GameObject dialoguePanel;
    public DialogueStoryUI playerDialogue;
    public DialogueStoryUI interlocutorDialogue;

    [Header("Narration Attributes")]
    public GameObject narrationPanel;
    public TextMeshProUGUI narrationText;
    public TextMeshProUGUI narrationTextAbove;
    public TextMeshProUGUI narrationTextMiddle;
    public TextMeshProUGUI narrationTextUnder;

    [Header("PopUp Attributes")]
    public GameObject popUpPanel;
    public TextMeshProUGUI popUpText;

    [Header("Title Attributes")]
    public GameObject titlePanel;
    public TextMeshProUGUI titleText;

    [Header("Event Attributes")]
    public EventHandler eventHandler;

    [Header("Tutorial Attributes")]
    public GameObject tutorialParentPanel;

    private void Awake()
    {
        instance = this;
    }

    public void InitAllData(LevelData levelData)
    {
        currentLevelData = levelData;
        currentGameData = levelData.gameData;
        prologueStoryData = levelData.prologueStoryData;
        epilogueStoryData = levelData.epilogueStoryData;
    }

    public void InitPrologue(LevelData levelData)
    {
        InitAllData(levelData);
        if (prologueStoryData.Count == 0) SetPrologueStory(true);
        if (epilogueStoryData.Count == 0) SetEpilogueStory(true);
        SetPrologueStory(0);
    }

    public void InitEpilogue(LevelData levelData)
    {
        InitAllData(levelData);
        if (prologueStoryData.Count == 0) SetPrologueStory(true);
        if (epilogueStoryData.Count == 0) SetEpilogueStory(true);
        SetEpilogueStory(0);
    }

    public void SetPrologueStory(int factor)
    {
        isPrologue = true;
        prologueIndex += factor;
        if (prologueIndex == prologueStoryData.Count)
        {
            prologueIndex = 0;
            isPrologue = false;
            
            if (epilogueStoryData.Count == 0) 
                storyPanel.SetActive(false);
            else
            {
                UnityEvent whenGameUnloaded = new();
                whenGameUnloaded.AddListener(() => SetEpilogueStory(0));
                CommonHandler.instance.whenSceneUnloadedCustom = whenGameUnloaded;
            }

            SetPrologueStory(true);
            GameGenerator.instance.GenerateLevel(currentGameData);
            return;
        }

        currentStoryData = prologueStoryData[prologueIndex];
        backgroundImage.sprite = currentStoryData.backgroundSprite;
        backgroundImage.gameObject.SetActive(true);
        storyPanel.SetActive(true);

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
            case StoryData.StoryType.Event:
                if (!DataHandler.instance.GetUserCheckpointData().
                    checkpoint_value[currentGameData.gameLevel].prologue_is_done)
                {
                    eventHandler.Init(currentStoryData.eventDataStory);
                }
                else
                {
                    SetPrologueStory(1);
                }
                break;
            case StoryData.StoryType.Tutorial:
                if (!DataHandler.instance.GetUserCheckpointData().
                    checkpoint_value[currentGameData.gameLevel].prologue_is_done)
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

    public void SetEpilogueStory(int factor)
    {
        isEpilogue = true;
        epilogueIndex += factor;
        if (epilogueIndex == epilogueStoryData.Count)
        {
            epilogueIndex = 0;
            isEpilogue = false;
            storyPanel.SetActive(false);
            SetEpilogueStory(true);
            return;
        }

        currentStoryData = epilogueStoryData[epilogueIndex];
        backgroundImage.sprite = currentStoryData.backgroundSprite;
        backgroundImage.gameObject.SetActive(true);
        storyPanel.SetActive(true);

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
            case StoryData.StoryType.Event:
                if (!DataHandler.instance.GetUserCheckpointData().
                    checkpoint_value[currentGameData.gameLevel].epilogue_is_done)
                {
                    eventHandler.Init(currentStoryData.eventDataStory);
                }
                else
                {
                    SetEpilogueStory(1);
                }
                break;
            case StoryData.StoryType.Tutorial:
                if (!DataHandler.instance.GetUserCheckpointData().
                    checkpoint_value[currentGameData.gameLevel].epilogue_is_done)
                {
                    storyPanel.SetActive(false);
                    tutorialParentPanel.SetActive(true);
                    SetTutorialStory(currentStoryData.tutorialKey);
                }
                else
                {
                    SetEpilogueStory(1);
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
            else if (isEpilogue) SetEpilogueStory(1);
            return;
        }

        #region Setting Content
        TextMeshProUGUI currContent;
        DialogueStoryData dialogue = currentStoryData.dialogueStory.dialogueStories[dialogueIndex];

        playerDialogue.charImage.sprite = dialogue.contentData.playerSprite;
        interlocutorDialogue.charImage.sprite = dialogue.contentData.interlocutorSprite;

        playerDialogue.nameText.text = DataHandler.instance.GetUserDataValue().username;
        interlocutorDialogue.nameText.text = currentStoryData.dialogueStory.interlocutorName;

        if (dialogue.playerIsTalking) currContent = playerDialogue.dialogueText;
        else currContent = interlocutorDialogue.dialogueText;

        if (DataHandler.instance.GetLanguage() == "id")
            SetStory(
                currContent,
                dialogue.contentData.clipId,
                dialogue.contentData.contentId
                );
        else if (DataHandler.instance.GetLanguage() == "en")
            SetStory(
                currContent,
                dialogue.contentData.clipEn,
                dialogue.contentData.contentEn
                );
        else
            SetStory(
                currContent,
                dialogue.contentData.clipMy,
                dialogue.contentData.contentMy
                );

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
            else if (isEpilogue) SetEpilogueStory(1);
            return;
        }

        narrationText = null;
        narrationTextAbove.text = narrationTextMiddle.text = narrationTextUnder.text = "";
        switch (currentStoryData.narrationType)
        {
            case StoryData.NarrationType.Above:
                narrationText = narrationTextAbove;
                break;
            case StoryData.NarrationType.Middle:
                narrationText = narrationTextMiddle;
                break;
            case StoryData.NarrationType.Under:
                narrationText = narrationTextUnder;
                break;
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
            else if (isEpilogue) SetEpilogueStory(1);
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
            else if (isEpilogue) SetEpilogueStory(1);
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

    public void SetPrologueStory(bool isDone)
    {
        DataHandler.instance.GetUserCheckpointData().
            checkpoint_value[currentGameData.gameLevel].prologue_is_done = isDone;

        DataHandler.instance.IEPatchCheckpointData(() => { });
    }

    public void SetEpilogueStory(bool isDone)
    {
        DataHandler.instance.GetUserCheckpointData().
            checkpoint_value[currentGameData.gameLevel].epilogue_is_done = isDone;

        DataHandler.instance.IEPatchCheckpointData(() => { });
    }
}
