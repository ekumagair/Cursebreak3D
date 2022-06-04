using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioObject : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip[] clips;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = clips[Random.Range(0, clips.Length)];
        audioSource.Play();

        StartCoroutine(DestroyAfterAudio());
    }

    IEnumerator DestroyAfterAudio()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        Destroy(gameObject);
    }
}
