using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PerksUIData
{
    public Image perkImage;
    public TextMeshProUGUI perkNameText;
}

public class PerksHandler : MonoBehaviour
{
    [Header("All Perks")]
    public PerksValueData[] perksValueDatas;

    [Header("Final Perks")]
    public List<PerksUIData> top10Perks;
    public List<PerksUIData> bottom5Perks;

    public void RandomizeFinalPerks()
    {
        foreach (PerksValueData perk in perksValueDatas)
        {
            perk.perksPoint = UnityEngine.Random.Range(-1, 4);
        }

        DataHandler.instance.GetUserDataValue().perks_value.perksValueDatas = perksValueDatas;
        DataHandler.instance.IEPatchPerksData(() => { });
    }
}
