using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sfx;
    public Sound[] themes;

    public static AudioManager instance;

    // Start is called before the first frame update
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

        foreach(Sound s in sfx)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        foreach(Sound s in themes)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        StopMusic();
        PlayMusic(level);
    }

    private void Start()
    {
        PlayMusic(0);
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfx, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound with name " + name + "was not found");
            return;
        }
        s.source.Play();
    }

    private void PlayMusic(int lvlIdx)
    {
        themes[lvlIdx].source.Play();
    }

    private void StopMusic()
    {
        foreach(Sound s in themes)
        {
            s.source.Stop();
        }
    }

    public void AdjustMasterVolume(float newVolume)
    {
        foreach(Sound s in sfx)
        {
            s.source.volume = newVolume;
        }
        foreach(Sound s in themes)
        {
            s.source.volume = newVolume;
        }
    }
}
