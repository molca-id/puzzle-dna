using ActionCode.Attributes;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

[CreateAssetMenu(fileName = "LevelData", menuName = "DNA/LevelData", order = 1)]
public class LevelData : SingletonScriptableObject<LevelData>
{
    [Header("GameData")]
    public GameData gameData;
    public int perksPoinPlus;
    public int perksPoinMinus;

    [Header("Prologue Story")]
    public List<StoryData> prologueStoryData;

    [Header("Epilogue Story")]
    public List<StoryData> epilogueStoryData;
}