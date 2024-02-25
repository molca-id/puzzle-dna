using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    public static APIManager instance;
    public string rootUrl = "https://games.talentdna.me:3000";
    public string _apiKey = "";

    private void Awake()
    {
        instance = this;
    }

    public void SetRootURL(string url)
    {
        rootUrl = url;
    }

    public string GetRootURL()
    {
        return rootUrl;
    }

    public IEnumerator GetDataCoroutine(string subUri, Action<string> SetDataEvent)
    {
        string uri = string.Format("{0}/{1}", rootUrl, subUri);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    break;
                case UnityWebRequest.Result.Success:
                    SetDataEvent(webRequest.downloadHandler.text);
                    break;
            }

            webRequest.Dispose();
        }
    }

    //public IEnumerator DeleteDataCoroutine(string subUri, Action<string> SetDataEvent)
    //{
    //    string uri = string.Format("{0}/{1}", rootUrl, subUri);
    //    UnityWebRequest uwr = UnityWebRequest.Delete(uri);

    //    uwr.downloadHandler = new DownloadHandlerBuffer();
    //    uwr.SetRequestHeader("Content-Type", "application/json");
    //    uwr.SetRequestHeader("Authorization-Token", StaticData.userLoginData.TOKENS_TOKEN);
    //    uwr.SetRequestHeader("Api-Key", _apiKey);

    //    yield return uwr.SendWebRequest();
    //    Debug.Log(uwr.downloadHandler.text);

    //    if (uwr.result != UnityWebRequest.Result.Success)
    //    {
    //        StaticData.errorMessage = uwr.downloadHandler.text;
    //        if (uwr.responseCode != 200 && uwr.responseCode != 201) StaticData.apiError = true;
    //        else StaticData.apiError = false;
    //        StaticData.requestError = true;
    //    }
    //    else
    //    {
    //        StaticData.requestError = false;
    //    }

    //    SetDataEvent?.Invoke(uwr.downloadHandler.text);
    //}

    public IEnumerator PostDataCoroutine(string subUri, string postData, Action<string> SetDataEvent)
    {
        string uri = string.Format("{0}/{1}", rootUrl, subUri);
        byte[] rawData = Encoding.UTF8.GetBytes(postData);
        
        UnityWebRequest uwr = UnityWebRequest.PostWwwForm(uri, postData);
        uwr.uploadHandler = new UploadHandlerRaw(rawData);
        uwr.SetRequestHeader("Content-Type", "application/json");
        uwr.SetRequestHeader("Api-Key", _apiKey);

        //if (!string.IsNullOrEmpty(StaticData.GetUserToken()))
        //{
        //    uwr.SetRequestHeader("Authorization-Token", StaticData.userLoginData.TOKENS_TOKEN);
        //}

        yield return uwr.SendWebRequest();
        Debug.Log(uwr.downloadHandler.text);

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            StaticData.errorMessage = uwr.downloadHandler.text;
            if (uwr.responseCode != 200 && uwr.responseCode != 201) StaticData.apiError = true;
            else StaticData.apiError = false;
            StaticData.requestError = true;
        }
        else
        {
            StaticData.requestError = false;
        }

        SetDataEvent?.Invoke(uwr.downloadHandler.text);
    }
}