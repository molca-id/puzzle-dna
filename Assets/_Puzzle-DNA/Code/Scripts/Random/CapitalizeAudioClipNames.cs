using UnityEngine;
using System.Collections.Generic;
using System.Globalization;

public class CapitalizeAudioClipNames : MonoBehaviour
{
    public List<AudioClip> audioClips;

    void Start()
    {
        // Loop through each audio clip in the list
        foreach (AudioClip clip in audioClips)
        {
            // Get the original filename
            string originalName = clip.name;

            // Capitalize the first letter of each word in the filename
            string[] words = originalName.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (!string.IsNullOrEmpty(words[i]))
                {
                    char[] letters = words[i].ToCharArray();
                    if (letters.Length > 0)
                    {
                        letters[0] = char.ToUpper(letters[0]);
                        words[i] = new string(letters);
                    }
                }
            }
            string newName = string.Join(" ", words);

            // Rename the audio clip
            clip.name = newName;

            // Print the new name for debugging purposes
            Debug.Log("Renamed " + originalName + " to " + newName);
        }
    }
}
