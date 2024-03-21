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
    public AudioClip clipId;
    public AudioClip clipEn;
    public AudioClip clipMy;
    [TextArea(5, 5)] public string contentId;
    [TextArea(5, 5)] public string contentEn;
    [TextArea(5, 5)] public string contentMy;
}

[CreateAssetMenu(fileName = "StoryData", menuName = "DNA/StoryData", order = 1)]
public class StoryData : ScriptableObject
{
    public enum StoryType { Dialogue, Narration, PopUp, Tutorial, Title }
    public enum NarrationType { Above, Middle, Under }

    public StoryType storyType;
    public Sprite backgroundSprite;

    [Header("Selecting Tutorial")]
    public string tutorialKey;

    [Header("Dialogue Story")]
    public DialogueStory dialogueStory;

    [Header("Narration Story")]
    public NarrationType narrationType;
    public List<ContentData> narrationStories;

    [Header("Pop Up Story")]
    public List<ContentData> popUpStories;

    [Header("Title Story")]
    public List<ContentData> titleStories;
}