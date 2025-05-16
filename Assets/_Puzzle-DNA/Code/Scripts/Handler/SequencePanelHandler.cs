using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SequenceEventsData
{
    public bool willOpenGame;
    public LevelData levelData;
    public bool willPlayVO;
    public AudioClip voClipEn, voClipId, voClipMy;
    public bool willGetPlayerSprite;
    public ExpressionType playerExpressionType;
    public Image playerCharImage;
    public bool usingHandClick;
    public bool skippableWithoutDelay;
    public AudioClip bgmClip;
    public UnityEvent whenGameLoaded, whenGameUnloaded, sequenceEvent;
}

public class SequencePanelHandler : MonoBehaviour
{
    [Header("Basic Settings")]
    public int index;
    public string key;
    public bool startAutomatically;
    
    [Header("Panel Settings")]
    public bool skippableAlthoughVO;
    public bool disableParentPanelAfterDone;
    public List<GameObject> parentPanel;
    public List<GameObject> panels;
    public List<SequenceEventsData> sequenceEvents;

    [Header("Audio")]
    public AudioSource storyAudioSource;
    public AudioSource voAudioSource;

    [Header("Skip Settings")]
    public GameObject handClick;
    public float delayHandClick = 1.5f;
    public float delaySkippable;
    public bool isSkippable;

    void Start()
    {
        if (startAutomatically) Init();
    }

    public void Init()
    {
        index = 0;
        voAudioSource = MainMenuHandler.instance.GetVOSource();
        storyAudioSource = MainMenuHandler.instance.GetStorySource();
        SetPanel();
        
        if (parentPanel.Count > 0)
            parentPanel.ForEach(panel => panel.SetActive(true));
    }

    public void SetPanel()
    {
        handClick?.SetActive(false);
        StopCoroutine(ShowHandClick());

        var data = sequenceEvents[index];
        
        if (data.willGetPlayerSprite)
            data.playerCharImage.sprite = DataHandler.instance.GetPlayerSprite(data.playerExpressionType);

        if (data.bgmClip != null)
        {
            storyAudioSource.clip = data.bgmClip;
            storyAudioSource.Play();
        }

        if (data.willPlayVO)
        {
            string lang = DataHandler.instance.GetLanguage();
            AudioClip clip = lang == "id" ? data.voClipId : 
                           lang == "en" ? data.voClipEn : 
                           lang == "my" ? data.voClipMy : null;
                           
            if (clip != null)
            {
                voAudioSource.clip = clip;
                voAudioSource.Play();
            }
        }

        if (data.willOpenGame)
        {
            CommonHandler.instance.whenGameLoaded = data.whenGameLoaded;
            CommonHandler.instance.whenGameUnloaded = data.whenGameUnloaded;
            LevelDataHandler.instance.InitPrologue(data.levelData);
        }
        else
        {
            panels.ForEach(panel => panel.SetActive(false));
        }

        data.sequenceEvent.Invoke();
        StartCoroutine(DelayingSkippable());

        if (data.usingHandClick)
            StartCoroutine(ShowHandClick());
    }

    public void NextPanel()
    {
        var data = sequenceEvents[index];
        
        if (index >= sequenceEvents.Count - 1 || 
            (!skippableAlthoughVO && data.willPlayVO) || 
            !isSkippable) return;

        if (storyAudioSource?.isPlaying == true)
            storyAudioSource.Stop();

        if (voAudioSource?.isPlaying == true)
            voAudioSource.Stop();

        index++;
        SetPanel();

        if (disableParentPanelAfterDone && index >= sequenceEvents.Count - 1)
            parentPanel.ForEach(panel => panel.SetActive(false));
    }

    IEnumerator DelayingSkippable()
    {
        isSkippable = false;
        var data = sequenceEvents[index];

        if (data.willPlayVO)
        {
            yield return new WaitForSeconds(data.skippableWithoutDelay ? 0f : delaySkippable);
            isSkippable = true;

            yield return new WaitUntil(() => !voAudioSource.isPlaying);
            yield return new WaitForSeconds(.25f);
            index++;
            SetPanel();
        }
        else
        {
            yield return new WaitForSeconds(data.skippableWithoutDelay ? 0f : delaySkippable);
            isSkippable = true;
        }
    }

    public IEnumerator ShowHandClick()
    {
        handClick?.SetActive(false);
        yield return new WaitUntil(() => isSkippable);
        if (!skippableAlthoughVO)
        {
            handClick?.SetActive(true);
        }
    }
}
