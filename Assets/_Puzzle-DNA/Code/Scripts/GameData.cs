﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public enum GemType {
    Milk,
    Apple,
    Orange,
    Bread,
    Lettuce,
    Coconut,
    Flower,
    Special,
    Empty
}

[System.Serializable]
public class GemData {
    public GemType type;
    public Sprite sprite;
    public int minMatch = 3;
}

[System.Serializable]
public class SpecialGemData {
    public string name;
    public GameObject prefab;
}

[System.Serializable]
public class EmptyGemData
{
    public Sprite emptySprite;
    public List<Vector2Int> positions;
}

[System.Serializable]
public class AudioClipInfo {
    public string name;
    public AudioClip clip;
}

[CreateAssetMenu(fileName = "GameData", menuName = "Match3/GameData", order = 1)]
public class GameData : SingletonScriptableObject<GameData> {

    [SerializeField] Vector2Int boardDimension;
    public bool usingPowerUps;

    [Header("Gems Attributes")]
    [SerializeField] List<GemData> gems = new List<GemData>();
    [SerializeField] List<SpecialGemData> specialGems = new List<SpecialGemData>();
    public List<EmptyGemData> emptyGems = new List<EmptyGemData>();

    [Header("Other Attribute")]
    [SerializeField] List<AudioClipInfo> audioClipInfos = new List<AudioClipInfo>();
    [SerializeField] string[] comboMessages;

    public Vector2Int _boardDimension => instance.boardDimension;
    
    public static int maxCombo {
        get { return instance.comboMessages.Length; }
    }

    public static GemData GemOfType(GemType type) {
        return instance.gems.Find(gem => gem.type == type);
    }

    public static GemData RandomGem() {
        return Miscellaneous.Choose(instance.gems);
    }

    public static GameObject GetSpecialGem(string name) {
        SpecialGemData sgd = instance.specialGems.Find(gem => gem.name == name);
        if(sgd != null)
            return sgd.prefab;

        return null;
    }

    public static AudioClip GetAudioClip(string name) {
        AudioClipInfo audioClipInfo = instance.audioClipInfos.Find(
            aci => aci.name == name
        );

        if(audioClipInfo != null)
            return audioClipInfo.clip;

        return null;
    }

    public static string GetComboMessage(int combo) {
        return instance.comboMessages[combo];
    }
}
