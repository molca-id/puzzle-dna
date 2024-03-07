using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum PerksType { Drive, Network, Action }

public class PerksHandler : MonoBehaviour
{
    [Serializable]
    public class PerksUIData
    {
        public Image perkImage;
        public TextMeshProUGUI perkNameText;
    }

    [Serializable]
    public class PerksValueData
    {
        public string perksName;
        public PerksType perksType;
        public Sprite perksSprite;
        public int perksPoint;
    }

    [Header("All Perks")]
    public PerksValueData[] perksValueDatas;
    public Button[] allPerksButton;

    [Header("Final Perks")]
    public List<PerksUIData> top10Perks;
    public List<PerksUIData> bottom5Perks;

    void Start()
    {
        DataHandler.instance.GetUserDataValue().perks_value.perksValueDatas.ToList().ForEach(data =>
        {
            PerksValueData founded = perksValueDatas.ToList().Find(perk => perk.perksName == data.perksName);
            founded.perksPoint = data.perksPoint;
        });
    }

    public void RandomizeFinalPerks()
    {
        foreach (PerksValueData perk in perksValueDatas)
        {
            perk.perksPoint = UnityEngine.Random.Range(-1, 4);
        }

        DataHandler.instance.GetUserDataValue().perks_value.perksValueDatas = 
            new UserDataSpace.PerksValueData[perksValueDatas.Length];

        int index = 0;
        perksValueDatas.ToList().ForEach(perk =>
        {
            UserDataSpace.PerksValueData temp = new()
            {
                perksName = perk.perksName,
                perksPoint = perk.perksPoint
            };

            DataHandler.instance.GetUserDataValue().perks_value.perksValueDatas[index] = temp;
            index++;
        });

        DataHandler.instance.IEPatchPerksData(() => { });
    }
}
