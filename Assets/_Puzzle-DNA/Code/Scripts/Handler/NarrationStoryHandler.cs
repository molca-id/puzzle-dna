using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class NarrationStoryData
{
    public string levelIndex;
    public List<StoryData> narrationStoryDatas;
}

public class NarrationStoryHandler : MonoBehaviour
{
    public static NarrationStoryHandler instance;
    public List<NarrationStoryData> storyDatas;

    [Header("Current Index Attributes")]
    public StoryData currentStoryData;
    public int lastCheckpoint;
    public int nextLevelIndex;
    public bool isLastStory;
    [HideInInspector] public int storyIndex;
    [HideInInspector] public int subStoryIndex;
    [HideInInspector] public int dialogueIndex;
    [HideInInspector] public int narrationIndex;
    [HideInInspector] public int popUpIndex;
    [HideInInspector] public int titleIndex;

    [Header("General Story Attributes")]
    public GameObject storyPanel;
    public Image backgroundImage;
    public Button prevButton;
    public Button nextButton;

    [Header("Dialogue Attributes")]
    public GameObject dialoguePanel;
    public DialogueStoryUI playerDialogue;
    public DialogueStoryUI interlocutorDialogue;

    [Header("Narration Attributes")]
    public GameObject narrationPanel;
    public GameObject narrationPanelWithBG;
    public Image narrationCharImage;
    public TextMeshProUGUI narrationText;
    public TextMeshProUGUI narrationTextAbove;
    public TextMeshProUGUI narrationTextMiddle;
    public TextMeshProUGUI narrationTextUnder;
    public TextMeshProUGUI narrationTextWithBG;

    [Header("PopUp Attributes")]
    public GameObject popUpPanel;
    public TextMeshProUGUI popUpText;

    [Header("Title Attributes")]
    public GameObject titlePanel;
    public TextMeshProUGUI titleText;

    private void Awake()
    {
        instance = this;
    }

    public void InitStoryFirst()
    {
        storyIndex = subStoryIndex = 0;
        dialogueIndex = narrationIndex = popUpIndex = titleIndex = 0;
        InitCurrentStory();
    }

    public void InitCurrentStory()
    {
        storyPanel.SetActive(true);
        dialoguePanel.SetActive(false);
        narrationPanel.SetActive(false);
        popUpPanel.SetActive(false);
        titlePanel.SetActive(false);

        currentStoryData = storyDatas[storyIndex].narrationStoryDatas[subStoryIndex];
        backgroundImage.sprite = currentStoryData.backgroundSprite;
        for (int i = 0; i < storyDatas.Count; i++)
        {
            if (storyDatas[i].narrationStoryDatas.Find(item => item == currentStoryData) != null)
            {
                nextLevelIndex = Convert.ToInt32(storyDatas[i + 1].levelIndex);
                break;
            }
        }

        if (currentStoryData.backgroundSprite == null) backgroundImage.gameObject.SetActive(false);
        else backgroundImage.gameObject.SetActive(true);

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
        }

        SetButton();
    }

    public void SetButton()
    {
        isLastStory = false;
        prevButton.interactable = true;
        nextButton.interactable = true;
        switch (currentStoryData.storyType)
        {
            case StoryData.StoryType.Dialogue:
                if (dialogueIndex == currentStoryData.dialogueStory.dialogueStories.Count - 1) 
                    isLastStory = true;
                break;
            case StoryData.StoryType.Narration:
                if (narrationIndex == currentStoryData.narrationStories.Count - 1) isLastStory = true;
                break;
            case StoryData.StoryType.PopUp:
                if (popUpIndex == currentStoryData.popUpStories.Count - 1) 
                    isLastStory = true;
                break;
            case StoryData.StoryType.Title:
                if (titleIndex == currentStoryData.titleStories.Count - 1) 
                    isLastStory = true;
                break;
        }

        // prev button in the first story
        if (storyIndex == 0 && subStoryIndex == 0)
            prevButton.interactable = false;

        // next button in the checkpoint
        for (int i = 0; i < DataHandler.instance.GetUserCheckpointData().checkpoint_value.Count; i++)
        {
            if (DataHandler.instance.GetUserCheckpointData().checkpoint_value[i].checkpoint_level_score == 0)
            {
                lastCheckpoint = i - 1;
                break;
            }
        }
        if ((storyIndex == lastCheckpoint ||
            nextLevelIndex > lastCheckpoint) && 
            isLastStory) 
            nextButton.interactable = false;
    }

    public void SetPrevStory()
    {
        if (currentStoryData != null)
        {
            switch (currentStoryData.storyType)
            {
                case StoryData.StoryType.Dialogue:
                    SetDialogueStory(-1);
                    break;
                case StoryData.StoryType.Narration:
                    SetNarrationStory(-1);
                    break;
                case StoryData.StoryType.PopUp:
                    SetPopUpStory(-1);
                    break;
                case StoryData.StoryType.Title:
                    SetTitleStory(-1);
                    break;
            }

            SetButton();
            return;
        }
        else
        {
            if (subStoryIndex == 0)
            {
                if (storyIndex > 0)
                {
                    subStoryIndex = 0;
                    storyIndex--;
                }
            }
            else
            {
                subStoryIndex--;
            }
        }

        InitCurrentStory();
    }

    public void SetNextStory()
    {
        if (currentStoryData != null)
        {
            switch (currentStoryData.storyType)
            {
                case StoryData.StoryType.Dialogue:
                    SetDialogueStory(1);
                    break;
                case StoryData.StoryType.Narration:
                    SetNarrationStory(1);
                    break;
                case StoryData.StoryType.PopUp:
                    SetPopUpStory(1);
                    break;
                case StoryData.StoryType.Title:
                    SetTitleStory(1);
                    break;
            }

            SetButton();
            return;
        }
        else
        {
            if (subStoryIndex == storyDatas[storyIndex].narrationStoryDatas.Count - 1)
            {
                subStoryIndex = 0;
                storyIndex++;
            }
            else
            {
                subStoryIndex++;
            }
        }

        InitCurrentStory();
    }

    public void SetDialogueStory(int factor)
    {
        dialogueIndex += factor;
        if (dialogueIndex < 0)
        {
            dialogueIndex = 0;
            dialoguePanel.SetActive(false);
            currentStoryData = null;
            SetPrevStory();
            return;
        }
        if (dialogueIndex == currentStoryData.dialogueStory.dialogueStories.Count)
        {
            dialogueIndex = 0;
            dialoguePanel.SetActive(false);
            currentStoryData = null;
            SetNextStory();
            return;
        }

        #region Setting Content
        TextMeshProUGUI currContent;
        DialogueStoryData dialogue = currentStoryData.dialogueStory.dialogueStories[dialogueIndex];

        playerDialogue.charImage.sprite = DataHandler.instance.GetPlayerSprite(dialogue.contentData.playerExpression);
        interlocutorDialogue.charImage.sprite = dialogue.contentData.interlocutorSprite;

        playerDialogue.nameText.text = DataHandler.instance.GetUserDataValue().username;
        interlocutorDialogue.nameText.text = currentStoryData.dialogueStory.interlocutorName;

        if (dialogue.playerIsTalking) currContent = playerDialogue.dialogueText;
        else currContent = interlocutorDialogue.dialogueText;

        dialogue.contentData.contentId = dialogue.contentData.contentId.
            Replace("(playerName)", DataHandler.instance.GetUserDataValue().username);
        dialogue.contentData.contentMy = dialogue.contentData.contentMy.
            Replace("(playerName)", DataHandler.instance.GetUserDataValue().username);
        dialogue.contentData.contentEn = dialogue.contentData.contentEn.
            Replace("(playerName)", DataHandler.instance.GetUserDataValue().username);

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
        if (narrationIndex < 0)
        {
            narrationIndex = 0;
            narrationPanel.SetActive(false);
            currentStoryData = null;
            SetPrevStory();
            return;
        }
        if (narrationIndex == currentStoryData.narrationStories.Count)
        {
            narrationIndex = 0;
            narrationPanel.SetActive(false);
            currentStoryData = null;
            SetNextStory();
            return;
        }

        #region Setting Content
        narrationText = null;
        narrationPanelWithBG.SetActive(false);
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
            case StoryData.NarrationType.WithBG:
                narrationText = narrationTextWithBG;
                narrationPanelWithBG.SetActive(true);
                break;
        }

        if (currentStoryData.narrationStories[narrationIndex].interlocutorSprite != null)
        {
            narrationCharImage.sprite = currentStoryData.narrationStories[narrationIndex].interlocutorSprite;
            narrationCharImage.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            narrationCharImage.sprite = null;
            narrationCharImage.transform.parent.gameObject.SetActive(false);
        }

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
        if (popUpIndex < 0)
        {
            popUpIndex = 0;
            popUpPanel.SetActive(false);
            currentStoryData = null;
            SetPrevStory();
            return;
        }
        if (popUpIndex == currentStoryData.popUpStories.Count)
        {
            popUpIndex = 0;
            popUpPanel.SetActive(false);
            currentStoryData = null;
            SetNextStory();
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
        if (titleIndex < 0)
        {
            titleIndex = 0;
            titlePanel.SetActive(false);
            currentStoryData = null;
            SetPrevStory();
            return;
        }
        if (titleIndex == currentStoryData.titleStories.Count)
        {
            titleIndex = 0;
            titlePanel.SetActive(false);
            currentStoryData = null;
            SetNextStory();
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