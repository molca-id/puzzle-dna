using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Data
{
    [HideInInspector] public string survey_code;
    [HideInInspector] public string survey_dibuat;
    [HideInInspector] public string tgl_lahir;
    [HideInInspector] public string f_jenis_kelamin;
    [HideInInspector] public object game_start;
    [HideInInspector] public object game_finish;
    [HideInInspector] public string email;
    [HideInInspector] public string job;
    [HideInInspector] public string f_pendidikan;

    public string status;
    public string duration;
    public string game_url;
    public string language;
    public string username;
    public int narration_story;

    public DateTime createdAt;
    public DateTime updatedAt;
    
    public PerksValue perks_value;
    public CheckpointData checkpoint_data;
}

[Serializable]
public class CheckpointData
{
    public bool tutorialIsDone;
    public List<CheckpointLevelData> checkpointLevelDatas;
}

[Serializable]
public class CheckpointLevelData
{
    public int level;
    public int score;
}

[Serializable]
public class PerksValue
{
    public PerksValueData[] perksValueDatas;
}

[Serializable]
public class PerksValueData
{
    public string perksName;
    public int perksPoint;
}

[Serializable]
public class UserData
{
    public bool success;
    public string message;
    public Data data;
}
