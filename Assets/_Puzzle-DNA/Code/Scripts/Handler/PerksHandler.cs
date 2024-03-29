using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UserDataSpace;
using UnityEngine.Events;

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
        public int perks_point;
        [HideInInspector] public string perks_id;
        [HideInInspector] public string perks_deskripsi_singkat;
        [HideInInspector] public string perks_deskripsi_panjang;
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

    [HideInInspector] public int pivotPoint;
    [HideInInspector] public int plusPointUsed;
    [HideInInspector] public int minusPointUsed;
    [HideInInspector] public PerksValueData currentPerk;

    [Header("All Perks")]
    public List<PerksTypeGroupData> perksTypeDatas;

    [Header("UI Attributes")]
    public GameObject perksPanel;
    public GameObject perksDetailPanel;
    public Button submitButton;
    [Space]
    public GameObject driveDescPanel;
    public GameObject networkDescPanel;
    public GameObject actionDescPanel;
    [Space]
    public TextMeshProUGUI perkName;
    public TextMeshProUGUI perkTagline;
    public TextMeshProUGUI perkDescription;
    public List<GameObject> perkPointObject;
    [Space]
    public List<TextMeshProUGUI> perkPointPlus;
    public List<TextMeshProUGUI> perkPointMinus;
    public List<AbilityUIData> perkAbilityDatas;

    [Header("Addon For Perks Panel As Tutorial")]
    public bool asTutorial;
    public UnityEvent whenSubmitPerk;

    private void Awake()
    {
        instance = this;
    }

    public void SetDNADescriptionState(int index)
    {
        if (index == 0) driveDescPanel.SetActive(!driveDescPanel.activeSelf); 
        else if (index == 1) networkDescPanel.SetActive(!networkDescPanel.activeSelf);
        else actionDescPanel.SetActive(!actionDescPanel.activeSelf);
    }

    public void OpenPerksPanel(bool isSmall)
    {
        MainMenuHandler.instance.GetTalentPerksFromMenu(isSmall, delegate
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
                        UserDataSpace.PerksValueData perk = perksValue.perks_value_datas.Find(perk => perk.perks_name.
                            Contains(perkTemp.perks_name));
                        TalentDataSpace.TalentValueData talent = talentValues.Find(talent => talent.nama.
                            Contains(perkTemp.perks_name));

                        perk.perks_id = talent.id;
                        perkTemp.perks_id = talent.id;
                        perkTemp.perks_deskripsi_panjang = talent.deskripsi.deskripsi.Replace("\n", "");
                        perkTemp.perks_deskripsi_singkat = talent.deskripsi.deskripsi_singkat;
                        perkTemp.perks_point = perk.perks_point;

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
                            OpenPerksDescription();
                        });
                    }
                }
            }

            perksPanel.SetActive(true);
        });
    }

    public void OpenPerksDescription()
    {
        pivotPoint = currentPerk.perks_point;
        perkName.text = currentPerk.perks_name;
        perkTagline.text = currentPerk.perks_deskripsi_singkat;
        perkDescription.text = currentPerk.perks_deskripsi_panjang;
        
        perksDetailPanel.SetActive(true);
        submitButton.interactable = false;
        SetPerksDetailUI();
    }

    public void ClosePerksDescription()
    {
        DataHandler.instance.GetPerksData().perks_point_data.perks_point_plus += plusPointUsed;
        DataHandler.instance.GetPerksData().perks_point_data.perks_point_minus += minusPointUsed;
        
        currentPerk.perks_point = DataHandler.instance.GetPerksData().perks_value_datas.
            Find(perk => perk.perks_id == currentPerk.perks_id).perks_point;

        plusPointUsed = minusPointUsed = 0;
        perksDetailPanel.SetActive(false);

        SetPerksItemUI(currentPerk);
        SetPerksDetailUI();
        SetPointText();
    }

    public void ResetAllPerks()
    {
        //saving data in temporary
        int totalProton = DataHandler.instance.GetPerksData().perks_point_data.total_perks_point_plus;
        int totalElectron = DataHandler.instance.GetPerksData().perks_point_data.total_perks_point_minus;

        bool driveUpgraded = DataHandler.instance.GetPerksData().perks_ability_data.driveUpgraded;
        bool networkUpgraded = DataHandler.instance.GetPerksData().perks_ability_data.networkUpgraded;
        bool actionUpgraded = DataHandler.instance.GetPerksData().perks_ability_data.actionUpgraded;

        List<bool> driveStage = DataHandler.instance.GetPerksData().perks_stage_datas[0].perks_stage_locks;
        List<bool> networkStage = DataHandler.instance.GetPerksData().perks_stage_datas[1].perks_stage_locks;
        List<bool> actionStage = DataHandler.instance.GetPerksData().perks_stage_datas[2].perks_stage_locks;

        //resetting
        DataHandler.instance.ResetAllPerks();

        //after reset
        DataHandler.instance.GetPerksData().perks_point_data.perks_point_minus =
            DataHandler.instance.GetPerksData().perks_point_data.total_perks_point_minus =
            totalElectron;
        DataHandler.instance.GetPerksData().perks_point_data.perks_point_plus =
            DataHandler.instance.GetPerksData().perks_point_data.total_perks_point_plus =
            totalProton;

        DataHandler.instance.GetPerksData().perks_ability_data.driveUpgraded = driveUpgraded;
        DataHandler.instance.GetPerksData().perks_ability_data.networkUpgraded = networkUpgraded;
        DataHandler.instance.GetPerksData().perks_ability_data.actionUpgraded = actionUpgraded;

        DataHandler.instance.GetPerksData().perks_stage_datas[0].perks_stage_locks = driveStage;
        DataHandler.instance.GetPerksData().perks_stage_datas[1].perks_stage_locks = networkStage;
        DataHandler.instance.GetPerksData().perks_stage_datas[2].perks_stage_locks = actionStage;

        MainMenuHandler.instance.PatchPerksFromMenu(delegate
        {
            SetPerksItemUI();
            SetPerksDetailUI();
            SetPointText();
        });
    }

    public void SubmitTalentPoint()
    {
        UserDataSpace.PerksValueData currPerk = DataHandler.instance.GetPerksData().perks_value_datas.
            Find(perk => perk.perks_id == currentPerk.perks_id);
        currPerk.perks_point = currentPerk.perks_point;
        currPerk.perks_submit_time = DateTime.Now.ToString();

        MainMenuHandler.instance.PatchPerksFromMenu(delegate
        {
            SetPerksItemUI(currentPerk);
            SetPerksDetailUI();
            SetPointText();

            perksDetailPanel.SetActive(false);
            minusPointUsed = plusPointUsed = 0;
            currentPerk = new();

            if (!asTutorial) return;
            whenSubmitPerk.Invoke();
        });
    }

    public void AddTalentPoint()
    {
        if (currentPerk.perks_point == 3)
        {
            return;
        }

        if (currentPerk.perks_point >= pivotPoint &&
            DataHandler.instance.GetPerksData().perks_point_data.perks_point_plus > 0)
        {
            DataHandler.instance.GetPerksData().perks_point_data.perks_point_plus--;

            plusPointUsed++;
            currentPerk.perks_point++;
        }
        else if (currentPerk.perks_point < pivotPoint)
        {
            DataHandler.instance.GetPerksData().perks_point_data.perks_point_minus++;

            if (minusPointUsed > 0) minusPointUsed--;
            currentPerk.perks_point++;
        }

        if (currentPerk.perks_point > pivotPoint ||
            currentPerk.perks_point < pivotPoint)
        {
            submitButton.interactable = true;
        }
        else
        {
            submitButton.interactable = false;
        }

        SetPerksItemUI(currentPerk);
        SetPerksDetailUI();
        SetPointText();
    }

    public void SubtractTalentPoint()
    {
        if (currentPerk.perks_point == -1)
        {
            return;
        }

        if (currentPerk.perks_point > pivotPoint)
        {
            DataHandler.instance.GetPerksData().perks_point_data.perks_point_plus++;

            if (plusPointUsed > 0) plusPointUsed--;
            currentPerk.perks_point--;
        }
        else if (currentPerk.perks_point <= pivotPoint &&
            DataHandler.instance.GetPerksData().perks_point_data.perks_point_minus > 0)
        {
            DataHandler.instance.GetPerksData().perks_point_data.perks_point_minus--;

            minusPointUsed++;
            currentPerk.perks_point--;
        }

        if (currentPerk.perks_point > pivotPoint ||
            currentPerk.perks_point < pivotPoint)
        {
            submitButton.interactable = true;
        }
        else
        {
            submitButton.interactable = false;
        }

        SetPerksItemUI(currentPerk);
        SetPerksDetailUI();
        SetPointText();
    }

    public void SetPerksItemUI(PerksValueData currentPerk = null)
    {
        if (currentPerk != null)
        {
            currentPerk.perks_background.ForEach(p => p.SetActive(false));
            currentPerk.perks_background[currentPerk.perks_point + 1].SetActive(true);
        }
        else
        {
            UserDataSpace.PerksValue perksValue = DataHandler.instance.GetPerksData();
            UserDataSpace.PerksAbilityData abilityStatus = DataHandler.instance.GetPerksData().perks_ability_data;
            List<TalentDataSpace.TalentValueData> talentValues = DataHandler.instance.GetTalentDatas();
            
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
                        UserDataSpace.PerksValueData perk = perksValue.perks_value_datas.Find(perk => perk.perks_name.
                            Contains(perkTemp.perks_name));
                        TalentDataSpace.TalentValueData talent = talentValues.Find(talent => talent.nama.
                            Contains(perkTemp.perks_name));

                        perk.perks_point = 0;
                        perk.perks_id = talent.id;

                        perkTemp.perks_id = talent.id;
                        perkTemp.perks_deskripsi_panjang = talent.deskripsi.deskripsi.Replace("\n", "");
                        perkTemp.perks_deskripsi_singkat = talent.deskripsi.deskripsi_singkat;
                        perkTemp.perks_point = perk.perks_point;

                        if (perkTemp.perks_background.Count == 0)
                        {
                            Transform currButton = perkTemp.perks_button.transform;
                            for (int x = 0; x < currButton.childCount - 1; x++)
                            {
                                perkTemp.perks_background.Add(currButton.GetChild(x).gameObject);
                            }
                        }

                        SetPerksItemUI(perkTemp);
                    }
                }
            }
        }
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
}
