using ActionCode.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SequencePanelHandler : MonoBehaviour
{
    public List<GameObject> panels;
    public UnityEvent afterFinished;

    [Header("Add On For Game")]
    public bool usingGameScene;
    public CommonHandler commonHandler;
    public UnityEvent whenGameLoaded;

    int index;

    void Start()
    {
        index = 0;
        SetPanel();

        if (usingGameScene)
            commonHandler.whenSceneLoaded = whenGameLoaded;
    }

    void SetPanel()
    {
        panels.ForEach(panel => panel.SetActive(false));
        panels[index].SetActive(true);
    }

    public void PrevPanel()
    {
        if (index <= 0) return;
        
        index--;
        SetPanel();
    }

    public void NextPanel()
    {
        if (index >= panels.Count - 1)
        {
            afterFinished.Invoke();
            return;
        }

        index++;
        SetPanel();
    }
}
