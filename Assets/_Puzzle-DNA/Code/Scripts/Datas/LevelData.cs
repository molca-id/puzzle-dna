using ActionCode.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

[CreateAssetMenu(fileName = "LevelData", menuName = "DNA/LevelData", order = 1)]
public class LevelData : SingletonScriptableObject<LevelData>
{
    [Header("GameData")]
    public GameData gameData;
    public bool showResultPanel;

    [Header("Story")]
    public AudioClip bgmAudioClip;
    public List<StoryData> prologueStoryData;
    public List<StoryData> epilogueStoryData;
}