using ActionCode.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class SequencePanelHandler : MonoBehaviour
{
    public List<GameObject> panels;
    public List<UnityEvent> sequenceEvents;

    [Header("Add On For Game")]
    public float delaySkippable;
    public bool isSkippable;

    int index;

    void Start()
    {
        index = 0;
        SetPanel();
    }

    void SetPanel()
    {
        panels.ForEach(panel => panel.SetActive(false));
        sequenceEvents[index].Invoke();

        StartCoroutine(DelayingSkippable());
    }

    public void PrevPanel()
    {
        if (!isSkippable) return;
        if (index <= 0) return;
        
        index--;
        SetPanel();
    }

    public void NextPanel()
    {
        if (!isSkippable) return;
        if (index >= sequenceEvents.Count - 1) return;

        index++;
        SetPanel();
    }

    IEnumerator DelayingSkippable()
    {
        isSkippable = false;
        yield return new WaitForSeconds(delaySkippable);
        isSkippable = true;
    }
}
