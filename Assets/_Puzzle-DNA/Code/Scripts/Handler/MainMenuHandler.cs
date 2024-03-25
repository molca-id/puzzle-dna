using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UserDataSpace;
using TMPro;

[Serializable]
public class CharacterSelectionUI
{
    public Character character;
    public GameObject selectedBackground;
    public GameObject descriptionPanel;
    public AudioClip voClipEn;
    public AudioClip voClipId;
    public AudioClip voClipMy;
}

[Serializable]
public class GlossariumData
{
    public string name;
    public ContentData titleValue;
    public ContentData descriptionValue;
}

[Serializable]
public class LevelButtonData
{
    public GameObject stageObject;
    public List<Button> beforeButtons;
    public Button currentButton;
}

public class MainMenuHandler : MonoBehaviour
{
    public static MainMenuHandler instance;

    [Header("Welcome Attributes")]
    [SerializeField] TextMeshProUGUI playerNameWelcome;

    [Header("Character Selection Attributes")]
    [SerializeField] Character character;
    [SerializeField] AudioSource voAudioSource;
    [SerializeField] List<CharacterSelectionUI> characterSelections;

    [Header("Glossarium & HowTo Attributes")]
    [SerializeField] Button buttonTemp;
    [SerializeField] GameObject glosHowToPanel;
    [SerializeField] Transform glosHowToParentButton;
    [SerializeField] TextMeshProUGUI glosHowToDescriptionText;
    [SerializeField] GameObject glossariumTitle;
    [SerializeField] GameObject howToTitle;
    [SerializeField] Color idleColor;
    [SerializeField] Color selectedColor;
    [SerializeField] List<GlossariumData> glossariumDatas;
    [SerializeField] List<GlossariumData> howToDatas;
    [HideInInspector] public List<Button> glosHowToButtons;

    [Header("Splash Attributes")]
    [SerializeField] float splashSpeed;
    [SerializeField] CanvasGroup tutorialPanel;
    [SerializeField] CanvasGroup mainMenuPanel;
    [SerializeField] CanvasGroup loadingPanel;

    [Header("Level Attributes")]
    [SerializeField] List<Button> levelButton;
    [SerializeField] List<LevelButtonData> levelButtons;

    [Header("ScrollView Attributes")]
    [HideInInspector] public LevelButtonData levelButtonTemp;
    public MiddleButtonChecker middleButtonChecker;
    public ScrollRect scrollRect;
    public float scrollSpeed;
    public bool scrollAutomatically;

    private void Awake()
    {
        instance = this;    
    }

    private void Start()
    {
        InitMenu();
    }

    private void Update()
    {
        scrollRect.enabled = !scrollAutomatically;
        if (!scrollAutomatically) return;
        scrollRect.horizontalNormalizedPosition += scrollSpeed * Time.deltaTime;
        if (scrollRect.horizontalNormalizedPosition > 1)
            scrollAutomatically = false;
    }

    #region Tutorial
    public void SelectLanguage(string lang)
    {
        DataHandler.instance.GetUserDataValue().language = lang;
    }

    public void SubmitLanguage(SequencePanelHandler seq)
    {
        StartCoroutine(IEOpenScreen(loadingPanel, delegate
        {
            DataHandler.instance.IEPatchLanguageData(delegate
            {
                seq.NextPanel();
                StartCoroutine(IECloseScreen(loadingPanel));
            });
        }));
    }

    public void SelectCharacter(int index) 
    {
        character = (Character)index;
        characterSelections.ForEach(chars =>
        {
            chars.selectedBackground.SetActive(false);
            chars.descriptionPanel.SetActive(false);
        });

        characterSelections[(int)character].selectedBackground.SetActive(true);
        characterSelections[(int)character].descriptionPanel.SetActive(true);

        AudioClip clip;
        voAudioSource.Stop();
        if (DataHandler.instance.GetLanguage() == "id")
            clip = characterSelections[(int)character].voClipId;
        else if (DataHandler.instance.GetLanguage() == "en")
            clip = characterSelections[(int)character].voClipEn;
        else
            clip = characterSelections[(int)character].voClipMy;
        voAudioSource.clip = clip;
        voAudioSource.Play();
    }

    public void SubmitCharacter()
    {
        DataHandler.instance.currPlayerSpriteData = DataHandler.instance.playerSpriteDatas.Find(data => data.character == character);
        DataHandler.instance.GetUserDataValue().character = (int)character;
        StartCoroutine(IEOpenScreen(loadingPanel, delegate
        {
            DataHandler.instance.IEPatchCharacterData(delegate
            {
                StartCoroutine(IECloseScreen(loadingPanel));
            });
        }));
    }
    #endregion

    #region Glossarium HowTo
    public void InitGlosHowTo(bool isGlossarium)
    {
        glosHowToButtons.Clear();
        glosHowToPanel.SetActive(true);
        glossariumTitle.SetActive(isGlossarium);
        howToTitle.SetActive(!isGlossarium);

        if (glosHowToParentButton.childCount > 0)
            for (int i = 1; i < glosHowToParentButton.childCount; i++)
                if (glosHowToParentButton.GetChild(i).gameObject != buttonTemp)
                    Destroy(glosHowToParentButton.GetChild(i).gameObject);

        List<GlossariumData> newDatas = isGlossarium ? glossariumDatas : howToDatas;
        for (int i = 0; i < newDatas.Count; i++)
        {
            int index = i;
            Transform btn = Instantiate(buttonTemp).transform;
            glosHowToButtons.Add(btn.GetComponent<Button>());

            glosHowToButtons[index].transform.parent = glosHowToParentButton;
            glosHowToButtons[index].transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            glosHowToButtons[index].transform.localScale = Vector3.one;
            btn.gameObject.SetActive(true);

            if (DataHandler.instance.GetLanguage() == "id")
                glosHowToButtons[index].transform.GetComponentInChildren<TextMeshProUGUI>().text =
                    newDatas[index].titleValue.contentId;
            else if (DataHandler.instance.GetLanguage() == "en")
                glosHowToButtons[index].transform.GetComponentInChildren<TextMeshProUGUI>().text =
                    newDatas[index].titleValue.contentEn;
            else
                glosHowToButtons[index].transform.GetComponentInChildren<TextMeshProUGUI>().text =
                    newDatas[index].titleValue.contentMy;

            glosHowToButtons[index].onClick.AddListener(delegate
            {
                OpenGlosHowTo(newDatas, index);
            });
        }

        glosHowToParentButton.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(glosHowToParentButton.GetComponent<RectTransform>().anchoredPosition.x, 0f);
        OpenGlosHowTo(newDatas, 0);
    }

    public void OpenGlosHowTo(List<GlossariumData> datas, int index)
    {
        glosHowToButtons.ForEach(item => item.GetComponentInChildren<TextMeshProUGUI>().color = idleColor);
        glosHowToButtons[index].GetComponentInChildren<TextMeshProUGUI>().color = selectedColor;

        if (DataHandler.instance.GetLanguage() == "id")
            glosHowToDescriptionText.text = datas[index].descriptionValue.contentId;
        else if (DataHandler.instance.GetLanguage() == "en")
            glosHowToDescriptionText.text = datas[index].descriptionValue.contentEn;
        else
            glosHowToDescriptionText.text = datas[index].descriptionValue.contentMy;
    }
    #endregion

    #region Setup Menu
    public void InitMenu()
    {
        CanvasGroup willEnable, willDisabled;
        if (DataHandler.instance.GetUserCheckpointData().tutorial_is_done)
        {
            willEnable = mainMenuPanel;
            willDisabled = tutorialPanel;
            SetupLevelButtons();
        }
        else
        {
            willEnable = tutorialPanel;
            willDisabled = mainMenuPanel;
            playerNameWelcome.text = DataHandler.instance.GetUserDataValue().username;
        }

        willEnable.gameObject.SetActive(true);
        willDisabled.gameObject.SetActive(false);
    }

    public void SetupLevelButtons()
    {
        for (int i = 0; i < levelButton.Count; i++)
        {
            //setting ui
            int index = i + 1;
            levelButton[i].interactable = false;
            levelButton[i].transform.Find("Disable").gameObject.SetActive(true);
            levelButton[i].transform.Find("Pinpoint").gameObject.SetActive(false);
            levelButton[i].transform.Find("LevelText").GetComponent<TextMeshProUGUI>().text = $"{i + 1}";
            levelButton[i].transform.Find("ScoreText").GetComponent<TextMeshProUGUI>().text =
                DataHandler.instance.GetUserCheckpointData().checkpoint_value[index].checkpoint_level_score.ToString();

            //setting pinpoint
            if (levelButton.Find(button => button.transform.Find("Pinpoint").gameObject.activeSelf) == null &&
                DataHandler.instance.GetUserCheckpointData().checkpoint_value[index].checkpoint_level_score == 0)
            {
                levelButton[i].transform.Find("Pinpoint").gameObject.SetActive(true);
                middleButtonChecker.buttonTarget = levelButton[i].gameObject;
                scrollAutomatically = true;
            }

            //setting button addlistener
            int levelIndex = i;
            levelButton[i].onClick.RemoveAllListeners();
            levelButton[i].onClick.AddListener(() =>
                LevelDataHandler.instance.Init(
                    DataHandler.instance.levelDatas[index]
                    ));
        }

        foreach (var data in levelButtons)
        {
            if (data.beforeButtons.Count == 0 || ScoreChecker(data.beforeButtons))
            {
                if (data.stageObject != null) data.stageObject.transform.Find("Disable").gameObject.SetActive(false);
                data.currentButton.transform.Find("Disable").gameObject.SetActive(false);
                data.currentButton.interactable = true;
            }
        }
    }

    public bool ScoreChecker(List<Button> buttons)
    {
        foreach (var button in buttons)
        {
            if (button.GetComponent<Transform>().Find("ScoreText").
                GetComponent<TextMeshProUGUI>().text == "0")
            {
                return false;
            }
        }

        return true;
    }

    public void PlayCurrentLevel()
    {
        for (int i = 0; i < levelButton.Count; i++)
        {
            if (levelButton[i].transform.Find("Pinpoint").gameObject.activeSelf)
                LevelDataHandler.instance.Init(DataHandler.instance.levelDatas[i + 1]);
        }
    }
    #endregion

    #region API Hit
    public void PostIntroductionState(bool cond)
    {
        StartCoroutine(IEOpenScreen(loadingPanel, delegate
        {
            tutorialPanel.gameObject.SetActive(false);
            mainMenuPanel.gameObject.SetActive(false);
            DataHandler.instance.GetUserCheckpointData().tutorial_is_done = cond;
            DataHandler.instance.IEPatchCheckpointData(delegate
            {
                StartCoroutine(IECloseScreen(loadingPanel));
                InitMenu();
            });
        }));
    }

    public void PatchCheckpointFromMenu()
    {
        StartCoroutine(IEOpenScreen(loadingPanel, delegate
        {
            DataHandler.instance.IEPatchCheckpointData(delegate
            {
                StartCoroutine(IECloseScreen(loadingPanel));
                InitMenu();
            });
        }));
    }

    public void PatchPerksFromMenu(Action executeAfter = null)
    {
        StartCoroutine(IEOpenScreen(loadingPanel, delegate
        {
            DataHandler.instance.IEPatchPerksData(delegate
            {
                executeAfter.Invoke();
                StartCoroutine(IECloseScreen(loadingPanel));
            });
        }));
    }

    public void GetTalentPerksFromMenu(Action executeAfter = null)
    {
        StartCoroutine(IEOpenScreen(loadingPanel, delegate
        {
            DataHandler.instance.IEGetTalentData(delegate
            {
                executeAfter.Invoke();
                StartCoroutine(IECloseScreen(loadingPanel));
            });
        }));
    }

    public AudioSource GetVOSource() => voAudioSource;
    #endregion

    #region OpenClosePanel
    IEnumerator IEOpenScreen(CanvasGroup screen, Action executeAfter = null)
    {
        screen.gameObject.SetActive(true);
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
        screen.gameObject.SetActive(false);
    }
    #endregion
}
