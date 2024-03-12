using System.Web;
using System;
using UnityEngine;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;

public class SessionCodeHooker : MonoBehaviour
{
    public static SessionCodeHooker instance;
    [TextArea(3, 3)] [SerializeField] string dummyCodeTemp;
    [TextArea(3, 3)] [SerializeField] string currCodeTemp;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
#if UNITY_EDITOR
        PlayerCodeHandler(dummyCodeTemp);
#else
        Uri myUri = new(Application.absoluteURL);
        string game_url = HttpUtility.ParseQueryString(myUri.Query).Get("game_url");

        if (!string.IsNullOrEmpty(game_url)) PlayerCodeHandler(game_url);
        else PreloadManager.instance.SetValidState(false);
#endif
    }

    public void PlayerCodeHandler(string code)
    {
        currCodeTemp = code;
        //Debug.Log($"Game Code: {currCodeTemp}");
    }

    public string GetSessionCode() => currCodeTemp;
}