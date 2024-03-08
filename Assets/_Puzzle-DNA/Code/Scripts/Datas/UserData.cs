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
        public List<PerksTypeGroupData> perks_type_datas;
    }

    [Serializable]
    public class PerksTypeGroupData
    {
        public PerksType perks_type;
        public string perks_type_code;
        public List<PerksStageGroupData> perks_stage_datas;
    }

    [Serializable]
    public class PerksStageGroupData
    {
        public PerksStage perks_stage;
        public List<PerksValueData> perks_value_datas;
    }

    [Serializable]
    public class PerksValueData
    {
        public string perksName;
        public string perksId;
        public int perksPoint;
    }

    [Serializable]
    public class UserData
    {
        [HideInInspector] public bool success;
        [HideInInspector] public string message;
        public UserDataValue data;
    }
}