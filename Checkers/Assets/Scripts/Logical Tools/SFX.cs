using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    [SerializeField] private AudioClip _pickSound;
    [SerializeField] private AudioClip _dropSound;
    [SerializeField] private AudioClip _killSound;
    private AudioSource _audioSource;
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayPicKSound()
    {
        _audioSource.PlayOneShot(_pickSound);
    }
    public void PlayDropSound()
    {
        _audioSource.PlayOneShot(_dropSound);
    }

    public void PlayKillSound()
    {
        _audioSource.PlayOneShot(_killSound);
    }
}
