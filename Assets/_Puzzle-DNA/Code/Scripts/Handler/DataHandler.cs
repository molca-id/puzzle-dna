using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

#region Player Sprite
public enum ExpressionType { Netral, Senang, Optimis, Sedih, Unknown }

[Serializable]
public class PlayerSpriteExpressionData
{
    public ExpressionType expressionType;
    public Sprite sprite;
}

[Serializable]
public class PlayerSpriteData
{
    public string name;
    public UserDataSpace.Character character;
    public List<AudioClip> playerClips;
    public List<PlayerSpriteExpressionData> expressionDatas;
}
#endregion

#region Talent Data
[Serializable]
public class TalentData
{
    public string talent_name;
    public List<AssessmentValue> assessment_values;
}

[Serializable]
public class AssessmentValue
{
    public long assessment_id;
    public string assessment_description;
    public string assessment_vo;
}

[System.Serializable]
public class TalentGroupData
{
    public bool isDone;
    public List<long> assessment_ids;
}

[System.Serializable]
public class TalentGroupDataWrapper
{
    public List<TalentGroupData> talentGroupDatas;
}
#endregion

#region Talent Raw Data
[Serializable]
public class ItemData
{
    public long id;
    public string item;
    public string vo;
}

[Serializable]
public class CategoryData
{
    public List<ItemData> COMPETITIVE;
    public List<ItemData> DIRECTIVE;
    public List<ItemData> GOAL_GETTER;
    public List<ItemData> OPTIMIZER;
    public List<ItemData> PERFECTIONIST;
    public List<ItemData> SELF_CONFIDENT;
    public List<ItemData> SIGNIFICANT;
    public List<ItemData> AVERSIVE;
    public List<ItemData> COLLECTOR;
    public List<ItemData> CONTEMPLATIVE;
    public List<ItemData> EQUITABLE;
    public List<ItemData> EXPLORER;
    public List<ItemData> NOBLE;
    public List<ItemData> VIGOROUS;
    public List<ItemData> VISIONARY;
    public List<ItemData> ADVISOR;
    public List<ItemData> ARTICULATIVE;
    public List<ItemData> COLLABORATOR;
    public List<ItemData> COURAGEOUS;
    public List<ItemData> CONVINCING;
    public List<ItemData> DEVELOPER;
    public List<ItemData> ENERGIZER;
    public List<ItemData> AFFECTIONATE;
    public List<ItemData> CARING;
    public List<ItemData> FORGIVING;
    public List<ItemData> GENEROUS;
    public List<ItemData> GENUINE;
    public List<ItemData> HARMONY;
    public List<ItemData> PERSONALIZER;
    public List<ItemData> SOICABLE;
    public List<ItemData> CONTEXTUAL;
    public List<ItemData> FOCUSED;
    public List<ItemData> INTUITIVE;
    public List<ItemData> INNOVATIVE;
    public List<ItemData> LOGICAL;
    public List<ItemData> STRATEGIZER;
    public List<ItemData> TROUBLESHOOTER;
    public List<ItemData> ACCOUNTABLE;
    public List<ItemData> AUTHORITATIVE;
    public List<ItemData> DECISIVE;
    public List<ItemData> FIXER;
    public List<ItemData> FLEXIBLE;
    public List<ItemData> INITIATOR;
    public List<ItemData> RESOURCEFUL;
    public List<ItemData> STRUCTURED;
}

[Serializable]
public class RawData
{
    public int status;
    public CategoryData data;
}
#endregion

public class DataHandler : MonoBehaviour
{
    public static DataHandler instance;
    [HideInInspector] public ValidateData validateData;

    [Header("Default Datas")]
    public UserDataSpace.UserData defaultUserData;
    public TalentDataSpace.TalentData talentData;
    public List<PlayerSpriteData> playerAssetDatas;

    [Header("Current Datas")]
    public UserDataSpace.UserData currentUserData;
    public PlayerSpriteData currPlayerAssetData;

    [Header("Talent Data")]
    public List<TalentData> talentDatas;
    public List<TalentGroupData> talentGroupDatas;

    [Header("Another Attributes")]
    public AudioMixer bgmAudioMixer;
    public AudioMixer sfxAudioMixer;
    public AudioMixer voAudioMixer;
    public List<LanguageHandler> languageHandlers;
    public List<LevelData> levelDatas;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

#region Talent Raw Data Processing
    public void InitializeTalentList()
    {
        string[] allTalents = new string[]
        {
            "Competitive", "Directive", "Goal-Getter", "Optimizer", "Perfectionist", "Self-Confident", "Significant", "Aversive",
            "Collector", "Contemplative", "Equitable", "Explorer", "Noble", "Vigorous", "Visionary", "Advisor", "Articulative",
            "Collaborator", "Courageous", "Convincing", "Developer", "Energizer", "Affectionate", "Caring", "Forgiving", "Generous",
            "Genuine", "Harmony", "Personalizer", "Soicable", "Contextual", "Focused", "Intuitive", "Innovative", "Logical",
            "Strategizer", "Troubleshooter", "Accountable", "Authoritative", "Decisive", "Fixer", "Flexible", "Initiator",
            "Resourceful", "Structured"
        };

        talentDatas = new List<TalentData>();

        foreach (var talentName in allTalents)
        {
            talentDatas.Add(new TalentData
            {
                talent_name = talentName,
                assessment_values = new List<AssessmentValue>()
            });
        }
    }

    public void UpdateTalentDataFromJson(RawData rawData)
    {
        foreach (var talent in talentDatas)
        {
            talent.assessment_values.Clear();
        }

        var categoryData = rawData.data;

        MapTalent("Competitive", categoryData.COMPETITIVE);
        MapTalent("Directive", categoryData.DIRECTIVE);
        MapTalent("Goal-Getter", categoryData.GOAL_GETTER);
        MapTalent("Optimizer", categoryData.OPTIMIZER);
        MapTalent("Perfectionist", categoryData.PERFECTIONIST);
        MapTalent("Self-Confident", categoryData.SELF_CONFIDENT);
        MapTalent("Significant", categoryData.SIGNIFICANT);
        MapTalent("Aversive", categoryData.AVERSIVE);
        MapTalent("Collector", categoryData.COLLECTOR);
        MapTalent("Contemplative", categoryData.CONTEMPLATIVE);
        MapTalent("Equitable", categoryData.EQUITABLE);
        MapTalent("Explorer", categoryData.EXPLORER);
        MapTalent("Noble", categoryData.NOBLE);
        MapTalent("Vigorous", categoryData.VIGOROUS);
        MapTalent("Visionary", categoryData.VISIONARY);
        MapTalent("Advisor", categoryData.ADVISOR);
        MapTalent("Articulative", categoryData.ARTICULATIVE);
        MapTalent("Collaborator", categoryData.COLLABORATOR);
        MapTalent("Courageous", categoryData.COURAGEOUS);
        MapTalent("Convincing", categoryData.CONVINCING);
        MapTalent("Developer", categoryData.DEVELOPER);
        MapTalent("Energizer", categoryData.ENERGIZER);
        MapTalent("Affectionate", categoryData.AFFECTIONATE);
        MapTalent("Caring", categoryData.CARING);
        MapTalent("Forgiving", categoryData.FORGIVING);
        MapTalent("Generous", categoryData.GENEROUS);
        MapTalent("Genuine", categoryData.GENUINE);
        MapTalent("Harmony", categoryData.HARMONY);
        MapTalent("Personalizer", categoryData.PERSONALIZER);
        MapTalent("Soicable", categoryData.SOICABLE);
        MapTalent("Contextual", categoryData.CONTEXTUAL);
        MapTalent("Focused", categoryData.FOCUSED);
        MapTalent("Intuitive", categoryData.INTUITIVE);
        MapTalent("Innovative", categoryData.INNOVATIVE);
        MapTalent("Logical", categoryData.LOGICAL);
        MapTalent("Strategizer", categoryData.STRATEGIZER);
        MapTalent("Troubleshooter", categoryData.TROUBLESHOOTER);
        MapTalent("Accountable", categoryData.ACCOUNTABLE);
        MapTalent("Authoritative", categoryData.AUTHORITATIVE);
        MapTalent("Decisive", categoryData.DECISIVE);
        MapTalent("Fixer", categoryData.FIXER);
        MapTalent("Flexible", categoryData.FLEXIBLE);
        MapTalent("Initiator", categoryData.INITIATOR);
        MapTalent("Resourceful", categoryData.RESOURCEFUL);
        MapTalent("Structured", categoryData.STRUCTURED);
    }

    void MapTalent(string talentName, List<ItemData> items)
    {
        TalentData talent = talentDatas.Find(t => t.talent_name == talentName);
    
        if (talent != null && items != null)
        {
            foreach (var item in items)
            {
                talent.assessment_values.Add(new AssessmentValue
                {
                    assessment_id = item.id,
                    assessment_description = item.item,
                    assessment_vo = item.vo
                });
            }
        }
    }
#endregion

    public Sprite GetPlayerSprite(ExpressionType expressionType)
    {
        return currPlayerAssetData.expressionDatas.
            Find(exp => exp.expressionType == expressionType).sprite;
    }

    public string GetCharacterName() => currPlayerAssetData.name;

    public AudioClip GetPlayerClip(string clipCode)
    {
        return currPlayerAssetData.playerClips.
            Find(exp => exp.name.Contains(clipCode));
    }

    public AudioHandler GetAudioHandler(string key)
    {
        return FindObjectsOfType<AudioHandler>().ToList().
            Find(audio => audio.audioGroupKey == key);
    }

    public void RefreshAllTextLanguage()
    {
        languageHandlers = FindObjectsOfType<LanguageHandler>().ToList();
        foreach (var item in languageHandlers)
        {
            item.SetContentByLanguage();
        }
    }

    public void IECreateUserData()
    {
        defaultUserData.data.game_url = SessionCodeHooker.instance.GetSessionCode();
        string json = JsonUtility.ToJson(defaultUserData.data);

        PreloadManager.instance.SetLoadingText("Creating Player Data");

        //hitting api
        StartCoroutine(
            APIManager.instance.PostDataCoroutine(
                APIManager.instance.SetupGameUrl(),
                json, res =>
                {
                    IEGetUserData();
                }));
    }

    public void IEPatchAllVolumeData()
    {
        bgmAudioMixer.GetFloat("MasterVolume", out float bgm);
        sfxAudioMixer.GetFloat("MasterVolume", out float sfx);
        voAudioMixer.GetFloat("MasterVolume", out float vo);
        string json = "{ \"bgm_value\":" + bgm + ", \"sfx_value\":" + sfx + ", \"vo_value\":" + vo + " }";

        //hitting api
        StartCoroutine(
            APIManager.instance.PatchDataCoroutine(
                APIManager.instance.SetupGameUrl(GetUniqueCode()),
                json, res => { }));
    }

    public void IEPatchCharacterData(Action executeAfter = null)
    {
        string json = "{ \"character\" : " + GetUserDataValue().character + " }";

        //hitting api
        StartCoroutine(
            APIManager.instance.PatchDataCoroutine(
                APIManager.instance.SetupGameUrl(GetUniqueCode()),
                json, res => executeAfter.Invoke()));
    }

    public void IEPatchLanguageData(Action executeAfter = null)
    {
        string json = "{ \"language\" : \"" + GetLanguage() + "\" }";

        //hitting api
        StartCoroutine(
            APIManager.instance.PatchDataCoroutine(
                APIManager.instance.SetupGameUrl(GetUniqueCode()),
                json, res => executeAfter.Invoke()));
    }

    public void IEPatchCheckpointData(Action executeAfter = null)
    {
        string json = "{ \"checkpoint_data\" : " + JsonUtility.ToJson(GetUserCheckpointData()) + "}";

        //hitting api
        StartCoroutine(
            APIManager.instance.PatchDataCoroutine(
                APIManager.instance.SetupGameUrl(GetUniqueCode()),
                json, res => executeAfter.Invoke()));
    }

    public void IEValidateGameSession()
    {
        PreloadManager.instance.SetLoadingText("Validating Player Data");

        //hitting api
        StartCoroutine(
            APIManager.instance.GetDataCoroutine(
                APIManager.instance.SetupValidateUrl(
                    SessionCodeHooker.instance.GetSessionCode()),
                res =>
                {
                    validateData = JsonUtility.FromJson<ValidateData>(res);
                    PreloadManager.instance.SetValidState(validateData.success);
                }));
    }

    public void IEGetItemData()
    {
        //hitting api
        StartCoroutine(
            APIManager.instance.GetDataWithTokenCoroutine(
                APIManager.instance.SetupGetItemUrl(),
                res=>
                {
                    RawData rawData = JsonUtility.FromJson<RawData>(res);
                    InitializeTalentList();
                    UpdateTalentDataFromJson(rawData);
                }));
    }

    public void IEGetTalentData(Action executeAfter = null)
    {
        //hitting api
        StartCoroutine(
            APIManager.instance.GetDataCoroutine(
                APIManager.instance.SetupTalentPerksUrl(GetUserDataValue().language),
                res =>
                {
                    talentData = JsonUtility.FromJson<TalentDataSpace.TalentData>(res);
                    executeAfter.Invoke();
                }));
    }

    public void IEGetUserData()
    {
        PreloadManager.instance.SetLoadingText("Getting Player Data");

        //hitting api
        StartCoroutine(
            APIManager.instance.GetDataCoroutine(
                APIManager.instance.SetupGameUrl(
                    SessionCodeHooker.instance.GetSessionCode()),
                res =>
                {
                    currentUserData = JsonUtility.FromJson<UserDataSpace.UserData>(res);
                    bgmAudioMixer.SetFloat("MasterVolume", GetUserDataValue().bgm_value);
                    sfxAudioMixer.SetFloat("MasterVolume", GetUserDataValue().sfx_value);
                    voAudioMixer.SetFloat("MasterVolume", GetUserDataValue().vo_value);

                    if (!currentUserData.success) 
                        IECreateUserData();
                    else
                    {
                        currPlayerAssetData = playerAssetDatas.Find(data => (int)data.character == GetUserDataValue().character);
                        //SetupPlayerSprites();
                    }
                }));
    }

    public List<TalentDataSpace.TalentValueData> GetTalentDatas() => talentData.data;
    
    public UserDataSpace.UserDataValue GetUserDataValue() => currentUserData.data;

    public UserDataSpace.CheckpointData GetUserCheckpointData() => currentUserData.data.checkpoint_data;

    public string GetUniqueCode() => GetUserDataValue().game_url;

    public string GetLanguage() => GetUserDataValue().language;
}
