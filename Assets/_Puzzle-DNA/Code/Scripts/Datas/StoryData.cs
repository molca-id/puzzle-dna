using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public enum StoryType
{
    Type1, Type2, Type3, Type4
}

public enum StoryPoint
{
    Prologue, Epilogue
}

[CreateAssetMenu(fileName = "StoryData", menuName = "DNA/StoryData", order = 1)]
public class StoryData : SingletonScriptableObject<StoryData>
{
    public StoryPoint storyPoint;
    public StoryType storyType;
}
