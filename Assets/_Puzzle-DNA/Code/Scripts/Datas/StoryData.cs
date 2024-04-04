using UnityEngine;
using System.Collections.Generic;
using System;
using ActionCode.Attributes;

[Serializable]
public class DialogueStoryData
{
    public bool playerIsTalking;
    public ContentData contentData;
}

[Serializable]
public class DialogueStory
{
    public string interlocutorName;
    public List<DialogueStoryData> dialogueStories;
}

[Serializable]
public class ContentData
{
    public ExpressionType playerExpression;
    public Sprite interlocutorSprite;
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
    public enum StoryType { Dialogue, Narration, PopUp, Tutorial, Title, Event }
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

    [Header("Event Story")]
    public EventData eventDataStory;
}