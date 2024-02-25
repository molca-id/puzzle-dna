using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticData
{
    public static UserData userData;

    public static bool requestError;
    public static bool apiError;

    public static string socketUpdatePosData;
    public static string errorMessage;
    public static string debugMessage;
    public static string currentPartId;
    public static string chatText;
    public static string playerNicknameToChat;
    
    public static float masterVolumeCache = 1f;
    public static float bgmVolumeCache = 1f;
    public static float sfxVolumeCache = 1f;
    public static float concertVolumeCache = 1f;

    public static void SetUserData(string json)
    {
        userData = JsonUtility.FromJson<UserData>(json);
    }
}
