using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;

    [SerializeField] private AudioSource soundFXObject;
    [SerializeField] private AudioSource _backgroundAudio;

    private List<AudioSource> _soundClipPool = new();

    [Header("Global Sounds")]
    [SerializeField] private AudioClip _menuMusic;
    [SerializeField] private List<AudioClip> _gameSceneBackgroundMusics;
    [SerializeField] private AudioClip _clickSoundClip;
    [SerializeField] private AudioClip _basicWhoosh;


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

        PlayBackgroundMusic(SceneManager.GetActiveScene().name);
    }

    private void Start()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneChanged(Scene previousScene, Scene newScene)
    {
        PlayBackgroundMusic(newScene.name); 
    }
    private void OnSceneUnloaded(Scene scene)
    {
        StopAllCoroutines();
    }

    private void PlayBackgroundMusic(string CurrentSceneName)
    {

        CreateSoundClipPool();

        AudioClip nextClip;

        if (CurrentSceneName.Equals("GameScene"))
        {

            nextClip = _gameSceneBackgroundMusics[GameManager.Instance.Rand.Next(_gameSceneBackgroundMusics.Count)];
        }
        else
        {
            nextClip = _menuMusic;
        }

        if(_backgroundAudio.clip != nextClip)
        {
            _backgroundAudio.clip = nextClip;
            _backgroundAudio.Play();
        }

    }

    private void CreateSoundClipPool()
    {
        GameObject soundObjectPoolParent = new GameObject("soundObjectPoolParent");

        _soundClipPool = new();
        for (int i = 0; i < 200; i++)
        {
            AudioSource audioSource = Instantiate(soundFXObject, soundObjectPoolParent.transform);
            audioSource.gameObject.SetActive(false);
            _soundClipPool.Add(audioSource);
        }

    }

    private AudioSource GetAudioSourceObjectFromPool()
    {
        AudioSource audioSource = _soundClipPool.FirstOrDefault(x => x != null && !x.gameObject.activeSelf);
        if (audioSource == null)
            return null;               // 풀이 가득 참: 이번 사운드는 생략
        audioSource.gameObject.SetActive(true);

        return audioSource;
    }

    IEnumerator DeactivateAudioSource(AudioSource audiosource, float clipLength)
    {
        // 일시정지(timeScale = 0) 중에도 반환되도록 실시간 기준으로 대기
        yield return new WaitForSecondsRealtime(clipLength);

        if (audiosource != null)
            audiosource.gameObject.SetActive(false);
    }


    // Start is called before the first frame update
    public void PlaySoundFXClip(AudioClip clip, Transform spawnTransform, float volume)
    {
        if (clip == null || spawnTransform == null)
            return;

        AudioSource audiosource = GetAudioSourceObjectFromPool();
        if (audiosource == null)
            return;

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
        PlaySoundFXClip(_clickSoundClip, transform, 1f);
    }
}
