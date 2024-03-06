using UnityEngine;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class DialogueStoryData
{
    public bool playerIsTalking;
    [TextArea(5, 5)] public string dialogueContent;
}

[Serializable]
public class DialogueStory
{
    public Sprite playerSprite;
    public Sprite interlocutorSprite;
    public string interlocutorName;
    public List<DialogueStoryData> dialogueStories;
}

[CreateAssetMenu(fileName = "StoryData", menuName = "DNA/StoryData", order = 1)]
public class StoryData : ScriptableObject
{
    public enum StoryType { Dialogue, Narration, PopUp }

    public StoryType storyType;
    public Sprite backgroundSprite;
    public DialogueStory dialogueStory;
    [TextArea(5, 5)] public List<string> narrationStories;
    [TextArea(5, 5)] public List<string> popUpStories;
}