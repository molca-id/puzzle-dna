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
    [SerializeField] string talentPerksMyDomain = "get_dna/ms-My";
    [SerializeField] string sendResult = "get_result";

    [Header("Error Handler")]
    public GameObject deletePanel;
    public GameObject errorPanel;

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

    public string SetupSendResultUrl(string sessionCode = "")
    {
        return string.Format("{0}/{1}", rootActUrl, sendResult);
    }

    public string SetupDeleteUrl(string sessionCode = "")
    {
        return string.Format("{0}/{1}/{2}", rootUrl, deleteDomain, sessionCode);
    }

    public string SetupTalentPerksUrl(string lang)
    {
        if (lang == "id") return string.Format("{0}/{1}", rootActUrl, talentPerksIdDomain);
        else if (lang == "en") return string.Format("{0}/{1}", rootActUrl, talentPerksEnDomain);
        else return string.Format("{0}/{1}", rootActUrl, talentPerksMyDomain);
    }

    public IEnumerator PostDataCoroutine(string url, string jsonData, Action<string> SetDataEvent = null)
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        var request = new UnityWebRequest(url, "POST");

        request.SetRequestHeader("Content-Type", "application/json");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
            errorPanel.SetActive(true);
        else
            SetDataEvent?.Invoke(request.downloadHandler.text);
    }

    public IEnumerator PostDataWithTokenCoroutine(string url, string jsonData, Action<string> SetDataEvent = null)
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        var request = new UnityWebRequest(url, "POST");

        // Set the authorization header
        string authHeaderValue = $"Basic dGRuYXhtb2xjYQ==";
        request.SetRequestHeader("Authorization", authHeaderValue);

        // Set the content type header
        request.SetRequestHeader("Content-Type", "application/json");

        // Set the request method and upload/download handlers
        request.method = UnityWebRequest.kHttpVerbPOST;
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Send the request asynchronously
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            // Handle error, for example, activate an error panel
            // errorPanel.SetActive(true);
        }
        else
        {
            // Invoke the callback with the response data
            SetDataEvent?.Invoke(request.downloadHandler.text);
        }
    }

    public IEnumerator PatchDataCoroutine(string url, string jsonData, Action<string> SetDataEvent = null)
    {
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        var request = new UnityWebRequest(url, "PATCH");

        request.SetRequestHeader("Content-Type", "application/json");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
            errorPanel.SetActive(true);
        else
            SetDataEvent(request.downloadHandler.text);
    }

    public IEnumerator GetDataCoroutine(string url, Action<string> SetDataEvent = null)
    {
        using UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
            errorPanel.SetActive(true);
        else
            SetDataEvent(request.downloadHandler.text);
    }

    public IEnumerator DeleteDataCoroutine(string url)
    {
        deletePanel.SetActive(true);
        Time.timeScale = 0f;

        using (UnityWebRequest request = UnityWebRequest.Delete(url))
        {
            yield return request.SendWebRequest();
            Time.timeScale = 1f;

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to delete data: " + request.error);
            }
            else
            {
                Debug.Log("Data deleted successfully.");
                string currentUrl = Application.absoluteURL.Replace("delete", "");
                Debug.Log($"{currentUrl}");
                Application.ExternalEval("window.open('" + currentUrl + "', '_self')");
            }
        }
    }
}