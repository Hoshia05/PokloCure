using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundMixerManager : MonoBehaviour
{
    public static SoundMixerManager Instance;

    [SerializeField] private AudioMixer _audioMixer;

    private void Awake()
    {

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }


        DontDestroyOnLoad(gameObject);
    }

    public void SetMasterVolume(float level)
    {
        _audioMixer.SetFloat("MasterVolume", level);
    }

    public void SetSoundFXVolume(float level)
    {
        _audioMixer.SetFloat("SoundFXVolume", level);
    }

    public void SetMusicVolume(float level)
    {
        _audioMixer.SetFloat("MusicVolume", level);
    }
}
