using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

public class GameTutorialHandler : MonoBehaviour
{
    [SerializeField] int index;
    public string tutorialKey;
    [SerializeField] Canvas generalCanvas;
    [SerializeField] Transform gamePanel;
    [SerializeField] Transform tutorialPanel;
    [SerializeField] List<GameObject> descriptionPanels;
    [SerializeField] List<UnityEvent> tutorialEvents;

    [Header("Delaying Skippable")]
    [SerializeField] float delaySkippable;
    [SerializeField] bool isSkippable;

    public void InitTutorial(bool use)
    {
        if (GameController.instance.standalone) 
        {
            if (!use) GameController.instance.StartTimer();
            else
            {
                tutorialPanel.gameObject.SetActive(true);
                NextTutorial();
            }
            return;
        }

        if (!DataHandler.instance.GetUserCheckpointData().
            checkpoint_value[LevelDataHandler.instance.currentGameData.gameLevel].
            game_is_done && use)
        {
            tutorialPanel.gameObject.SetActive(true);
            NextTutorial();
        }
        else
        {
            tutorialPanel.gameObject.SetActive(false);
            FinishTutorial();
        }
    }

    public void FinishTutorial()
    {
        generalCanvas.sortingLayerName = "UI";
        tutorialPanel.gameObject.SetActive(false);
        descriptionPanels.ForEach(panel =>
        {
            panel.SetActive(false);
        });

        BoardController.usingTutorial = false;
        GameController.instance.tutorial_is_done = true;
        GameController.instance.StartTimer();
    }

    public void NextTutorial()
    {
        if (!isSkippable) return;
        descriptionPanels.ForEach(panel =>
        {
            panel.SetActive(false);
        });

        if (index < tutorialEvents.Count)
        {
            tutorialEvents[index].Invoke();
            index++;
        }

        StartCoroutine(DelayingSkippable());
    }

    public void StartGame()
    {
        HintController.FindHints();
        HintController.StartHinting();
        GameController.instance.gemIsInteractable = true;

        generalCanvas.sortingLayerName = "Background";
        for (int i = 0; i < tutorialPanel.childCount - 1; i++)
        {
            tutorialPanel.GetChild(i).SetParent(gamePanel);
            tutorialPanel.GetChild(i).SetAsLastSibling();
        }
    }

    public void MovingPanelToTutorial(Transform panel)
    {
        if (tutorialPanel.childCount > 1)
        {
            for (int i = 0; i < tutorialPanel.childCount - 1; i++)
            {
                tutorialPanel.GetChild(i).SetParent(gamePanel);
                tutorialPanel.GetChild(i).SetAsLastSibling();
            }
        }

        panel.SetParent(tutorialPanel);
        panel.SetAsFirstSibling();
    }

    IEnumerator DelayingSkippable()
    {
        isSkippable = false;
        yield return new WaitForSeconds(delaySkippable);
        isSkippable = true;
    }
}
