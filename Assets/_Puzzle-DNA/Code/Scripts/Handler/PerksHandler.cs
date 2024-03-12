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
    public class TalentValueData
    {
        public string id;
        public string nama;
        public string deskripsiSingkat;
        public string deskripsiLengkap;
        public int point;
    }

    public GameObject perksPanel;

    [Header("All Perks")]
    public Button[] allPerksButton;
    public List<TalentValueData> perksTypeDatas;

    [Header("UI Attributes")]
    public TextMeshProUGUI perkName;
    public TextMeshProUGUI perkTagline;
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

    public void SetPerksDescription(TalentValueData perkData)
    {
        perkName.text = perkData.nama;
        perkTagline.text = perkData.deskripsiSingkat;
        perkDescription.text = perkData.deskripsiLengkap;
        perkPoint.text = perkData.point.ToString();
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
