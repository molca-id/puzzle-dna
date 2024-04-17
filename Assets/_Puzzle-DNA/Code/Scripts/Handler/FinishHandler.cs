using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("Perks UI Ranking")]
    public List<GameObject> top10PerksObject;
    public List<GameObject> bottom5PerksObject;

    [Header("Perks Ranking")]
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
        var topSortedPerks = perksData.OrderByDescending(x => x.perks_point)
                                   .ThenBy(x => DateTime.TryParse(x.perks_submit_time, out var dt) ? dt : DateTime.MinValue)
                                   .ThenBy(x => x.perks_name)
                                   .ToList();

        var bottomSortedPerks = perksData.OrderBy(x => x.perks_point)
                                   .ThenBy(x => DateTime.TryParse(x.perks_submit_time, out var dt) ? dt : DateTime.MinValue)
                                   .ThenBy(x => x.perks_name)
                                   .ToList();

        top10Perks = topSortedPerks.Take(10).ToList();
        bottom5Perks = bottomSortedPerks.Take(5).ToList();

        parentPanel.SetActive(true);
        finalResultPanel.SetActive(true);
        charReplaceSprite.sprite = DataHandler.instance.GetPlayerSprite(expressionType);

        for (int i = 0; i < top10PerksObject.Count; i++)
        {
            top10PerksObject[i].transform.GetComponentInChildren<Image>().sprite = 
                perkIcons.Find(res => res.name.ToLower().Contains(top10Perks[i].perks_name.ToLower()));
            top10PerksObject[i].transform.GetComponentInChildren<TextMeshProUGUI>().text =
                top10Perks[i].perks_name;
        }

        for (int i = 0; i < bottom5PerksObject.Count; i++)
        {
            bottom5PerksObject[i].transform.GetComponentInChildren<Image>().sprite =
                perkIcons.Find(res => res.name.ToLower().Contains(bottom5Perks[i].perks_name.ToLower()));
            bottom5PerksObject[i].transform.GetComponentInChildren<TextMeshProUGUI>().text =
                bottom5Perks[i].perks_name;
        }
    }
}
