using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class DialogueStoryData
{
    public bool playerIsTalking;
    public ContentData contentData;
}

[Serializable]
public class DialogueStory
{
    public Sprite playerSprite;
    public Sprite interlocutorSprite;
    public string interlocutorName;
    public List<DialogueStoryData> dialogueStories;
}

[Serializable]
public class ContentData
{
    [TextArea(5, 5)] public string contentId;
    [TextArea(5, 5)] public string contentEn;
}

[CreateAssetMenu(fileName = "StoryData", menuName = "DNA/StoryData", order = 1)]
public class StoryData : ScriptableObject
{
    public enum StoryType { Dialogue, Narration, PopUp }

    public StoryType storyType;
    public Sprite backgroundSprite;
    public DialogueStory dialogueStory;
    public List<ContentData> narrationStories;
    public List<ContentData> popUpStories;
}