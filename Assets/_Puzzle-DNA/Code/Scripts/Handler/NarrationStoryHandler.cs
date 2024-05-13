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
    public GameObject currAnimation;
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
                DataHandler.instance.GetPlayerClip(dialogue.contentData.clipCodeId),
                dialogue.contentData.bgmClip,
                dialogue.contentData.clipId,
                dialogue.contentData.animPrefab,
                dialogue.contentData.contentId
                );
        else if (DataHandler.instance.GetLanguage() == "en")
            SetStory(
                currContent,
                DataHandler.instance.GetPlayerClip(dialogue.contentData.clipCodeEn),
                dialogue.contentData.bgmClip,
                dialogue.contentData.clipEn,
                dialogue.contentData.animPrefab,
                dialogue.contentData.contentEn
                );
        else
            SetStory(
                currContent,
                DataHandler.instance.GetPlayerClip(dialogue.contentData.clipCodeMy),
                dialogue.contentData.bgmClip,
                dialogue.contentData.clipMy,
                dialogue.contentData.animPrefab,
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
        narrationTextAbove.transform.parent.gameObject.SetActive(false);
        narrationTextMiddle.transform.parent.gameObject.SetActive(false);
        narrationTextUnder.transform.parent.gameObject.SetActive(false);
        narrationPanelWithBG.SetActive(false);

        narrationTextAbove.text = narrationTextMiddle.text = narrationTextUnder.text = "";
        switch (currentStoryData.narrationType)
        {
            case StoryData.NarrationType.Above:
                narrationText = narrationTextAbove;
                narrationText.transform.parent.gameObject.SetActive(true);
                break;
            case StoryData.NarrationType.Middle:
                narrationText = narrationTextMiddle;
                narrationText.transform.parent.gameObject.SetActive(true);
                break;
            case StoryData.NarrationType.Under:
                narrationText = narrationTextUnder;
                narrationText.transform.parent.gameObject.SetActive(true);
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
                DataHandler.instance.GetPlayerClip(currentStoryData.narrationStories[narrationIndex].clipCodeId),
                currentStoryData.narrationStories[narrationIndex].bgmClip,
                currentStoryData.narrationStories[narrationIndex].clipId,
                currentStoryData.narrationStories[narrationIndex].animPrefab,
                currentStoryData.narrationStories[narrationIndex].contentId
                );
        else if (DataHandler.instance.GetLanguage() == "en")
            SetStory(
                narrationText,
                DataHandler.instance.GetPlayerClip(currentStoryData.narrationStories[narrationIndex].clipCodeEn),
                currentStoryData.narrationStories[narrationIndex].bgmClip,
                currentStoryData.narrationStories[narrationIndex].clipEn,
                currentStoryData.narrationStories[narrationIndex].animPrefab,
                currentStoryData.narrationStories[narrationIndex].contentEn
                );
        else
            SetStory(
                narrationText,
                DataHandler.instance.GetPlayerClip(currentStoryData.narrationStories[narrationIndex].clipCodeMy),
                currentStoryData.narrationStories[narrationIndex].bgmClip,
                currentStoryData.narrationStories[narrationIndex].clipMy,
                currentStoryData.narrationStories[narrationIndex].animPrefab,
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
                DataHandler.instance.GetPlayerClip(currentStoryData.popUpStories[popUpIndex].clipCodeId),
                currentStoryData.popUpStories[popUpIndex].bgmClip,
                currentStoryData.popUpStories[popUpIndex].clipId,
                currentStoryData.popUpStories[popUpIndex].animPrefab,
                currentStoryData.popUpStories[popUpIndex].contentId
                );
        else if (DataHandler.instance.GetLanguage() == "en")
            SetStory(
                popUpText,
                DataHandler.instance.GetPlayerClip(currentStoryData.popUpStories[popUpIndex].clipCodeEn),
                currentStoryData.popUpStories[popUpIndex].bgmClip,
                currentStoryData.popUpStories[popUpIndex].clipEn,
                currentStoryData.popUpStories[popUpIndex].animPrefab,
                currentStoryData.popUpStories[popUpIndex].contentEn
                );
        else
            SetStory(
                popUpText,
                DataHandler.instance.GetPlayerClip(currentStoryData.popUpStories[popUpIndex].clipCodeMy),
                currentStoryData.popUpStories[popUpIndex].bgmClip,
                currentStoryData.popUpStories[popUpIndex].clipMy,
                currentStoryData.popUpStories[popUpIndex].animPrefab,
                currentStoryData.popUpStories[popUpIndex].contentMy
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
                DataHandler.instance.GetPlayerClip(currentStoryData.titleStories[titleIndex].clipCodeId),
                currentStoryData.titleStories[titleIndex].bgmClip,
                currentStoryData.titleStories[titleIndex].clipId,
                currentStoryData.titleStories[titleIndex].animPrefab,
                currentStoryData.titleStories[titleIndex].contentId
                );
        else if (DataHandler.instance.GetLanguage() == "en")
            SetStory(
                titleText,
                DataHandler.instance.GetPlayerClip(currentStoryData.titleStories[titleIndex].clipCodeEn),
                currentStoryData.titleStories[titleIndex].bgmClip,
                currentStoryData.titleStories[titleIndex].clipEn,
                currentStoryData.titleStories[titleIndex].animPrefab,
                currentStoryData.titleStories[titleIndex].contentEn
                );
        else
            SetStory(
                titleText,
                DataHandler.instance.GetPlayerClip(currentStoryData.titleStories[titleIndex].clipCodeMy),
                currentStoryData.titleStories[titleIndex].bgmClip,
                currentStoryData.titleStories[titleIndex].clipMy,
                currentStoryData.titleStories[titleIndex].animPrefab,
                currentStoryData.titleStories[titleIndex].contentMy
                );
        #endregion
    }

    public void SetStory(TextMeshProUGUI text, AudioClip playerClip, AudioClip storyClip, AudioClip clip, GameObject animPrefab, string textValue)
    {
        AudioSource voAudioSource = MainMenuHandler.instance.GetVOSource();
        AudioSource storyAudioSource = MainMenuHandler.instance.GetStorySource();

        if (currAnimation != null)
            Destroy(currAnimation);

        if (animPrefab != null)
        {
            currAnimation = Instantiate(animPrefab);
            RectTransform rectTransform = currAnimation.GetComponent<RectTransform>();

            currAnimation.transform.SetParent(storyPanel.transform.GetChild(0));
            currAnimation.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            currAnimation.transform.localScale = Vector3.one;
            currAnimation.transform.SetSiblingIndex(2);

            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.anchorMin = new Vector2(0f, 0f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.pivot = Vector2.one * 0.5f;
        }

        voAudioSource.Stop();
        storyAudioSource.Stop();
        text.text = textValue;

        if (storyClip != null)
        {
            storyAudioSource.clip = storyClip;
            storyAudioSource.Play();
        }

        if (playerClip != null)
        {
            voAudioSource.clip = clip;
            voAudioSource.Play();
        }

        if (clip != null)
        {
            voAudioSource.clip = clip;
            voAudioSource.Play();
        }
    }
}
