using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public enum ExpressionType { Senang, Optimis, Sedih }

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
    public Sprite idleSprite;
    public List<PlayerSpriteExpressionData> expressionDatas;
}

public class DataHandler : MonoBehaviour
{
    public static DataHandler instance;
    [HideInInspector] public ValidateData validateData;

    [Header("Default Datas")]
    public UserDataSpace.UserData defaultUserData;
    public TalentDataSpace.TalentData talentData;
    public List<PlayerSpriteData> playerSpriteDatas;

    [Header("Current Datas")]
    public UserDataSpace.UserData currentUserData;
    public PlayerSpriteData currPlayerSpriteData;

    [Header("Another Attributes")]
    public AudioMixer bgmAudioMixer;
    public AudioMixer sfxAudioMixer;
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

    private void Start()
    {
        SetupPlayerSprites();
    }

    public void SetupPlayerSprites()
    {
        foreach (var level in levelDatas)
        {
            GameData game = level.gameData;
            PlayerSpriteData player = DataHandler.instance.currPlayerSpriteData;
            game.dialogues.playerIdleSprite = player.idleSprite;

            foreach (var dialogue in game.dialogues.dialogueBonus)
            {
                foreach (var item in dialogue.dialogueBonusDatas)
                {
                    Sprite temp = player.expressionDatas.Find(
                        exp => exp.expressionType == item.playerExpressionType
                        ).sprite;
                }
            }
        }
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
        string json = "{ \"bgm_value\":" + bgm + ", \"sfx_value\":" + sfx + " }";

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
        string json = "{ \"checkpoint_data\" : " + JsonUtility.ToJson(GetUserDataValue().checkpoint_data) + "}";

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
        bool isID = false;
        if (GetUserDataValue().language == "id")
            isID = true;

        //hitting api
        StartCoroutine(
            APIManager.instance.GetDataCoroutine(
                APIManager.instance.SetupTalentPerksUrl(isID),
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
                    currPlayerSpriteData = playerSpriteDatas.Find(data => (int)data.character == GetUserDataValue().character);

                    if (!currentUserData.success) IECreateUserData();
                }));
    }

    public List<TalentDataSpace.TalentValueData> GetTalentDatas() => talentData.data;

    public UserDataSpace.PerksValue GetPerksData() => currentUserData.data.perks_value;

    public void ResetAllPerks() => currentUserData.data.perks_value = defaultUserData.data.perks_value;
    
    public UserDataSpace.UserDataValue GetUserDataValue() => currentUserData.data;

    public UserDataSpace.CheckpointData GetUserCheckpointData() => currentUserData.data.checkpoint_data;

    public string GetUniqueCode() => GetUserDataValue().game_url;

    public string GetLanguage() => GetUserDataValue().language;
}
