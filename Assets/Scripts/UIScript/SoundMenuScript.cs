using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMenuScript : MonoBehaviour
{
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _soundFxSlider;

    public void SetSliders()
    {
        _masterSlider.value = SoundMixerManager.Instance.GetMasterVolume();
        _bgmSlider.value = SoundMixerManager.Instance.GetMusicVolume();
        _soundFxSlider.value = SoundMixerManager.Instance.GetSoundFXVolume();
    }

    public void SetMasterVolume(float level)
    {
        SoundMixerManager.Instance.SetMasterVolume(level);
    }

    public void SetSoundFXVolume(float level)
    {
        SoundMixerManager.Instance.SetSoundFXVolume(level);
    }

    public void SetMusicVolume(float level)
    {
        SoundMixerManager.Instance.SetMusicVolume(level);
    }
}
