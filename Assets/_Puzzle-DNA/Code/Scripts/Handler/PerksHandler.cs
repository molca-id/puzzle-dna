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
        public string perks_id;
        public string perks_deskripsi_singkat;
        public string perks_deskripsi_panjang;
        public int perks_point;
        
        [Space]
        public Button perks_button;
        public List<GameObject> perks_background;
    }

    public static PerksHandler instance;
    public PerksValueData currentPerk;

    [Header("UI Attributes")]
    public GameObject perksPanel;
    public GameObject perksDetailPanel;
    [Space]
    public TextMeshProUGUI perkName;
    public TextMeshProUGUI perkTagline;
    public TextMeshProUGUI perkDescription;
    public TextMeshProUGUI perkPoint;
    [Space]
    public TextMeshProUGUI perkPointPlus;
    public TextMeshProUGUI perkPointMinus;

    [Header("All Perks")]
    public List<PerksTypeGroupData> perksTypeDatas;

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

            perkPointPlus.text = DataHandler.instance.GetPerksData().perks_point_plus.ToString();
            perkPointMinus.text = DataHandler.instance.GetPerksData().perks_point_minus.ToString();

            for (int i = 0; i < perksTypeDatas.Count; i++)
            {
                for (int j = 0; j < perksTypeDatas[i].perks_stage_datas.Count; j++)
                {
                    perksTypeDatas[i].perks_stage_datas[j].lock_panel.
                        SetActive(perksValue.perks_stage_datas[i].perks_stage_locks[j]);

                    for (int k = 0; k < perksTypeDatas[i].perks_stage_datas[j].perks_value_datas.Count; k++)
                    {
                        int index = k;
                        PerksValueData currPerk = perksTypeDatas[i].perks_stage_datas[j].perks_value_datas[index];
                        UserDataSpace.PerksValueData perk = perksValue.perks_type_datas.Find(perk => perk.perks_name.
                            Contains(perksTypeDatas[i].perks_stage_datas[j].perks_value_datas[index].perks_name));
                        TalentDataSpace.TalentValueData talent = talentValues.Find(talent => talent.nama.
                            Contains(perksTypeDatas[i].perks_stage_datas[j].perks_value_datas[index].perks_name));

                        perk.perks_id = talent.id;
                        currPerk.perks_id = talent.id;
                        currPerk.perks_deskripsi_panjang = talent.deskripsi.deskripsi.Replace("\n", "");
                        currPerk.perks_deskripsi_singkat = talent.deskripsi.deskripsi_singkat;
                        currPerk.perks_point = perk.perks_point;

                        if (currPerk.perks_background.Count == 0)
                        {
                            Transform currButton = currPerk.perks_button.transform;
                            for (int x = 0; x < currButton.childCount; x++)
                            {
                                currPerk.perks_background.Add(currButton.GetChild(x).gameObject);
                            }
                        }

                        currPerk.perks_background.ForEach(p => p.SetActive(false));
                        currPerk.perks_background[currPerk.perks_point + 1].SetActive(true);
                        currPerk.perks_button.onClick.AddListener(delegate
                        {
                            currentPerk = currPerk;
                            SetPerksDescription();
                        });
                    }
                }
            }

            perksPanel.SetActive(true);
        });
    }

    public void SetPerksDescription()
    {
        perkName.text = currentPerk.perks_name;
        perkTagline.text = currentPerk.perks_deskripsi_singkat;
        perkDescription.text = currentPerk.perks_deskripsi_panjang;
        perkPoint.text = currentPerk.perks_point.ToString();
        perksDetailPanel.SetActive(true);
    }
}
