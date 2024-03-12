using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    public string audioGroupKey;
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource sfxSource;

    bool isActive;

    public void SetAudiosState()
    {
        isActive = !isActive;
        bgmSource.mute = isActive;
        sfxSource.mute = isActive;
    }
}
