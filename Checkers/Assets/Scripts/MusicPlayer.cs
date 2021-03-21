using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] _tracks;
    AudioSource _audioSource;

    private static MusicPlayer _player;
    private void Awake()
    {
        if (_player == null)
        {
            _player = this;
        }
           
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!_audioSource.isPlaying)
            _audioSource.PlayOneShot(_tracks[Random.Range(0, _tracks.Length)]);
    }
    
}
