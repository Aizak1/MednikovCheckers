using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] _tracks;
    AudioSource _audioSource;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
       
    }

    // Update is called once per frame
    void Update()
    {
        if(!_audioSource.isPlaying)
        _audioSource.PlayOneShot(_tracks[Random.Range(0,_tracks.Length)]);
    }
    
}
