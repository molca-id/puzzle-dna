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
    [SerializeField] List<UnityEvent> tutorialEvents;

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

        GameController.instance.tutorialIsDone = true;
        GameController.instance.StartTimer();
    }

    public void NextTutorial()
    {
        if (index < tutorialEvents.Count)
        {
            tutorialEvents[index].Invoke();
            index++;
        }
        else
        {
            StartGame();
        }
    }

    void StartGame()
    {
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
}
