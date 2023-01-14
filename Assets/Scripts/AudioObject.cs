using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioObject : MonoBehaviour
{
    AudioSource audioSource;
    float baseVolume;
    public AudioClip[] clips;
    public float pitchMultMin = 1.0f;
    public float pitchMultMax = 1.0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        baseVolume = audioSource.volume;

        audioSource.clip = clips[Random.Range(0, clips.Length)];
        audioSource.pitch *= Random.Range(pitchMultMin, pitchMultMax);
        SetVolume();
        audioSource.Play();

        StartCoroutine(DestroyAfterAudio());
    }

    void Update()
    {
        SetVolume();
    }

    void SetVolume()
    {
        audioSource.volume = baseVolume * Options.soundVolume;
    }

    IEnumerator DestroyAfterAudio()
    {
        yield return new WaitForSeconds(audioSource.clip.length * 1.5f);
        Destroy(gameObject);
    }
}
