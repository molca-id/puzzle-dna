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
    public bool willPlayVO;
    [ShowIf("willOpenGame")] public LevelData levelData;
    [ShowIf("willPlayVO")] public AudioClip voClipEn;
    [ShowIf("willPlayVO")] public AudioClip voClipId;
    public UnityEvent whenGameLoaded;
    public UnityEvent whenGameUnloaded;
    public UnityEvent sequenceEvent;
}

public class SequencePanelHandler : MonoBehaviour
{
    public string key;
    public GameObject parentPanel;
    public AudioSource voAudioSource;
    public bool startAutomatically;
    [Space]
    public List<GameObject> panels;
    public List<SequenceEventsData> sequenceEvents;

    [Header("Add On For Game")]
    public float delaySkippable;
    public bool isSkippable;

    int index;

    void Start()
    {
        if (!startAutomatically) return;
        Init();
    }

    public void Init()
    {
        index = 0;
        SetPanel();

        if (parentPanel == null) return; 
        parentPanel.SetActive(true);
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

        if (data.willPlayVO)
        {
            AudioClip clip = DataHandler.instance.GetLanguage() == "id" ? data.voClipId : data.voClipEn;
            voAudioSource.PlayOneShot(clip);
        }

        data.sequenceEvent.Invoke();
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

    public void DisableAllPanels()
    {
        panels.ForEach(panel => panel.SetActive(false));
    }

    IEnumerator DelayingSkippable()
    {
        isSkippable = false;

        yield return new WaitForSeconds(delaySkippable);
        if (voAudioSource != null) yield return new WaitUntil(() => !voAudioSource.isPlaying);

        isSkippable = true;
    }
}
