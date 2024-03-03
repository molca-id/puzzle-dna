using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    public static MainMenuHandler instance;

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

    public void InitMenu() 
    {
        CanvasGroup willEnable, willDisabled;
        if (DataHandler.instance.GetUserDataValue().checkpoint_data.tutorialIsDone)
        {
            willEnable = mainMenuPanel;
            willDisabled = tutorialPanel;
        }
        else
        {
            willEnable = tutorialPanel;
            willDisabled = mainMenuPanel;
        }

        willEnable.gameObject.SetActive(true);
        willDisabled.gameObject.SetActive(false);

        if (!DataHandler.instance.GetUserDataValue().checkpoint_data.tutorialIsDone) return;
        SetupLevelButtons();
    }

    void SetupLevelButtons()
    {
        for (int i = 0; i < levelButtonDatas.Count; i++)
        {
            levelButtonDatas[i].transform.Find("Pinpoint").gameObject.SetActive(false);
            levelButtonDatas[i].transform.Find("LevelText").GetComponent<TextMeshProUGUI>().text = $"{i + 1}";

            if (DataHandler.instance.GetUserCheckpointData().checkpointLevelDatas[i].score > 0)
            {
                levelButtonDatas[i].transform.Find("ScorePanel").gameObject.SetActive(true);
                levelButtonDatas[i].transform.Find("ScorePanel").Find("ScoreText").GetComponent<TextMeshProUGUI>().text =
                    DataHandler.instance.GetUserCheckpointData().checkpointLevelDatas[i].score.ToString();
            }
            else
            {
                levelButtonDatas[i].transform.Find("ScorePanel").gameObject.SetActive(false);
            }

            if (i == 0 || i > 0 && DataHandler.instance.GetUserCheckpointData().checkpointLevelDatas[i - 1].score > 0)
            {
                levelButtonDatas[i].interactable = true;
                if (i == 0 && DataHandler.instance.GetUserCheckpointData().checkpointLevelDatas[i].score == 0 ||
                    DataHandler.instance.GetUserCheckpointData().checkpointLevelDatas[i].score == 0 &&
                    DataHandler.instance.GetUserCheckpointData().checkpointLevelDatas[i - 1].score > 0)
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
            DataHandler.instance.GetUserDataValue().checkpoint_data.tutorialIsDone = cond;
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
            tutorialPanel.gameObject.SetActive(false);
            mainMenuPanel.gameObject.SetActive(false);
            DataHandler.instance.IEPatchCheckpointData(delegate
            {
                StartCoroutine(IECloseScreen(loadingPanel));
                InitMenu();
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
