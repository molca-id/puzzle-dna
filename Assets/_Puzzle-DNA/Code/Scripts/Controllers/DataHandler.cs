using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Events;
using System;

public class DataHandler : MonoBehaviour
{
    public static DataHandler instance;

    [Header("Default Datas")]
    public UserData defaultUserData;

    [Header("Current Datas")]
    public ValidateData validateData;
    public UserData userData;

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
                    userData = JsonUtility.FromJson<UserData>(res);
                    if (!userData.success) IECreateUserData();
                }));
    }

    public UserDataValue GetUserDataValue() => userData.data;

    public CheckpointData GetUserCheckpointData() => userData.data.checkpoint_data;

    public string GetUniqueCode() => GetUserDataValue().game_url;
}
