using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip RunAudio;

    public AudioClip DashAudio;

    public AudioClip DamageAudio;

    void Start()
    {
        this.audioSource = this.gameObject.GetComponent<AudioSource>();
    }

    public void PlayRunAudio()
    {
        if (this.audioSource.clip != this.RunAudio && !this.audioSource.isPlaying)
        {
            this.audioSource.loop = true;
            this.audioSource.clip = this.RunAudio;
            this.audioSource.Play();
        }

    }
    public void StopRunAudio()
    {
        if (this.audioSource.clip == this.RunAudio && this.audioSource.isPlaying)
        {
            this.audioSource.loop = false;
            this.audioSource.clip = null;
            this.audioSource.Stop();
        }

    }
    public void PlayDashAudio()
    {
        this.audioSource.loop = false;
        this.audioSource.clip = this.DashAudio;
        this.audioSource.Play();

    }

    public void PlayDamageAudio()
    {
        this.audioSource.loop = false;
        this.audioSource.clip = this.DamageAudio;
        this.audioSource.Play();

    }
}
