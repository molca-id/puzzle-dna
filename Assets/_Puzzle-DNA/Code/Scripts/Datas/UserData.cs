using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UserDataSpace
{
    public enum StoryType { Prologue, Epilogue, Unknown }

    public enum Character { Alfa, Rei, Budi, Bunga, Fatimah, Mentari }

    [Serializable]
    public class UserDataValue
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
        public string f_report_type;
        public string language;
        public string username;
        public int character;
        public int bgm_value;
        public int sfx_value;
        public int vo_value;
        public int narration_story;

        public DateTime createdAt;
        public DateTime updatedAt;

        public CheckpointData checkpoint_data;
        public List<bool> perks_value;
    }

    [Serializable]
    public class CheckpointData
    {
        public bool tutorial_is_done;
        public List<CheckpointValue> checkpoint_value;
    }

    [Serializable]
    public class CheckpointValue
    {
        public int checkpoint_level_score;
        public bool game_is_done;
        public bool prologue_is_done;
        public bool epilogue_is_done;
    }

    [Serializable]
    public class UserData
    {
        [HideInInspector] public bool success;
        [HideInInspector] public string message;
        public UserDataValue data;
    }
}