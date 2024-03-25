using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public enum GemType
{
    Drive,
    Network,
    Action,
    GemA,
    GemB,
    GemC,
    Special,
    Empty
}

[Serializable]
public class GemData
{
    public GemType type;
    public Sprite sprite;
    public int minMatch = 3;
}

[Serializable]
public class SpecialGemData
{
    public string name;
    public GameObject prefab;
}

[Serializable]
public class EmptyGemData
{
    public Sprite emptySprite;
    public List<Vector2Int> positions;
}

[Serializable]
public class AudioClipInfo
{
    public string name;
    public AudioClip clip;
}

[Serializable]
public class DialogueBonusData
{
    public bool playerIsTalking;
    public ExpressionType playerExpressionType;
    public float dialogueDelay;
    public ContentData contentData;
}

[Serializable]
public class DialogueBonus
{
    public int scoreTrigger;
    public List<DialogueBonusData> dialogueBonusDatas;
    public bool isDone;
}

[Serializable]
public class Dialogue
{
    public Sprite playerIdleSprite;
    public Sprite interlocutorIdleSprite;
    public string interlocutorName;
    public List<DialogueBonus> dialogueBonus;
}

[CreateAssetMenu(fileName = "GameData", menuName = "DNA/GameData", order = 1)]
public class GameData : ScriptableObject
{
    [Header("Board Dimension")]
    public Vector2Int boardDimension;

    [Header("Game Attributes")]
    public int gameLevel;
    public float abilityDriveDuration;
    public Sprite backgroundGame;
    public Sprite layoutGame;

    [Header("Dialogue Bonus Attributes")]
    public Dialogue dialogues;
    public bool usingDialogueBonus;

    [Header("Board Attribute")]
    public float hintDelay;
    public float totalTime;
    public bool usingTutorial;
    public bool usingHandHint;

    [Header("Gems Attributes")]
    public List<GemData> gems = new();
    public List<SpecialGemData> specialGems = new();
    public List<EmptyGemData> emptyGems = new();

    [Header("Other Attribute")]
    public List<AudioClipInfo> audioClipInfos = new();
    public string[] comboMessages;

    public int maxCombo
    {
        get { return comboMessages.Length; }
    }

    public GemData GemOfType(GemType type)
    {
        return gems.Find(gem => gem.type == type);
    }

    public GemData RandomGem()
    {
        return Miscellaneous.Choose(gems);
    }

    public GameObject GetSpecialGem(string name)
    {
        SpecialGemData sgd = specialGems.Find(gem => gem.name == name);
        if (sgd != null)
            return sgd.prefab;

        return null;
    }

    public AudioClip GetAudioClip(string name)
    {
        AudioClipInfo audioClipInfo = audioClipInfos.Find(
            aci => aci.name == name
        );

        if (audioClipInfo != null)
            return audioClipInfo.clip;

        return null;
    }

    public string GetComboMessage(int combo)
    {
        return comboMessages[combo];
    }
}