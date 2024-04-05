using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;

    [SerializeField] private AudioSource soundFXObject;
    [SerializeField] private AudioSource _backgroundAudio;

    private List<AudioSource> _soundClipPool = new();

    [Header("Global Sounds")]
    [SerializeField] private List<AudioClip> _backgroundMusics;
    [SerializeField] private AudioClip _clickSoundClip;
    [SerializeField] private AudioClip _basicWhoosh;


    private void Awake()
    {

        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }


        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        CreateSoundClipPool();

        _backgroundAudio.clip = _backgroundMusics[GameManager.Instance.Rand.Next(_backgroundMusics.Count)];
        _backgroundAudio.Play();
    }

    private void CreateSoundClipPool()
    {
        for (int i = 0; i < 200; i++)
        {
            AudioSource audioSource = Instantiate(soundFXObject, Vector2.zero, Quaternion.identity);
            audioSource.gameObject.SetActive(false);
            _soundClipPool.Add(audioSource);
        }

    }

    private AudioSource GetAudioSourceObjectFromPool()
    {
        AudioSource audioSource = _soundClipPool.First(x => x.gameObject.activeSelf == false);
        audioSource.gameObject.SetActive(true);

        return audioSource;
    }

    IEnumerator DeactivateAudioSource(AudioSource audiosource, float clipLength)
    {
        yield return new WaitForSeconds(clipLength);

        audiosource.gameObject.SetActive(false);
    }


    // Start is called before the first frame update
    public void PlaySoundFXClip(AudioClip clip, Transform spawnTransform, float volume)
    {
        AudioSource audiosource = GetAudioSourceObjectFromPool();

        audiosource.transform.position = spawnTransform.position;

        audiosource.clip = clip;

        audiosource.volume = volume;

        audiosource.Play();

        float cliplength = audiosource.clip.length;

        StartCoroutine(DeactivateAudioSource(audiosource, cliplength));
    }

    public void PlayBasicWhoosh(Transform spawnTransform, float volume)
    {
        PlaySoundFXClip(_basicWhoosh, spawnTransform, volume);
    }

    public void PlayClickSound()
    {
        PlaySoundFXClip(_clickSoundClip, transform, 10f);
    }
}
