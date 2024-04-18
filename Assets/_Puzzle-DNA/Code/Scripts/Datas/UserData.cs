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
    public class PerksValue
    {
        public PerksAbilityData perks_ability_data;
        public PerksPointData perks_point_data;
        public List<PerksStage> perks_stage_datas;
        public List<PerksValueData> perks_value_datas;
    }

    [Serializable]
    public class PerksStage
    {
        public PerksType perks_types;
        public int perks_point_plus;
        public int perks_point_minus;
        public List<bool> perks_stage_locks;
    }

    [Serializable]
    public class PerksPointData
    {
        public int total_perks_point_plus;
        public int total_perks_point_minus;
        [Space]
        public int perks_point_plus;
        public int perks_point_minus;
        [Space]
        public SpecificPerksPoint specific_perks_point;
    }

    [Serializable]
    public class SpecificPerksPoint
    {
        public int current_game_level;
        public StoryType perks_story_type;
        [Space]
        public PerksType perks_plus_type;
        public PerksType perks_minus_type;
        public int perks_point_plus;
        public int perks_point_minus;
        [Space]
        public List<SpecificPerksPointData> specific_perks_point_datas;
    }

    [Serializable]
    public class SpecificPerksPointData
    {
        public PerksType perks_type;
        public int perks_point_plus;
        public int perks_point_minus;
    }

    [Serializable]
    public class PerksAbilityData
    {
        public bool driveUpgraded;
        public bool networkUpgraded;
        public bool actionUpgraded;
    }

    [Serializable]
    public class PerksValueData
    {
        public string perks_name;
        public string perks_id;
        public int perks_point;
        public string perks_submit_time;
    }

    [Serializable]
    public class UserData
    {
        [HideInInspector] public bool success;
        [HideInInspector] public string message;
        public UserDataValue data;
    }
}