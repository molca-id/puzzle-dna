using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

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

    [Header("Another Attributes")]
    public int protonMax;
    public int electronMax;
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

    public void IEPatchPerksData(Action executeAfter = null)
    {
        string json = "{ \"perks_value\" : " + JsonUtility.ToJson(GetUserDataValue().perks_value) + "}";

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

    public UserDataSpace.PerksValue GetPerksData() => currentUserData.data.perks_value;

    public void ResetAllPerks() => currentUserData.data.perks_value = defaultUserData.data.perks_value;
    
    public UserDataSpace.UserDataValue GetUserDataValue() => currentUserData.data;

    public UserDataSpace.CheckpointData GetUserCheckpointData() => currentUserData.data.checkpoint_data;

    public UserDataSpace.SpecificPerksPoint GetUserSpecificPerksPoint() => GetPerksData().perks_point_data.specific_perks_point;

    public string GetUniqueCode() => GetUserDataValue().game_url;

    public string GetLanguage() => GetUserDataValue().language;

    public UserDataSpace.SpecificPerksPointData GetSpecificPerksPoint(PerksType type) =>
        GetPerksData().perks_point_data.specific_perks_point.specific_perks_point_datas.Find(res => res.perks_type == type);
}
