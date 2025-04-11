using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0, 1)]
    public float volume = 1;

    [Range(-3, 3)]
    public float pitch = 1;

    public bool loop = false;

    [HideInInspector]
    public AudioSource source;

    public Sound()
    {
        volume = 1;
        pitch = 1;
        loop = false;
    }
}

public class AudioManager : MonoBehaviour
{
    public Sound[] bgmSounds;
    public Sound[] sfxSounds;

    private Dictionary<string, Sound> soundMap = new Dictionary<string, Sound>();

    public static AudioManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        InitSounds(bgmSounds);
        InitSounds(sfxSounds);
    }

    void InitSounds(Sound[] sounds)
    {
        foreach (Sound s in sounds)
        {
            if (!s.source)
                s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            if (!soundMap.ContainsKey(s.name))
                soundMap.Add(s.name, s);
            else
                Debug.LogWarning("Duplicate sound name found: " + s.name);
        }
    }

    public void PlayAudio(string name)
    {
        if (!soundMap.TryGetValue(name, out Sound s))
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }

        s.source.Play();
    }

    public void StopAudio(string name)
    {
        if (soundMap.TryGetValue(name, out Sound s))
        {
            s.source.Stop();
        }
    }
    
}
