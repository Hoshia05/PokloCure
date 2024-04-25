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

    public float GetMasterVolume()
    {
        float value;
        _audioMixer.GetFloat("MasterVolume", out value);

        float calcualtedValue = Mathf.Pow(10, value / 20);

        return calcualtedValue;
    }

    public void SetMasterVolume(float level)
    {
        _audioMixer.SetFloat("MasterVolume", Mathf.Log10(level) * 20f);
    }

    public float GetSoundFXVolume()
    {
        float value;
        _audioMixer.GetFloat("SoundFXVolume", out value);

        float calcualtedValue = Mathf.Pow(10, value / 20);

        return calcualtedValue;
    }

    public void SetSoundFXVolume(float level)
    {
        _audioMixer.SetFloat("SoundFXVolume", Mathf.Log10(level) * 20f);
    }


    public float GetMusicVolume()
    {
        float value;
        _audioMixer.GetFloat("MusicVolume", out value);

        float calcualtedValue = Mathf.Pow(10, value / 20);

        return calcualtedValue;
    }

    public void SetMusicVolume(float level)
    {
        _audioMixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20f);
    }
}
