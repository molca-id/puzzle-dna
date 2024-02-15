using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

public class GameTutorialHandler : MonoBehaviour
{
    public static GameTutorialHandler instance;
    [SerializeField] int index;
    [SerializeField] Canvas generalCanvas;
    [SerializeField] Transform gamePanel;
    [SerializeField] Transform tutorialPanel;
    [SerializeField] List<GameObject> descriptionPanels;
    [SerializeField] List<UnityEvent> tutorialEvents;

    [Header("Delaying Skippable")]
    [SerializeField] float delaySkippable;
    [SerializeField] bool isSkippable;

    void Awake()
    {
        instance = this;
        tutorialPanel.gameObject.SetActive(false);       
    }

    public void InitTutorial()
    {
        tutorialPanel.gameObject.SetActive(true);
        NextTutorial();
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
        GameController.instance.tutorialIsDone = true;
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
