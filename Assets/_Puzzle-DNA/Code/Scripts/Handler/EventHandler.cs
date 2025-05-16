using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Events;

public class EventHandler : MonoBehaviour
{
    public float splashSpeed;
    public GameObject eventPanel;
    public AudioSource voAudioSource;

    [Header("Talent Group Data Attribute")]
    public int talentId;
    public int talentGroupIndex;
    public AssessmentValue currentAssessmentValue;
    public List<TalentGroupValue> talentGroupValues;

    [Header("UI Attribute")]
    public Image playerCharImage;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI currentSliderValueText;
    public Button playVoiceOverButton;
    public Slider talentSlider;
    public Image sliderFillImage;
    public Gradient sliderColorGradient;

    public void InitData(List<TalentGroupValue> talentGroupValues)
    {
        this.talentGroupValues = talentGroupValues;

        eventPanel.SetActive(true);
        playerCharImage.sprite = DataHandler.instance.GetPlayerSprite(ExpressionType.Netral);
        StartCoroutine(IEOpenScreen(eventPanel.GetComponent<CanvasGroup>()));
        SetupLoadingBeforeEvent();
    }

    public void SetupLoadingBeforeEvent()
    {
        playVoiceOverButton.onClick.RemoveAllListeners();
        talentId = Convert.ToInt32(talentGroupValues.Find
                    (res => res.assessment_score < 0 || res.assessment_score > 10).
                    assessment_id);

        foreach (var item in DataHandler.instance.talentDatas.data)
        {
            foreach (var assessment in item.item_talent)
            {
                if (assessment.id == talentId)
                {
                    currentAssessmentValue = new();
                    currentAssessmentValue.assessment_id = assessment.id;
                    currentAssessmentValue.assessment_description = assessment.item;
                    currentAssessmentValue.assessment_vo = assessment.vo;
                    break;
                }
            }
        }
        
        if (!string.IsNullOrEmpty(currentAssessmentValue.assessment_vo))
        {
            UnityEvent unityEvent = new UnityEvent();
            unityEvent.AddListener(() =>
            {
                playVoiceOverButton.interactable = true;
                playVoiceOverButton.onClick.AddListener(() => voAudioSource.Play());
                questionText.text = currentAssessmentValue.assessment_description + " <sprite=0>";
                
                StartCoroutine(IECloseScreen(MainMenuHandler.instance.bigLoadingPanel));
                InitEvent();
            });

            StartCoroutine(IEOpenScreen(MainMenuHandler.instance.bigLoadingPanel, delegate
            {
                StartCoroutine(
                    DownloadAndPlayM4A(
                        currentAssessmentValue.assessment_vo, 
                        unityEvent
                        ));
            }));
        }
        else
        {
            playVoiceOverButton.interactable = false;
            questionText.text = currentAssessmentValue.assessment_description;
            InitEvent();
        }
    }

    public void InitEvent()
    {
        currentSliderValueText.text = string.Empty;
        UpdateSliderValueText(5);
        talentSlider.value = 5;
        UpdateSliderFillColor(5);
    }

    public void UpdateSliderValueText(float value)
    {
        if (value < 0.1f || value > talentSlider.maxValue - 0.1f)
        {
            currentSliderValueText.text = string.Empty;
        }
        else
        {
            currentSliderValueText.text = value.ToString("F1");
        }
        UpdateSliderFillColor(value);
    }

    private void UpdateSliderFillColor(float value)
    {
        if (sliderFillImage != null)
        {
            float normalizedValue = Mathf.Clamp01(value / talentSlider.maxValue);
            sliderFillImage.color = sliderColorGradient.Evaluate(normalizedValue);
        }
    }

    public void SubmitAnswer()
    {
        float value = float.Parse(talentSlider.value.ToString("F1"));
        DataHandler.instance.currentUserData.data.assessment_values.Find(res => res.assessment_id == talentId).assessment_score = value;
        talentGroupValues.Find(res => res.assessment_id == talentId).assessment_score = value;
        voAudioSource.Stop();

        StartCoroutine(IEOpenScreen(MainMenuHandler.instance.smallLoadingPanel, delegate
            {
                DataHandler.instance.IEPatchPerksValue(delegate
                {
                    StartCoroutine(IECloseScreen(MainMenuHandler.instance.smallLoadingPanel));
                    if (talentGroupValues.Find(res => 
                        res.assessment_score < 0 || 
                        res.assessment_score > 10) != null)
                    {
                        SetupLoadingBeforeEvent();
                    }
                    else
                    {
                        if (LevelDataHandler.instance.isPrologue)
                            LevelDataHandler.instance.SetPrologueStory(1);

                        if (LevelDataHandler.instance.isEpilogue) 
                            LevelDataHandler.instance.SetEpilogueStory(1);

                        StartCoroutine(IECloseScreen(eventPanel.GetComponent<CanvasGroup>(), () =>
                        {
                            eventPanel.SetActive(false);
                            currentAssessmentValue = new();
                            talentGroupValues = new();
                        }));
                    }
                });
            }));
    }

    IEnumerator DownloadAndPlayM4A(string url, UnityEvent onComplete = null)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                voAudioSource.clip = clip;
                voAudioSource.Play();
            }
            else
            {
                Debug.LogError($"Failed to download audio: {www.error}");
            }
            
            onComplete?.Invoke();
        }
    }

    #region OpenClosePanel
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
    #endregion
}
