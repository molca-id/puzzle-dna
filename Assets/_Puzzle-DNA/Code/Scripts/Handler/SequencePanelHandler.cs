using ActionCode.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class SequenceEventsData
{
    public bool willOpenGame;
    [ShowIf("willOpenGame")] public LevelData levelData;
    public bool willPlayVO;
    [ShowIf("willPlayVO")] public AudioClip voClipEn;
    [ShowIf("willPlayVO")] public AudioClip voClipId;
    public bool skippableWithoutDelay;
    public UnityEvent whenGameLoaded;
    public UnityEvent whenGameUnloaded;
    public UnityEvent sequenceEvent;
}

public class SequencePanelHandler : MonoBehaviour
{
    public int index;
    public string key;
    public bool startAutomatically;
    [Space]
    public AudioSource voAudioSource;
    public List<GameObject> parentPanel;
    [Space]
    public List<GameObject> panels;
    public List<SequenceEventsData> sequenceEvents;

    [Header("Add On For Game")]
    public float delaySkippable;
    public bool isSkippable;

    void Start()
    {
        if (!startAutomatically) return;
        Init();
    }

    public void Init()
    {
        index = 0;
        SetPanel();

        if (parentPanel.Count == 0) return; 
        parentPanel.ForEach(panel => panel.SetActive(true));
    }

    public void SetPanel()
    {
        SequenceEventsData data = sequenceEvents[index];
        for (int i = 0; i < index; i++)
        {
            if (panels[i] == null) continue;
            if (panels[i].GetComponentInChildren<Button>() == null) continue;
            panels[i].GetComponentInChildren<Button>().interactable = false;
        }

        if (data.willPlayVO)
        {
            AudioClip clip = DataHandler.instance.GetLanguage() == "id" ? data.voClipId : data.voClipEn;
            voAudioSource.PlayOneShot(clip);
        }

        if (data.willOpenGame)
        {
            CommonHandler.instance.whenSceneLoadedCustom = data.whenGameLoaded;
            CommonHandler.instance.whenSceneUnloadedCustom = data.whenGameUnloaded;
            LevelDataHandler.instance.Init(data.levelData);
        }
        else
        {
            DisableAllPanels();
        }

        data.sequenceEvent.Invoke();
        StartCoroutine(DelayingSkippable());
    }

    public void PrevPanel()
    {
        SequenceEventsData data = sequenceEvents[index];

        if (index <= 0) return;
        if (data.willPlayVO) return;
        if (!isSkippable) return;
        
        index--;
        SetPanel();
    }

    public void NextPanel()
    {
        SequenceEventsData data = sequenceEvents[index];

        if (index >= sequenceEvents.Count - 1) return;
        if (data.willPlayVO) return;
        if (!isSkippable) return;

        index++;
        SetPanel();
    }

    public void DisableAllPanels()
    {
        panels.ForEach(panel => panel.SetActive(false));
    }

    IEnumerator DelayingSkippable()
    {
        isSkippable = false;
        SequenceEventsData data = sequenceEvents[index];

        if (data.willPlayVO)
        {
            yield return new WaitUntil(() => !voAudioSource.isPlaying);
            yield return new WaitForSeconds(0.5f);

            index++;
            SetPanel();
        }
        else
        {
            if (!data.skippableWithoutDelay) yield return new WaitForSeconds(delaySkippable);
            else yield return new WaitForSeconds(0f);
            isSkippable = true;
        }
    }
}
