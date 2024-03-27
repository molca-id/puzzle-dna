using ActionCode.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

[CreateAssetMenu(fileName = "LevelData", menuName = "DNA/LevelData", order = 1)]
public class LevelData : SingletonScriptableObject<LevelData>
{
    [Serializable]
    public class PerksStage
    {
        public PerksType perks_types;
        public List<bool> perks_stage_locks;
    }

    [Header("GameData")]
    public GameData gameData;
    public int perksPoinPlus;
    public int perksPoinMinus;
    public bool openPerksPanelAfterGame;

    [Header("Addon For Game Unlocking Perk Stage")]
    public bool usingPerkUnlocking;
    public List<PerksStage> perkStageForUnlocking;

    [Header("Story")]
    public List<StoryData> prologueStoryData;
    public List<StoryData> epilogueStoryData;
}