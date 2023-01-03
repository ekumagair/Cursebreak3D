using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSetVolume : MonoBehaviour
{
    // Changes the volume of this object's AudioSource based on the settings.
    AudioSource _as;

    public enum AudioType
    {
        SoundEffect,
        Music
    }
    [Tooltip("Whether this AudioSource represents a sound effect or the scene's music.")]
    public AudioType type = 0;

    void Start()
    {
        _as = GetComponent<AudioSource>();
    }

    void Update()
    {
        switch (type)
        {
            case AudioType.SoundEffect:
                _as.volume = Options.soundVolume;
                break;
            case AudioType.Music:
                _as.volume = Options.musicVolume;
                break;
            default:
                break;
        }
    }
}
