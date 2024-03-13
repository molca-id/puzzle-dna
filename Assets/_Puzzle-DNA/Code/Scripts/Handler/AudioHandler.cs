using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    public string audioGroupKey;
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioSource voSource;

    bool isActive;

    public void SetAudiosState()
    {
        isActive = !isActive;
        bgmSource.mute = isActive;
        sfxSource.mute = isActive;
        voSource.mute = isActive;
    }

    public void PlayVO(Action executeAfter = null)
    {


        if (executeAfter != null)
            executeAfter.Invoke();
    }
}
