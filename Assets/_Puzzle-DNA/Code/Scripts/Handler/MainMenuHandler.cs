using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UserDataSpace;

[Serializable]
public class CharacterSelectionUI
{
    public Character character;
    public GameObject selectedBackground;
    public GameObject descriptionPanel;
}

public class MainMenuHandler : MonoBehaviour
{
    public static MainMenuHandler instance;

    [Header("Welcome Attributes")]
    [SerializeField] TextMeshProUGUI playerNameWelcome;

    [Header("Character Selection Attributes")]
    [SerializeField] Character character;
    [SerializeField] List<CharacterSelectionUI> characterSelections;

    [Header("Splash Attributes")]
    [SerializeField] float splashSpeed;
    [SerializeField] CanvasGroup tutorialPanel;
    [SerializeField] CanvasGroup mainMenuPanel;
    [SerializeField] CanvasGroup loadingPanel;

    [Header("Level Attributes")]
    [SerializeField] List<Button> levelButtonDatas;

    private void Awake()
    {
        instance = this;    
    }

    private void Start()
    {
        InitMenu();
    }

    #region Tutorial
    public void SelectLanguage(string lang)
    {
        DataHandler.instance.GetUserDataValue().language = lang;
    }

    public void SubmitLanguage()
    {
        StartCoroutine(IEOpenScreen(loadingPanel, delegate
        {
            DataHandler.instance.IEPatchLanguageData(delegate
            {
                StartCoroutine(IECloseScreen(loadingPanel));
                InitMenu();
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
    }

    public void SubmitCharacter()
    {
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

    public void InitMenu() 
    {
        CanvasGroup willEnable, willDisabled;
        if (DataHandler.instance.GetUserDataValue().checkpoint_data.tutorial_is_done)
        {
            willEnable = mainMenuPanel;
            willDisabled = tutorialPanel;
        }
        else
        {
            willEnable = tutorialPanel;
            willDisabled = mainMenuPanel;
            playerNameWelcome.text = DataHandler.instance.GetUserDataValue().username;
            SelectCharacter(0);
        }

        willEnable.gameObject.SetActive(true);
        willDisabled.gameObject.SetActive(false);

        if (!DataHandler.instance.GetUserDataValue().checkpoint_data.tutorial_is_done) return;
        SetupLevelButtons();
    }

    public void SetupLevelButtons()
    {
        for (int i = 0; i < levelButtonDatas.Count; i++)
        {
            levelButtonDatas[i].transform.Find("Pinpoint").gameObject.SetActive(false);
            levelButtonDatas[i].transform.Find("LevelText").GetComponent<TextMeshProUGUI>().text = $"{i + 1}";

            if (DataHandler.instance.GetUserCheckpointData().checkpoint_level_datas[i].score > 0)
            {
                levelButtonDatas[i].transform.Find("ScorePanel").gameObject.SetActive(true);
                levelButtonDatas[i].transform.Find("ScorePanel").Find("ScoreText").GetComponent<TextMeshProUGUI>().text =
                    DataHandler.instance.GetUserCheckpointData().checkpoint_level_datas[i].score.ToString();
            }
            else
            {
                levelButtonDatas[i].transform.Find("ScorePanel").gameObject.SetActive(false);
            }

            if (i == 0 || i > 0 && DataHandler.instance.GetUserCheckpointData().checkpoint_level_datas[i - 1].score > 0)
            {
                levelButtonDatas[i].interactable = true;
                if (i == 0 && DataHandler.instance.GetUserCheckpointData().checkpoint_level_datas[i].score == 0 ||
                    DataHandler.instance.GetUserCheckpointData().checkpoint_level_datas[i].score == 0 &&
                    DataHandler.instance.GetUserCheckpointData().checkpoint_level_datas[i - 1].score > 0)
                    levelButtonDatas[i].transform.Find("Pinpoint").gameObject.SetActive(true);
            }
            else
            {
                levelButtonDatas[i].interactable = false;
            }
        }
    }

    public void PostIntroductionState(bool cond)
    {
        StartCoroutine(IEOpenScreen(loadingPanel, delegate
        {
            tutorialPanel.gameObject.SetActive(false);
            mainMenuPanel.gameObject.SetActive(false);
            DataHandler.instance.GetUserDataValue().checkpoint_data.tutorial_is_done = cond;
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
}
