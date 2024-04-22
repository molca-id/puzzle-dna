using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ResultValueData
{
    public int id_talent;
    public string nama_talent;
    public int score_talent;
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
    public GameObject finalSubmitPanel;
    public GameObject finalResultPanel;
    public List<Sprite> perkIcons;

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
    public List<UserDataSpace.PerksValueData> rankingPerks;
    public List<UserDataSpace.PerksValueData> top5Perks;
    public List<UserDataSpace.PerksValueData> top10Perks;
    public List<UserDataSpace.PerksValueData> bottom5Perks;

    private void Awake()
    {
        instance = this;
    }

    public void InitFinalSubmitPanel()
    {
        parentPanel.SetActive(true);
        finalSubmitPanel.SetActive(true);
    }

    public void CalculateFinalResult()
    {
        //top 10 perks
        var perksData = DataHandler.instance.GetPerksData().perks_value_datas;
        rankingPerks = perksData.OrderByDescending(x => x.perks_point)
                                   .ThenBy(x => DateTime.TryParse(x.perks_submit_time, out var dt) ? dt : DateTime.MinValue)
                                   .ThenBy(x => x.perks_name)
                                   .ToList();

        bottom5Perks.Clear();
        top5Perks = rankingPerks.Take(5).ToList();
        top10Perks = rankingPerks.Take(10).ToList();
        for (int i = 40; i < rankingPerks.Count; i++)
        {
            bottom5Perks.Add(rankingPerks[i]);
        }

        parentPanel.SetActive(true);
        finalResultPanel.SetActive(true);
        charReplaceSprite.sprite = DataHandler.instance.GetPlayerSprite(expressionType);
        switch (DataHandler.instance.GetUserDataValue().f_report_type)
        {
            case "5":
                parent5.SetActive(true);
                for (int i = 0; i < top5PerksObject.Count; i++)
                {
                    top5PerksObject[i].transform.GetComponentInChildren<Image>().sprite =
                        perkIcons.Find(res => res.name.ToLower().Contains(top10Perks[i].perks_name.ToLower()));
                    top5PerksObject[i].transform.GetComponentInChildren<TextMeshProUGUI>().text =
                        $"{i + 1}. " + top10Perks[i].perks_name;
                }
                break;
            case "10":
                parent10.SetActive(true);
                for (int i = 0; i < topJust10PerksObject.Count; i++)
                {
                    topJust10PerksObject[i].transform.GetComponentInChildren<Image>().sprite =
                        perkIcons.Find(res => res.name.ToLower().Contains(top10Perks[i].perks_name.ToLower()));
                    topJust10PerksObject[i].transform.GetComponentInChildren<TextMeshProUGUI>().text =
                        $"{i + 1}. " + top10Perks[i].perks_name;
                }
                break;
            case "45":
                parent4565.SetActive(true);
                for (int i = 0; i < top10PerksObject.Count; i++)
                {
                    top10PerksObject[i].transform.GetComponentInChildren<Image>().sprite =
                        perkIcons.Find(res => res.name.ToLower().Contains(top10Perks[i].perks_name.ToLower()));
                    top10PerksObject[i].transform.GetComponentInChildren<TextMeshProUGUI>().text =
                        $"{i + 1}. " + top10Perks[i].perks_name;
                }

                for (int i = 0; i < bottom5PerksObject.Count; i++)
                {
                    bottom5PerksObject[i].transform.GetComponentInChildren<Image>().sprite =
                        perkIcons.Find(res => res.name.ToLower().Contains(bottom5Perks[i].perks_name.ToLower()));
                    bottom5PerksObject[i].transform.GetComponentInChildren<TextMeshProUGUI>().text =
                        $"{i + 1}. " + bottom5Perks[i].perks_name;
                }
                break;
            case "65":
                parent4565.SetActive(true);
                for (int i = 0; i < top10PerksObject.Count; i++)
                {
                    top10PerksObject[i].transform.GetComponentInChildren<Image>().sprite =
                        perkIcons.Find(res => res.name.ToLower().Contains(top10Perks[i].perks_name.ToLower()));
                    top10PerksObject[i].transform.GetComponentInChildren<TextMeshProUGUI>().text =
                        $"{i + 1}. " + top10Perks[i].perks_name;
                }

                for (int i = 0; i < bottom5PerksObject.Count; i++)
                {
                    bottom5PerksObject[i].transform.GetComponentInChildren<Image>().sprite =
                        perkIcons.Find(res => res.name.ToLower().Contains(bottom5Perks[i].perks_name.ToLower()));
                    bottom5PerksObject[i].transform.GetComponentInChildren<TextMeshProUGUI>().text =
                        $"{i + 1}. " + bottom5Perks[i].perks_name;
                }
                break;
        }

        List<ResultValueData> datas = new List<ResultValueData>();
        foreach (UserDataSpace.PerksValueData perk in perksData)
        {
            ResultValueData data = new()
            {
                id_talent = Convert.ToInt32(perk.perks_id),
                ranking_talent = rankingPerks.FindIndex(res => res.perks_id == perk.perks_id),
                nama_talent = perk.perks_name,
                score_talent = perk.perks_point
            };

            datas.Add(data);
        }
        resultData.survey_code = DataHandler.instance.GetUserDataValue().game_url;
        resultData.hasil_isi = datas.OrderBy(x => x.id_talent).ToList();

        string json = JsonUtility.ToJson(resultData);
        StartCoroutine(
                APIManager.instance.PostDataWithTokenCoroutine(
                    APIManager.instance.SetupSendResultUrl(), json,
                    res => { Debug.Log("Posted Game Result!"); }));
    }
}
