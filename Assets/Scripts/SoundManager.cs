using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    AudioSource source;
    public AudioClip jump;
    public AudioClip hitGround;
    public AudioClip death;
    public AudioClip win;
    public AudioClip breakRope;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void Play(AudioClip audio, float volume = 1)
    {
        source.pitch = Random.Range(0.9f, 1.1f);
        source.PlayOneShot(audio, volume);
    }
}
