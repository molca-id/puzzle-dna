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

[Serializable]
public class TalentGroupDataWrapper
{
    public List<TalentGroupData> talentGroupDatas;
}

[Serializable]
public class TalentGroupData
{
    public List<TalentGroupValue> assessmentValues;
}

[Serializable]
public class TalentGroupValue
{
    public long assessment_id;
    public float assessment_score;
}

[System.Serializable]
public class AssessmentContainer
{
    public List<TalentGroupValue> assessment_values;
}

[System.Serializable]
public class Wrapper<T>
{
    public List<T> error; // Hanya sebagai pembungkus untuk JSON "error"
}
#endregion
#region Talent Raw Data
[Serializable]
public class RawData
{
    public int status;
    public List<CategoryData> data;
}

[Serializable]
public class CategoryData
{
    public int id_talent;
    public string nama_talent;
    public float score_talent;
    public List<ItemData> item_talent;
}

[Serializable]
public class ItemData
{
    public long id;
    public string item;
    public string vo;
    public float score;
}
#endregion

public class DataHandler : MonoBehaviour
{
    public static DataHandler instance;
    [HideInInspector] public ValidateData validateData;

    [Header("Default Datas")]
    public UserDataSpace.UserData defaultUserData;
    public List<PlayerSpriteData> defaultPlayerAssetDatas;

    [Header("Current Datas")]
    public UserDataSpace.UserData currentUserData;
    public PlayerSpriteData currentPlayerAssetData;

    [Header("Talent Data")]
    public RawData talentDatas;
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

    public void SetupPlayerTalentData()
    {
        foreach (var talentGroup in talentGroupDatas)
        {
            foreach (var assessment in talentGroup.assessmentValues)
            {
                bool assessmentExists = false;
                foreach (var perk in currentUserData.data.assessment_values)
                {
                    if (perk.assessment_id == assessment.assessment_id)
                    {
                        assessmentExists = true;
                        break;
                    }
                }

                if (!assessmentExists)
                {
                    currentUserData.data.assessment_values.Add(new TalentGroupValue
                    {
                        assessment_id = assessment.assessment_id,
                        assessment_score = assessment.assessment_score
                    });

                    currentUserData.data.assessment_values.
                        Sort((a, b) => a.assessment_id.CompareTo(b.assessment_id));
                }
            }
        }
    }

#region Setter Getter Data
    public Sprite GetPlayerSprite(ExpressionType expressionType)
    {
        return currentPlayerAssetData.expressionDatas.
            Find(exp => exp.expressionType == expressionType).sprite;
    }

    public string GetCharacterName() => currentPlayerAssetData.name;

    public AudioClip GetPlayerClip(string clipCode)
    {
        return currentPlayerAssetData.playerClips.
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
#endregion
#region API
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
    
    public void IEPatchPerksValue(Action executeAfter = null)
    {
        AssessmentContainer container = new AssessmentContainer { assessment_values = GetUserAssessmentData() };
        string json = JsonUtility.ToJson(container);

        //hitting api
        StartCoroutine(
            APIManager.instance.PatchDataCoroutine(
                APIManager.instance.SetupGameUrl(GetUniqueCode()), 
                json, res => executeAfter?.Invoke()));
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

    public void IEGetItemData(Action executeAfter = null)
    {
        StartCoroutine(
            APIManager.instance.GetDataWithTokenCoroutine(
                APIManager.instance.SetupGetItemUrl(GetLanguage()),
                res=>
                {
                    talentDatas = JsonUtility.FromJson<RawData>(res);
                    executeAfter?.Invoke();
                }));
    }

    public void IEGetTalentGroupData()
    {
        StartCoroutine(
            APIManager.instance.GetDataCoroutine(
                APIManager.instance.SetupGetTalentGroupData(),
                res =>
                {
                    Wrapper<TalentGroupData> wrapper = JsonUtility.FromJson<Wrapper<TalentGroupData>>(res);
                    talentGroupDatas = wrapper.error;
                    SetupPlayerTalentData();
                    IEPatchPerksValue();
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

                    if (!currentUserData.success) IECreateUserData();
                    else currentPlayerAssetData = defaultPlayerAssetDatas.Find(data => (int)data.character == GetUserDataValue().character);
                }));
    }
    
    public UserDataSpace.UserDataValue GetUserDataValue() => currentUserData.data;

    public UserDataSpace.CheckpointData GetUserCheckpointData() => currentUserData.data.checkpoint_data;

    public List<TalentGroupValue> GetUserAssessmentData() => currentUserData.data.assessment_values;

    public string GetUniqueCode() => GetUserDataValue().game_url;

    public string GetLanguage() => GetUserDataValue().language;
#endregion
}