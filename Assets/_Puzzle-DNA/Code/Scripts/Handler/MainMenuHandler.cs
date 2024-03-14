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
    public AudioClip voClipEn;
    public AudioClip voClipId;
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

        voAudioSource.Stop();
        voAudioSource.clip = DataHandler.instance.GetLanguage() == "id" ?
            characterSelections[(int)character].voClipId :
            characterSelections[(int)character].voClipEn;
        voAudioSource.Play();
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
            int index = i + 1;
            levelButtonDatas[i].transform.Find("Pinpoint").gameObject.SetActive(false);
            levelButtonDatas[i].transform.Find("LevelText").GetComponent<TextMeshProUGUI>().text = $"{i + 1}";
            levelButtonDatas[i].transform.Find("ScoreText").GetComponent<TextMeshProUGUI>().text =
                DataHandler.instance.GetUserCheckpointData().checkpoint_level_score[index].ToString();

            if (i == 0 || i > 0 && DataHandler.instance.GetUserCheckpointData().checkpoint_level_score[index - 1] > 0)
            {
                levelButtonDatas[i].interactable = true;
                levelButtonDatas[i].transform.Find("Disable").gameObject.SetActive(false);
                if (i == 0 && DataHandler.instance.GetUserCheckpointData().checkpoint_level_score[index] == 0 ||
                    DataHandler.instance.GetUserCheckpointData().checkpoint_level_score[index] == 0 &&
                    DataHandler.instance.GetUserCheckpointData().checkpoint_level_score[index - 1] > 0)
                    levelButtonDatas[i].transform.Find("Pinpoint").gameObject.SetActive(true);
            }
            else
            {
                levelButtonDatas[i].transform.Find("Disable").gameObject.SetActive(true);
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
