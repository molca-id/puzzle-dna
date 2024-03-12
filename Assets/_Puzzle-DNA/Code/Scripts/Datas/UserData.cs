using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UserDataSpace
{
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
        public string language;
        public string username;
        public int character;
        public int bgm_value;
        public int sfx_value;
        public int narration_story;

        public DateTime createdAt;
        public DateTime updatedAt;

        public PerksValue perks_value;
        public CheckpointData checkpoint_data;
    }

    [Serializable]
    public class CheckpointData
    {
        public bool tutorial_is_done;
        public List<CheckpointLevelData> checkpoint_level_datas;
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
        public int perks_point_plus;
        public int perks_point_minus;
        public List<PerksStage> perks_stage_datas;
        public List<PerksValueData> perks_type_datas;
    }

    [Serializable]
    public class PerksStage
    {
        public PerksType perks_types;
        public List<bool> perks_stage_locks;
    }

    [Serializable]
    public class PerksValueData
    {
        public string perks_name;
        public string perks_id;
        public int perks_point;
    }

    [Serializable]
    public class UserData
    {
        [HideInInspector] public bool success;
        [HideInInspector] public string message;
        public UserDataValue data;
    }
}