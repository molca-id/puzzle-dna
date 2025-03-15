using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ResultValueData
{
    public int id_talent;
    public string nama_talent;
    public float score_talent;
    public int ranking_talent;
}

[Serializable]
public class ResultData
{
    public string survey_code;
    public List<ResultValueData> hasil_isi;
}

public class FinishHandler : MonoBehaviour
{
    public static FinishHandler instance;
    public GameObject parentPanel;
    public GameObject finalFadePanel;
    public GameObject finalResultPanel;
    public List<Sprite> perkIconsColorful;
    public List<Sprite> perkIconsWhite;

    [Header("Character UI")]
    public Image charReplaceSprite;
    public ExpressionType expressionType;

    [Header("Perks UI Parent Ranking")]
    public GameObject parent5;
    public GameObject parent10;
    public GameObject parent4565;

    [Header("Perks UI Ranking")]
    public List<GameObject> top5PerksObject;
    public List<GameObject> topJust10PerksObject;
    public List<GameObject> top10PerksObject;
    public List<GameObject> bottom5PerksObject;

    [Header("Perks Ranking")]
    public ResultData resultData;
    public List<ResultValueData> top5Perks;
    public List<ResultValueData> top10Perks;
    public List<ResultValueData> bottom5Perks; 

    private void Awake()
    {
        instance = this;
    }

    public void InitFinishHandler()
    {
        StartCoroutine(IEInitFinishHandler());
    }

    IEnumerator IEInitFinishHandler()
    {
        DataHandler.instance.IEGetItemData();
        yield return new WaitUntil(() => DataHandler.instance.talentDatas.data.Count > 0);

        CalculateTalentValue();
        CalculateFinalResult();
    }

    void CalculateTalentValue()
    {
        foreach (var item in DataHandler.instance.talentDatas.data)
        {
            float totalScore = 0;
            for (int i = 0; i < item.item_talent.Count; i++)
            {
                item.item_talent[i].score = DataHandler.instance.GetUserAssessmentData().
                    Find(x => x.assessment_id == item.item_talent[i].id).
                    assessment_score;

                totalScore += item.item_talent[i].score;
            }

            item.score_talent = totalScore / item.item_talent.Count;

            resultData.hasil_isi.Add(new ResultValueData
            {
                id_talent = item.id_talent,
                nama_talent = item.nama_talent,
                score_talent = item.score_talent,
                ranking_talent = 0
            });
        }

        resultData.survey_code = DataHandler.instance.GetUserDataValue().game_url;
        resultData.hasil_isi = resultData.hasil_isi.OrderByDescending(x => x.score_talent)
            .ThenBy(x => x.nama_talent)
            .ToList();

        for (int i = 0; i < resultData.hasil_isi.Count; i++)
        {
            resultData.hasil_isi[i].ranking_talent = i + 1;
        }
    }

    void CalculateFinalResult()
    {
        StartCoroutine(IEOpenScreen(finalFadePanel.GetComponent<CanvasGroup>(), () => { }));
        StartCoroutine(IECloseScreen(finalResultPanel.GetComponent<CanvasGroup>(), () => { }));

        bottom5Perks.Clear();
        top5Perks = resultData.hasil_isi.Take(5).ToList();
        top10Perks = resultData.hasil_isi.Take(10).ToList();
        for (int i = 40; i < resultData.hasil_isi.Count; i++)
        {
            bottom5Perks.Add(resultData.hasil_isi[i]);
        }

        charReplaceSprite.sprite = DataHandler.instance.GetPlayerSprite(expressionType);
        switch (DataHandler.instance.GetUserDataValue().f_report_type)
        {
            case "5":
                parent5.SetActive(true);
                for (int i = 0; i < top5PerksObject.Count; i++)
                {
                    top5PerksObject[i].transform.GetComponentInChildren<Image>().sprite =
                        perkIconsColorful.Find(res => res.name.ToLower().Contains(top10Perks[i].nama_talent.ToLower()));
                    top5PerksObject[i].transform.GetComponentInChildren<TextMeshProUGUI>().text =
                        top10Perks[i].nama_talent;
                }
                break;
            case "10":
                parent10.SetActive(true);
                for (int i = 0; i < topJust10PerksObject.Count; i++)
                {
                    topJust10PerksObject[i].transform.GetComponentInChildren<Image>().sprite =
                        perkIconsColorful.Find(res => res.name.ToLower().Contains(top10Perks[i].nama_talent.ToLower()));
                    topJust10PerksObject[i].transform.GetComponentInChildren<TextMeshProUGUI>().text =
                        top10Perks[i].nama_talent;
                }
                break;
            case "45":
                parent4565.SetActive(true);
                for (int i = 0; i < top10PerksObject.Count; i++)
                {
                    top10PerksObject[i].transform.GetComponentInChildren<Image>().sprite =
                        perkIconsColorful.Find(res => res.name.ToLower().Contains(top10Perks[i].nama_talent.ToLower()));
                    top10PerksObject[i].transform.GetComponentInChildren<TextMeshProUGUI>().text =
                        top10Perks[i].nama_talent;
                }

                for (int i = 0; i < bottom5PerksObject.Count; i++)
                {
                    bottom5PerksObject[i].transform.GetComponentInChildren<Image>().sprite =
                        perkIconsColorful.Find(res => res.name.ToLower().Contains(bottom5Perks[i].nama_talent.ToLower()));
                    bottom5PerksObject[i].transform.GetComponentInChildren<TextMeshProUGUI>().text =
                        bottom5Perks[i].nama_talent;
                }
                break;
            case "65":
                parent4565.SetActive(true);
                for (int i = 0; i < top10PerksObject.Count; i++)
                {
                    top10PerksObject[i].transform.GetComponentInChildren<Image>().sprite =
                        perkIconsColorful.Find(res => res.name.ToLower().Contains(top10Perks[i].nama_talent.ToLower()));
                    top10PerksObject[i].transform.GetComponentInChildren<TextMeshProUGUI>().text =
                        top10Perks[i].nama_talent;
                }

                for (int i = 0; i < bottom5PerksObject.Count; i++)
                {
                    bottom5PerksObject[i].transform.GetComponentInChildren<Image>().sprite =
                        perkIconsColorful.Find(res => res.name.ToLower().Contains(bottom5Perks[i].nama_talent.ToLower()));
                    bottom5PerksObject[i].transform.GetComponentInChildren<TextMeshProUGUI>().text =
                        bottom5Perks[i].nama_talent;
                }
                break;
        }

        string json = JsonUtility.ToJson(resultData);
        StartCoroutine(
                APIManager.instance.PostDataWithTokenCoroutine(
                    APIManager.instance.SetupSendResultUrl(), json,
                    res => 
                    {

#if UNITY_EDITOR
                        string path = Path.Combine(Application.persistentDataPath, "TalentRanking.json");
                        File.WriteAllText(path, json);
                        Debug.Log($"JSON saved to: {path}");
#endif

                        parentPanel.SetActive(true);
                        StartCoroutine(IEOpenScreen(finalResultPanel.GetComponent<CanvasGroup>(), () => { }));
                        StartCoroutine(IECloseScreen(finalFadePanel.GetComponent<CanvasGroup>(), () => { }));
                        Debug.Log(res);
                    }));
    }

    #region OpenClosePanel
    IEnumerator IEOpenScreen(CanvasGroup screen, Action executeAfter = null)
    {
        while (screen.alpha < 1)
        {
            screen.alpha += Time.deltaTime * 2;
        }

        yield return null;
        executeAfter?.Invoke();
    }

    IEnumerator IECloseScreen(CanvasGroup screen, Action executeAfter = null)
    {
        while (screen.alpha > 0)
        {
            screen.alpha -= Time.deltaTime * 2;
        }

        yield return null;
        executeAfter?.Invoke();
    }
    #endregion
}
