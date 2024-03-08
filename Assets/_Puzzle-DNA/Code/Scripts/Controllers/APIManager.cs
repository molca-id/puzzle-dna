using Newtonsoft.Json;
using System;
using System.Collections;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    public static APIManager instance;

    [Header("Basic URL")]
    [SerializeField] string rootUrl = "https://games.talentdna.me:3000";
    [SerializeField] string gameDomain = "gamedata";
    [SerializeField] string validateDomain = "validate";
    [SerializeField] string deleteDomain = "delete";

    [Header("ACT URL")]
    [SerializeField] string rootActUrl = "https://talentdna.me/tdna/api_molca";
    [SerializeField] string talentPerksEnDomain = "get_dna/en-US";
    [SerializeField] string talentPerksIdDomain = "get_dna/id-ID";

    private void Awake()
    {
        instance = this;
    }

    public string SetupGameUrl(string sessionCode = "")
    {
        return string.Format("{0}/{1}/{2}", rootUrl, gameDomain, sessionCode);
    }

    public string SetupValidateUrl(string sessionCode = "")
    {
        return string.Format("{0}/{1}/{2}", rootUrl, validateDomain, sessionCode);
    }

    public string SetupDeleteUrl(string sessionCode = "")
    {
        return string.Format("{0}/{1}/{2}", rootUrl, deleteDomain, sessionCode);
    }

    public string SetupTalentPerksUrl(bool isID)
    {
        if (isID) return string.Format("{0}/{1}", rootActUrl, talentPerksIdDomain);
        else return string.Format("{0}/{1}", rootActUrl, talentPerksEnDomain);
    }

    public IEnumerator PostDataCoroutine(string url, string jsonData, Action<string> SetDataEvent = null)
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        var request = new UnityWebRequest(url, "POST");

        request.SetRequestHeader("Content-Type", "application/json");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError("Error posting data: " + request.error);
        else
            SetDataEvent?.Invoke(request.downloadHandler.text);
    }

    public IEnumerator PatchDataCoroutine(string url, string jsonData, Action<string> SetDataEvent = null)
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        var request = new UnityWebRequest(url, "PATCH");

        request.SetRequestHeader("Content-Type", "application/json");
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError("Error patching checkpoint data: " + request.error);
        else
            SetDataEvent(request.downloadHandler.text);
    }

    public IEnumerator GetDataCoroutine(string url, Action<string> SetDataEvent = null)
    {
        using UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError("Error getting checkpoint data: " + request.error);
        else
            SetDataEvent(request.downloadHandler.text);
    }
}