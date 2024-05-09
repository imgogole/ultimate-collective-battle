using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAdjustement : MonoBehaviour
{
    public AudioType audioType;

    AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (audioSource)
        {
            audioSource.volume = audioType == AudioType.SFX ? GameSettings.SFXVolume : GameSettings.MusicVolume ;
        }
    }
}

public enum AudioType
{
    SFX,
    Music
}
