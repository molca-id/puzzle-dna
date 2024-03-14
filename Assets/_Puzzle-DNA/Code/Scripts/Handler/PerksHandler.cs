using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEditor;

public enum PerksType { Drive, Network, Action }
public enum PerksStage { Stage1, Stage2, Stage3 }

public class PerksHandler : MonoBehaviour
{
    [Serializable]
    public class PerksTypeGroupData
    {
        public PerksType perks_type;
        public List<PerksStageGroupData> perks_stage_datas;
    }

    [Serializable]
    public class PerksStageGroupData
    {
        public PerksStage perks_stage;
        public GameObject lock_panel;
        public List<PerksValueData> perks_value_datas;
    }

    [Serializable]
    public class PerksValueData
    {
        public string perks_name;
        [HideInInspector] public string perks_id;
        [HideInInspector] public string perks_deskripsi_singkat;
        [HideInInspector] public string perks_deskripsi_panjang;
        public int perks_point;
        [HideInInspector] public Button perks_button;
        [HideInInspector] public List<GameObject> perks_background;
    }

    [Serializable]
    public class AbilityUIData
    {
        public PerksType abilityType;
        public GameObject iconBefore;
        public GameObject iconAfter;
    }

    public static PerksHandler instance;

    public int pointPlusUsed;
    public int pointMinusUsed;
    public PerksValueData currentPerk;

    [Header("All Perks")]
    public List<PerksTypeGroupData> perksTypeDatas;

    [Header("UI Attributes")]
    public GameObject perksPanel;
    public GameObject perksDetailPanel;
    [Space]
    public TextMeshProUGUI perkName;
    public TextMeshProUGUI perkTagline;
    public TextMeshProUGUI perkDescription;
    public List<GameObject> perkPointObject;
    [Space]
    public List<TextMeshProUGUI> perkPointPlus;
    public List<TextMeshProUGUI> perkPointMinus;
    public List<AbilityUIData> perkAbilityDatas;

    private void Awake()
    {
        instance = this;
    }

    public void OpenPerksPanel()
    {
        MainMenuHandler.instance.GetTalentPerksFromMenu(delegate
        {
            UserDataSpace.PerksValue perksValue = DataHandler.instance.GetPerksData();
            List<TalentDataSpace.TalentValueData> talentValues = DataHandler.instance.GetTalentDatas();
            UserDataSpace.PerksAbilityData abilityStatus = DataHandler.instance.GetPerksData().perks_ability_data;
            SetPointText();

            foreach (AbilityUIData data in perkAbilityDatas)
            {
                bool upgraded = false;
                data.iconBefore.SetActive(!upgraded);
                data.iconAfter.SetActive(upgraded);

                switch (data.abilityType)
                {
                    case PerksType.Drive:
                        upgraded = abilityStatus.driveUpgraded;
                        break;
                    case PerksType.Network:
                        upgraded = abilityStatus.networkUpgraded;
                        break;
                    case PerksType.Action:
                        upgraded = abilityStatus.actionUpgraded;
                        break;
                }

                data.iconBefore.SetActive(!upgraded);
                data.iconAfter.SetActive(upgraded);
            }

            for (int i = 0; i < perksTypeDatas.Count; i++)
            {
                for (int j = 0; j < perksTypeDatas[i].perks_stage_datas.Count; j++)
                {
                    perksTypeDatas[i].perks_stage_datas[j].lock_panel.
                        SetActive(perksValue.perks_stage_datas[i].perks_stage_locks[j]);

                    for (int k = 0; k < perksTypeDatas[i].perks_stage_datas[j].perks_value_datas.Count; k++)
                    {
                        int index = k;
                        PerksValueData perkTemp = perksTypeDatas[i].perks_stage_datas[j].perks_value_datas[index];
                        UserDataSpace.PerksValueData perk = perksValue.perks_type_datas.Find(perk => perk.perks_name.
                            Contains(perkTemp.perks_name));
                        TalentDataSpace.TalentValueData talent = talentValues.Find(talent => talent.nama.
                            Contains(perkTemp.perks_name));

                        perk.perks_id = talent.id;
                        perkTemp.perks_id = talent.id;
                        perkTemp.perks_deskripsi_panjang = talent.deskripsi.deskripsi.Replace("\n", "");
                        perkTemp.perks_deskripsi_singkat = talent.deskripsi.deskripsi_singkat;

                        if (perkTemp.perks_background.Count == 0)
                        {
                            Transform currButton = perkTemp.perks_button.transform;
                            for (int x = 0; x < currButton.childCount - 1; x++)
                            {
                                perkTemp.perks_background.Add(currButton.GetChild(x).gameObject);
                            }
                        }

                        SetPerksItemUI(perkTemp);
                        perkTemp.perks_button.onClick.AddListener(delegate
                        {
                            currentPerk = perkTemp;
                            SetPerksDescription();
                        });
                    }
                }
            }

            perksPanel.SetActive(true);
        });
    }

    public void ClosePerksDescription()
    {
        perksDetailPanel.SetActive(false);
        DataHandler.instance.GetPerksData().perks_point_data.perks_point_plus += pointPlusUsed;
        DataHandler.instance.GetPerksData().perks_point_data.perks_point_minus += pointMinusUsed;
        
        pointPlusUsed = pointMinusUsed = 0;
        currentPerk.perks_point = DataHandler.instance.GetPerksData().perks_type_datas.
            Find(perk => perk.perks_id == currentPerk.perks_id).perks_point;

        SetPerksItemUI(currentPerk);
        SetPerksDetailUI();
        SetPointText();
    }

    public void AddTalentPoint()
    {
        PerksValueData perk = currentPerk;
        int pivot = perk.perks_point;

        if (currentPerk.perks_point >= 3)
        {
            return;
        }
        else if (currentPerk.perks_point >= pivot)
        {
            if (DataHandler.instance.GetPerksData().perks_point_data.perks_point_plus <= 0)
                return;

            pointPlusUsed++;
            //currentPerk.perks_point++;
            DataHandler.instance.GetPerksData().perks_point_data.perks_point_plus--;
        }
        else if (currentPerk.perks_point < pivot)
        {
            if (DataHandler.instance.GetPerksData().perks_point_data.perks_point_minus <= 0)
                return;

            pointMinusUsed--;
            //currentPerk.perks_point++;
            DataHandler.instance.GetPerksData().perks_point_data.perks_point_minus++;
        }

        SetPerksItemUI(currentPerk);
        SetPerksDetailUI();
        SetPointText();
    }

    public void SubtractTalentPoint()
    {
        PerksValueData perk = currentPerk;
        int pivot = perk.perks_point;

        if (currentPerk.perks_point <= -1)
        {
            return;
        }
        else if (currentPerk.perks_point <= pivot)
        {
            if (DataHandler.instance.GetPerksData().perks_point_data.perks_point_minus <= 0)
                return;

            pointMinusUsed++;
            //currentPerk.perks_point--;
            DataHandler.instance.GetPerksData().perks_point_data.perks_point_minus--;
        }
        else if (currentPerk.perks_point > pivot)
        {
            if (DataHandler.instance.GetPerksData().perks_point_data.perks_point_plus <= 0)
                return;

            pointPlusUsed--;
            //currentPerk.perks_point--;
            DataHandler.instance.GetPerksData().perks_point_data.perks_point_plus++;
        }

        SetPerksItemUI(currentPerk);
        SetPerksDetailUI();
        SetPointText();
    }

    public void SubmitTalentPoint()
    {
        perksDetailPanel.SetActive(false);

        DataHandler.instance.GetPerksData().perks_point_data.perks_point_minus -= pointMinusUsed;
        DataHandler.instance.GetPerksData().perks_point_data.perks_point_plus -= pointPlusUsed;
        DataHandler.instance.GetPerksData().perks_type_datas.
            Find(perk => perk.perks_id == currentPerk.perks_id).
            perks_point = currentPerk.perks_point;

        pointMinusUsed = pointPlusUsed = 0;
    }

    public void SetPerksItemUI(PerksValueData perkTemp)
    {
        perkTemp.perks_background.ForEach(p => p.SetActive(false));
        perkTemp.perks_background[perkTemp.perks_point + 1].SetActive(true);
    }

    public void SetPerksDetailUI()
    {
        perkPointObject.ForEach(perk => perk.SetActive(false));
        perkPointObject[currentPerk.perks_point + 1].SetActive(true);
    }

    public void SetPointText()
    {
        perkPointPlus.ForEach(text => text.text = DataHandler.instance.GetPerksData().perks_point_data.perks_point_plus.ToString());
        perkPointMinus.ForEach(text => text.text = DataHandler.instance.GetPerksData().perks_point_data.perks_point_minus.ToString());
    }

    public void SetPerksDescription()
    {
        perkName.text = currentPerk.perks_name;
        perkTagline.text = currentPerk.perks_deskripsi_singkat;
        perkDescription.text = currentPerk.perks_deskripsi_panjang;
        perksDetailPanel.SetActive(true);
        SetPerksDetailUI();
    }
}
