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

    // SoundMixerManager가 아직 없을 때(에디터에서 GameScene부터 바로 재생 등) 슬라이더 초기화 이벤트가 먼저 호출될 수 있다.
    public void SetSliders()
    {
        if (SoundMixerManager.Instance == null)
            return;

        _masterSlider.value = SoundMixerManager.Instance.GetMasterVolume();
        _bgmSlider.value = SoundMixerManager.Instance.GetMusicVolume();
        _soundFxSlider.value = SoundMixerManager.Instance.GetSoundFXVolume();
    }

    public void SetMasterVolume(float level)
    {
        if (SoundMixerManager.Instance == null)
            return;

        SoundMixerManager.Instance.SetMasterVolume(level);
    }

    public void SetSoundFXVolume(float level)
    {
        if (SoundMixerManager.Instance == null)
            return;

        SoundMixerManager.Instance.SetSoundFXVolume(level);
    }

    public void SetMusicVolume(float level)
    {
        if (SoundMixerManager.Instance == null)
            return;

        SoundMixerManager.Instance.SetMusicVolume(level);
    }
}
