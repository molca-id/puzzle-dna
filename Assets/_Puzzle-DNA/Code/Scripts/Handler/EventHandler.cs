using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class EventHandler : MonoBehaviour
{
    public float splashSpeed;
    public GameObject eventPanel;

    [Header("Talent Group Data Attribute")]
    public int talentId;
    public int talentGroupIndex;
    public AssessmentValue currentAssessmentValue;
    public List<TalentGroupValue> talentGroupValues;

    [Header("UI Attribute")]
    public Image playerCharImage;
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI currentSliderValueText;
    public Slider talentSlider;

    public void InitData(List<TalentGroupValue> talentGroupValues)
    {
        this.talentGroupValues = talentGroupValues;

        eventPanel.SetActive(true);
        playerCharImage.sprite = DataHandler.instance.GetPlayerSprite(ExpressionType.Netral);
        StartCoroutine(IEOpenScreen(eventPanel.GetComponent<CanvasGroup>()));
        InitEvent();
    }

    public void InitEvent()
    {
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

        questionText.text = currentAssessmentValue.assessment_description;
        currentSliderValueText.text = string.Empty;
        UpdateSliderValueText(5);
        talentSlider.value = 5;
    }

    public void UpdateSliderValueText(float value)
    {
        if (value < 0.1f || value > 9.9f)
        {
            currentSliderValueText.text = string.Empty;
        }
        else
        {
            currentSliderValueText.text = value.ToString("F1");
        }
    }

    public void SubmitAnswer()
    {
        float value = float.Parse(talentSlider.value.ToString("F1"));
        DataHandler.instance.currentUserData.data.assessment_values.Find(res => res.assessment_id == talentId).assessment_score = value;
        talentGroupValues.Find(res => res.assessment_id == talentId).assessment_score = value;

        StartCoroutine(IEOpenScreen(MainMenuHandler.instance.smallLoadingPanel, delegate
            {
                DataHandler.instance.IEPatchPerksValue(delegate
                {
                    StartCoroutine(IECloseScreen(MainMenuHandler.instance.smallLoadingPanel));
                    if (talentGroupValues.Find(res => 
                        res.assessment_score < 0 || 
                        res.assessment_score > 10) != null)
                    {
                        InitEvent();
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
