using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioObject : MonoBehaviour
{
    public AudioClip[] clips;
    public float pitchMultMin = 1.0f;
    public float pitchMultMax = 1.0f;

    private AudioSource _audioSource;
    private float _baseVolume;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _baseVolume = _audioSource.volume;

        _audioSource.clip = clips[Random.Range(0, clips.Length)];
        _audioSource.pitch *= Random.Range(pitchMultMin, pitchMultMax);
        SetVolume();
        _audioSource.Play();

        StartCoroutine(DestroyAfterAudio());
    }

    void Update()
    {
        SetVolume();
    }

    private void SetVolume()
    {
        _audioSource.volume = _baseVolume * Options.soundVolume;
    }

    private IEnumerator DestroyAfterAudio()
    {
        yield return new WaitForSeconds(_audioSource.clip.length * 1.5f);
        Destroy(gameObject);
    }
}
