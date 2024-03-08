using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum PerksType { Drive, Network, Action }
public enum PerksStage { Stage1, Stage2, Stage3 }

public class PerksHandler : MonoBehaviour
{
    [Serializable]
    public class PerksUIData
    {
        public Image perkImage;
        public TextMeshProUGUI perkNameText;
    }

    [Serializable]
    public class PerksSpriteData
    {
        public PerksType perksType;
        public List<Sprite> perksSprites;
    }

    [Serializable]
    public class PerksValueData
    {
        public string perksName;
        public Sprite perksSprite;
        public int perksPoint;
        [TextArea(2, 2)] public string perksDescription;
    }

    [Serializable]
    public class PerksValue
    {
        public PerksType perksType;
        public PerksStage perksStage;
        public GameObject perksLockPanel;
        public bool isLock;
        public List<PerksValueData> perksValueDatas;
    }

    public GameObject perksPanel;

    [Header("All Perks")]
    public Button[] allPerksButton;
    public PerksValue[] perksValues;
    public List<PerksSpriteData> perksSpriteDatas;

    [Header("Final Perks")]
    public List<PerksUIData> top10Perks;
    public List<PerksUIData> bottom5Perks;

    [Header("UI Attributes")]
    public TextMeshProUGUI perkName;
    public TextMeshProUGUI perkDescription;
    public TextMeshProUGUI perkPoint;

    void Start()
    {
        //DataHandler.instance.GetUserDataValue().perks_value.perksValueDatas.ToList().ForEach(data =>
        //{
        //    PerksValueData founded = perksValueDatas.ToList().Find(perk => perk.perksName == data.perksName);
        //    founded.perksPoint = data.perksPoint;
        //});

        //SetPerksDescription(perksValueDatas[0]);
        //UpdatePerksDetails();
    }

    public void OpenPerksPanel()
    {
        MainMenuHandler.instance.GetTalentPerksFromMenu(delegate
        {
            perksPanel.SetActive(true);
        });
    }

    public void UpdatePerksDetails()
    {
        //for (int i = 0; i < perksValueDatas.Length; i++)
        //{
        //    int index = i;
        //    Button perkButton = allPerksButton[index];
        //    PerksValueData perkData = perksValueDatas[index];
        //    PerksSpriteData spriteData = perksSpriteDatas.Find(sprite => sprite.perksType == perkData.perksType);
            
        //    perkButton.GetComponent<Image>().sprite = spriteData.perksSprites[perkData.perksPoint + 1];
        //    perkButton.onClick.AddListener(delegate
        //    {
        //        SetPerksDescription(perkData);
        //    });
        //}
    }

    public void SetPerksDescription(PerksValueData perkData)
    {
        perkName.text = perkData.perksName;
        perkDescription.text = perkData.perksDescription;
        perkPoint.text = perkData.perksPoint.ToString();
    }

    public void RandomizeFinalPerks()
    {
        //foreach (PerksValueData perk in perksValueDatas)
        //{
        //    perk.perksPoint = UnityEngine.Random.Range(-1, 4);
        //}

        //DataHandler.instance.GetUserDataValue().perks_value.perksValueDatas = 
        //    new UserDataSpace.PerksValueData[perksValueDatas.Length];

        //int index = 0;
        //perksValueDatas.ToList().ForEach(perk =>
        //{
        //    UserDataSpace.PerksValueData temp = new()
        //    {
        //        perksName = perk.perksName,
        //        perksPoint = perk.perksPoint
        //    };

        //    DataHandler.instance.GetUserDataValue().perks_value.perksValueDatas[index] = temp;
        //    index++;
        //});

        //DataHandler.instance.IEPatchPerksData(() => { });
        //UpdatePerksDetails();
    }
}
