using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Sound[] sounds;

    private void Awake()
    {
        foreach (var sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.isLoop;
        }
    }

    public void Play(string name)
    {
        var s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }

    public bool IsPlaying(string name)
    {
        var s = Array.Find(sounds, sound => sound.name == name);
        return s.source.isPlaying;
    }
}