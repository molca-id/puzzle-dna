using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections;

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
    public bool isSkippable;
    public int delayTime;
    public int splashSpeed;
    public GameObject storyPanel;
    public GameObject currAnimation;
    public Image backgroundImage;

    [Header("Current Story Attributes")]
    [HideInInspector] public GameData currentGameData;
    [HideInInspector] public LevelData currentLevelData;
    [HideInInspector] public StoryData currentStoryData;
    [HideInInspector] public List<StoryData> prologueStoryData;
    [HideInInspector] public List<StoryData> epilogueStoryData;

    [Header("Current Index Attributes")]
    [HideInInspector] public int prologueIndex;
    [HideInInspector] public int epilogueIndex;
    [HideInInspector] public int dialogueIndex;
    [HideInInspector] public int narrationIndex;
    [HideInInspector] public int popUpIndex;
    [HideInInspector] public int titleIndex;
    [HideInInspector] public bool isPrologue;
    [HideInInspector] public bool isEpilogue;

    [Header("Dialogue Attributes")]
    public GameObject dialoguePanel;
    public DialogueStoryUI playerDialogue;
    public DialogueStoryUI interlocutorDialogue;

    [Header("Narration Attributes")]
    public GameObject narrationPanel;
    public GameObject narrationPanelWithBG;
    public Image narrationPlayerCharImage;
    public Image narrationInterlocutorCharImage;
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

        if (levelData.prologueStoryData.Count != 0)
            currentStoryData = levelData.prologueStoryData[0];
        else if (levelData.epilogueStoryData.Count != 0)
            currentStoryData = levelData.epilogueStoryData[0];
    }

    public void InitPrologue(LevelData levelData)
    {
        AudioSource source = MainMenuHandler.instance.GetBGMSource();
        if (levelData.bgmAudioClip != null)
        {
            source.clip = levelData.bgmAudioClip;
            source.Play();
        }

        InitAllData(levelData);
        if (prologueStoryData.Count == 0) SetPrologueStory(true);
        if (epilogueStoryData.Count == 0) SetEpilogueStory(true);
        SetPrologueStory(0);
    }

    public void InitEpilogue(LevelData levelData)
    {
        AudioSource source = MainMenuHandler.instance.GetBGMSource();
        if (levelData.bgmAudioClip != null &&
            levelData.bgmAudioClip != source.clip)
        {
            source.clip = levelData.bgmAudioClip;
            source.Play();
        }

        InitAllData(levelData);
        if (prologueStoryData.Count == 0) SetPrologueStory(true);
        if (epilogueStoryData.Count == 0) SetEpilogueStory(true);
        SetEpilogueStory(0);
    }

    public void SetPrologueStory(int factor)
    {
        isPrologue = true;
        prologueIndex += factor;
        StartCoroutine(IECloseScreen(tutorialParentPanel.GetComponent<CanvasGroup>(), () => tutorialParentPanel.SetActive(false)));
        if (prologueIndex == prologueStoryData.Count)
        {
            prologueIndex = 0;
            isPrologue = false;

            UnityEvent whenGameUnloaded = new();
            whenGameUnloaded.AddListener(() => SetEpilogueStory(0));
            CommonHandler.instance.whenSceneUnloadedCustom = whenGameUnloaded;
            GameGenerator.instance.GenerateLevel(currentGameData);
            SetPrologueStory(true);
            return;
        }

        currentStoryData = prologueStoryData[prologueIndex];
        SetStoryData();
    }

    public void SetEpilogueStory(int factor)
    {
        isEpilogue = true;
        epilogueIndex += factor;
        StartCoroutine(IECloseScreen(tutorialParentPanel.GetComponent<CanvasGroup>(), () => tutorialParentPanel.SetActive(false)));
        if (epilogueIndex == epilogueStoryData.Count)
        {
            epilogueIndex = 0;
            isEpilogue = false;
            storyPanel.SetActive(false);
            SetEpilogueStory(true);

            MainMenuHandler.instance.ResetBGMMenu();
            MainMenuHandler.instance.GetStorySource().Stop();
            MainMenuHandler.instance.GetVOSource().Stop();

            if (currentLevelData.showResultPanel)
                FinishHandler.instance.CalculateFinalResult();
            if (currentLevelData.openPerksPanelAfterEpilogue &&
                (DataHandler.instance.GetPerksData().perks_point_data.perks_point_plus > 0 ||
                DataHandler.instance.GetPerksData().perks_point_data.perks_point_minus > 0))
            {
                MainMenuHandler.instance.commonPerksHandler.SetAfterGame(true);
                MainMenuHandler.instance.commonPerksHandler.OpenPerksPanel(true);
            }
            return;
        }

        currentStoryData = epilogueStoryData[epilogueIndex];
        SetStoryData();
    }

    public void SetStoryData()
    {
        backgroundImage.sprite = currentStoryData.backgroundSprite;
        if (currentStoryData.backgroundSprite == null) backgroundImage.gameObject.SetActive(false);
        else backgroundImage.gameObject.SetActive(true);
        storyPanel.SetActive(true);

        switch (currentStoryData.storyType)
        {
            case StoryData.StoryType.Dialogue:
                dialoguePanel.SetActive(true);
                StartCoroutine(IEOpenScreen(dialoguePanel.GetComponent<CanvasGroup>()));
                SetDialogueStory(0);
                break;
            case StoryData.StoryType.Narration:
                narrationPanel.SetActive(true);
                StartCoroutine(IEOpenScreen(narrationPanel.GetComponent<CanvasGroup>()));
                SetNarrationStory(0);
                break;
            case StoryData.StoryType.PopUp:
                popUpPanel.SetActive(true);
                StartCoroutine(IEOpenScreen(popUpPanel.GetComponent<CanvasGroup>()));
                SetPopUpStory(0);
                break;
            case StoryData.StoryType.Title:
                titlePanel.SetActive(true);
                StartCoroutine(IEOpenScreen(titlePanel.GetComponent<CanvasGroup>()));
                SetTitleStory(0);
                break;
            case StoryData.StoryType.Event:
                if (((isPrologue && !DataHandler.instance.GetUserCheckpointData().
                    checkpoint_value[currentGameData.gameLevel].prologue_is_done) ||
                    (isEpilogue && !DataHandler.instance.GetUserCheckpointData().
                    checkpoint_value[currentGameData.gameLevel].epilogue_is_done)) &&
                    DataHandler.instance.GetUserSpecificPerksPoint().perks_point_plus == 0 &&
                    DataHandler.instance.GetUserSpecificPerksPoint().perks_point_minus == 0)
                {
                    eventHandler.Init(currentStoryData.eventDataStory);
                }
                else
                {
                    if (isPrologue) SetPrologueStory(1);
                    else if (isEpilogue) SetEpilogueStory(1);
                }
                break;
            case StoryData.StoryType.Tutorial:
                if ((isPrologue && !DataHandler.instance.GetUserCheckpointData().
                    checkpoint_value[currentGameData.gameLevel].prologue_is_done) ||
                    (isEpilogue && !DataHandler.instance.GetUserCheckpointData().
                    checkpoint_value[currentGameData.gameLevel].epilogue_is_done))
                {
                    tutorialParentPanel.SetActive(true);
                    StartCoroutine(IEOpenScreen(tutorialParentPanel.GetComponent<CanvasGroup>()));
                    SetTutorialStory(currentStoryData.tutorialKey);
                }
                else
                {
                    if (isPrologue) SetPrologueStory(1);
                    else if (isEpilogue) SetEpilogueStory(1);
                }
                break;
        }
    }

    public void SetDialogueStory(int factor)
    {
        if (!isSkippable) return;
        dialogueIndex += factor;
        if (dialogueIndex == currentStoryData.dialogueStory.dialogueStories.Count)
        {
            dialogueIndex = 0;
            StartCoroutine(IECloseScreen(dialoguePanel.GetComponent<CanvasGroup>(), () =>
            {
                dialoguePanel.SetActive(false);
            }));

            if (isPrologue) SetPrologueStory(1);
            else if (isEpilogue) SetEpilogueStory(1);
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
                dialogue.contentData.clipCodeId,
                dialogue.contentData.bgmClip,
                dialogue.contentData.clipId,
                dialogue.contentData.animPrefab,
                dialogue.contentData.contentId
                );
        else if (DataHandler.instance.GetLanguage() == "en")
            SetStory(
                currContent,
                dialogue.contentData.clipCodeEn,
                dialogue.contentData.bgmClip,
                dialogue.contentData.clipEn,
                dialogue.contentData.animPrefab,
                dialogue.contentData.contentEn
                );
        else
            SetStory(
                currContent,
                dialogue.contentData.clipCodeMy,
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
        if (!isSkippable) return;
        narrationIndex += factor;
        if (narrationIndex == currentStoryData.narrationStories.Count)
        {
            narrationIndex = 0;
            StartCoroutine(IECloseScreen(narrationPanel.GetComponent<CanvasGroup>(), () =>
            {
                narrationPanel.SetActive(false);
            }));

            if (isPrologue) SetPrologueStory(1);
            else if (isEpilogue) SetEpilogueStory(1);
            return;
        }

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

        if (currentStoryData.narrationStories[narrationIndex].playerExpression != ExpressionType.Unknown)
        {
            ExpressionType exp = currentStoryData.narrationStories[narrationIndex].playerExpression;
            narrationPlayerCharImage.sprite = DataHandler.instance.GetPlayerSprite(exp);
            narrationPlayerCharImage.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            narrationPlayerCharImage.sprite = null;
            narrationPlayerCharImage.transform.parent.gameObject.SetActive(false);
        }

        if (currentStoryData.narrationStories[narrationIndex].interlocutorSprite != null)
        {
            narrationInterlocutorCharImage.sprite = currentStoryData.narrationStories[narrationIndex].interlocutorSprite;
            narrationInterlocutorCharImage.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            narrationInterlocutorCharImage.sprite = null;
            narrationInterlocutorCharImage.transform.parent.gameObject.SetActive(false);
        }

        #region Setting Content
        if (DataHandler.instance.GetLanguage() == "id")
            SetStory(
                narrationText,
                currentStoryData.narrationStories[narrationIndex].clipCodeId,
                currentStoryData.narrationStories[narrationIndex].bgmClip,
                currentStoryData.narrationStories[narrationIndex].clipId,
                currentStoryData.narrationStories[narrationIndex].animPrefab,
                currentStoryData.narrationStories[narrationIndex].contentId
                );
        else if (DataHandler.instance.GetLanguage() == "en")
            SetStory(
                narrationText,
                currentStoryData.narrationStories[narrationIndex].clipCodeEn,
                currentStoryData.narrationStories[narrationIndex].bgmClip,
                currentStoryData.narrationStories[narrationIndex].clipEn,
                currentStoryData.narrationStories[narrationIndex].animPrefab,
                currentStoryData.narrationStories[narrationIndex].contentEn
                );
        else
            SetStory(
                narrationText,
                currentStoryData.narrationStories[narrationIndex].clipCodeMy,
                currentStoryData.narrationStories[narrationIndex].bgmClip,
                currentStoryData.narrationStories[narrationIndex].clipMy,
                currentStoryData.narrationStories[narrationIndex].animPrefab,
                currentStoryData.narrationStories[narrationIndex].contentMy
                );
        #endregion
    }

    public void SetPopUpStory(int factor)
    {
        if (!isSkippable) return;
        popUpIndex += factor;
        if (popUpIndex == currentStoryData.popUpStories.Count)
        {
            popUpIndex = 0;
            StartCoroutine(IECloseScreen(popUpPanel.GetComponent<CanvasGroup>(), () =>
            {
                popUpPanel.SetActive(false);
            }));

            if (isPrologue) SetPrologueStory(1);
            else if (isEpilogue) SetEpilogueStory(1);
            return;
        }

        #region Setting Content
        if (DataHandler.instance.GetLanguage() == "id")
            SetStory(
                popUpText,
                currentStoryData.popUpStories[popUpIndex].clipCodeId,
                currentStoryData.popUpStories[popUpIndex].bgmClip,
                currentStoryData.popUpStories[popUpIndex].clipId,
                currentStoryData.popUpStories[popUpIndex].animPrefab,
                currentStoryData.popUpStories[popUpIndex].contentId
                );
        else if (DataHandler.instance.GetLanguage() == "en")
            SetStory(
                popUpText,
                currentStoryData.popUpStories[popUpIndex].clipCodeEn,
                currentStoryData.popUpStories[popUpIndex].bgmClip,
                currentStoryData.popUpStories[popUpIndex].clipEn,
                currentStoryData.popUpStories[popUpIndex].animPrefab,
                currentStoryData.popUpStories[popUpIndex].contentEn
                );
        else
            SetStory(
                popUpText,
                currentStoryData.popUpStories[popUpIndex].clipCodeMy,
                currentStoryData.popUpStories[popUpIndex].bgmClip,
                currentStoryData.popUpStories[popUpIndex].clipMy,
                currentStoryData.popUpStories[popUpIndex].animPrefab,
                currentStoryData.popUpStories[popUpIndex].contentMy
                );
        #endregion
    }

    public void SetTitleStory(int factor)
    {
        if (!isSkippable) return;
        titleIndex += factor;
        if (titleIndex == currentStoryData.titleStories.Count)
        {
            titleIndex = 0;
            StartCoroutine(IECloseScreen(titlePanel.GetComponent<CanvasGroup>(), () =>
            {
                titlePanel.SetActive(false);
            }));

            if (isPrologue) SetPrologueStory(1);
            else if (isEpilogue) SetEpilogueStory(1);
            return;
        }

        #region Setting Content
        if (DataHandler.instance.GetLanguage() == "id")
            SetStory(
                titleText,
                currentStoryData.titleStories[titleIndex].clipCodeId,
                currentStoryData.titleStories[titleIndex].bgmClip,
                currentStoryData.titleStories[titleIndex].clipId,
                currentStoryData.titleStories[titleIndex].animPrefab,
                currentStoryData.titleStories[titleIndex].contentId
                );
        else if (DataHandler.instance.GetLanguage() == "en")
            SetStory(
                titleText,
                currentStoryData.titleStories[titleIndex].clipCodeEn,
                currentStoryData.titleStories[titleIndex].bgmClip,
                currentStoryData.titleStories[titleIndex].clipEn,
                currentStoryData.titleStories[titleIndex].animPrefab,
                currentStoryData.titleStories[titleIndex].contentEn
                );
        else
            SetStory(
                titleText,
                currentStoryData.titleStories[titleIndex].clipCodeMy,
                currentStoryData.titleStories[titleIndex].bgmClip,
                currentStoryData.titleStories[titleIndex].clipMy,
                currentStoryData.titleStories[titleIndex].animPrefab,
                currentStoryData.titleStories[titleIndex].contentMy
                );
        #endregion
    }

    public void SetTutorialStory(string key)
    {
        if (!isSkippable) return;
        FindObjectsOfType<SequencePanelHandler>().ToList().
            Find(seq => seq.key == key).Init();
    }

    public void SetStory(TextMeshProUGUI text, string playerAudioClipCode, AudioClip storyClip, AudioClip clip, GameObject animPrefab, string textValue)
    {
        AudioSource voAudioSource = MainMenuHandler.instance.GetVOSource();
        AudioSource storyAudioSource = MainMenuHandler.instance.GetStorySource();

        if (currAnimation != null)
            Destroy(currAnimation);

        if (animPrefab != null)
        {
            currAnimation = Instantiate(animPrefab);
            RectTransform rectTransform = currAnimation.GetComponent<RectTransform>();

            currAnimation.transform.SetParent(storyPanel.transform);
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

        if (!string.IsNullOrEmpty(playerAudioClipCode))
        {
            voAudioSource.clip = DataHandler.instance.GetPlayerClip(playerAudioClipCode);
            voAudioSource.Play();
        }
        else if (clip != null)
        {
            voAudioSource.clip = clip;
            voAudioSource.Play();
        }

        StartCoroutine(IESetSkippable());
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

    #region OpenClosePanel
    IEnumerator IEOpenScreen(CanvasGroup screen, Action executeAfter = null)
    {
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
    }
    #endregion

    IEnumerator IESetSkippable()
    {
        isSkippable = false;
        yield return new WaitForSeconds(delayTime);
        isSkippable = true;
    }
}
