using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSound : MonoBehaviour
{
    public AudioClip[] randomSound = new AudioClip[3];
    public AudioSource platAudioSource;

    private void Start()
    {
        platAudioSource = GetComponent<AudioSource>();
    }

    public void PlayRandomSound()
    {
        platAudioSource.clip = randomSound[Random.Range(0, 3)];
        platAudioSource.Play();
    }
}
