using ActionCode.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class SequenceEventsData
{
    public bool willOpenGame;
    public LevelData levelData;
    [Space]
    public bool willPlayVO;
    public AudioClip voClipEn;
    public AudioClip voClipId;
    public AudioClip voClipMy;
    [Space]
    public bool willGetPlayerSprite;
    public ExpressionType playerExpressionType;
    public Image playerCharImage;
    [Space]
    public bool skippableWithoutDelay;
    public AudioClip bgmClip;
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
    public bool skippableAlthoughVO;
    public bool disableParentPanelAfterDone;
    public List<GameObject> parentPanel;
    [Space]
    public List<GameObject> panels;
    public List<SequenceEventsData> sequenceEvents;

    [Header("Add On For Game")]
    public float delaySkippable;
    public bool isSkippable;

    AudioSource storyAudioSource;
    AudioSource voAudioSource;

    void Start()
    {
        if (!startAutomatically) return;
        Init();
    }

    public void Init()
    {
        index = 0;
        SetPanel();

        voAudioSource = MainMenuHandler.instance.GetVOSource();
        storyAudioSource = MainMenuHandler.instance.GetStorySource();
        if (parentPanel.Count == 0) return; 
        parentPanel.ForEach(panel =>
        {
            panel.SetActive(true);
        });
    }

    public void SetPanel()
    {
        SequenceEventsData data = sequenceEvents[index];
        if (data.willGetPlayerSprite)
        {
            data.playerCharImage.sprite = 
                DataHandler.instance.GetPlayerSprite(data.playerExpressionType);
        }

        if (data.bgmClip != null)
        {
            storyAudioSource.clip = data.bgmClip;
            storyAudioSource.Play();
        }

        if (data.willPlayVO)
        {
            if (DataHandler.instance.GetLanguage() == "id" && data.voClipId != null)
                voAudioSource.clip = data.voClipId;
            else if (DataHandler.instance.GetLanguage() == "en" && data.voClipEn != null)
                voAudioSource.clip = data.voClipEn;
            else if (DataHandler.instance.GetLanguage() == "my" && data.voClipMy != null)
                voAudioSource.clip = data.voClipMy;

            if (voAudioSource.clip != null)
                voAudioSource.Play();
        }

        if (data.willOpenGame)
        {
            CommonHandler.instance.whenSceneLoadedCustom = data.whenGameLoaded;
            CommonHandler.instance.whenSceneUnloadedCustom = data.whenGameUnloaded;
            LevelDataHandler.instance.InitPrologue(data.levelData);
        }
        else
        {
            panels.ForEach(panel =>
            {
                panel.SetActive(false);
            });
        }

        data.sequenceEvent.Invoke();
        StartCoroutine(DelayingSkippable());
    }

    public void NextPanel()
    {
        SequenceEventsData data = sequenceEvents[index];

        if (index >= sequenceEvents.Count - 1) return;
        if (!skippableAlthoughVO && data.willPlayVO) return;
        if (!isSkippable) return;

        if (storyAudioSource != null &&
            storyAudioSource.isPlaying)
        {
            storyAudioSource.Stop();
        }

        if (voAudioSource != null &&
            voAudioSource.isPlaying)
        {
            voAudioSource.Stop();
        }

        index++;
        SetPanel();

        if (!disableParentPanelAfterDone) return;
        if (index < sequenceEvents.Count - 1) return;
        parentPanel.ForEach(panel =>
        {
            panel.SetActive(false);
        });
    }

    IEnumerator DelayingSkippable()
    {
        isSkippable = false;
        SequenceEventsData data = sequenceEvents[index];

        if (data.willPlayVO)
        {
            if (skippableAlthoughVO)
            {
                if (!data.skippableWithoutDelay) yield return new WaitForSeconds(delaySkippable);
                else yield return new WaitForSeconds(0f);
                isSkippable = true;
            }
            else
            {
                yield return new WaitUntil(() => !voAudioSource.isPlaying);
                yield return new WaitForSeconds(0.5f);

                index++;
                SetPanel();
            }
        }
        else
        {
            if (!data.skippableWithoutDelay) yield return new WaitForSeconds(delaySkippable);
            else yield return new WaitForSeconds(0f);
            isSkippable = true;
        }
    }
}
