using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using System.Linq;

public class DataHandler : MonoBehaviour
{
    public static DataHandler instance;
    [HideInInspector] public ValidateData validateData;

    [Header("Default Datas")]
    public UserDataSpace.UserData defaultUserData;

    [Header("Current Datas")]
    public TalentDataSpace.TalentData talentData;
    public UserDataSpace.UserData userData;
    public List<LanguageHandler> languageHandlers;

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
                    userData = JsonUtility.FromJson<UserDataSpace.UserData>(res);
                    if (!userData.success) IECreateUserData();
                }));
    }

    public List<UserDataSpace.PerksTypeGroupData> GetPerksGroup() => userData.data.perks_value.perks_type_datas;

    public UserDataSpace.UserDataValue GetUserDataValue() => userData.data;

    public UserDataSpace.CheckpointData GetUserCheckpointData() => userData.data.checkpoint_data;

    public string GetUniqueCode() => GetUserDataValue().game_url;

    public string GetLanguage() => GetUserDataValue().language;
}
