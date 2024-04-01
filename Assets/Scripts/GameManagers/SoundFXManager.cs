using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;

    [SerializeField] private AudioSource soundFXObject;

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

    // Start is called before the first frame update
    public void PlaySoundFXClip(AudioClip clip, Transform spawnTransform, float volume)
    {
        AudioSource audiosource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audiosource.clip = clip;

        audiosource.volume = volume;

        audiosource.Play();

        float cliplength = audiosource.clip.length;

        Destroy(audiosource.gameObject, cliplength);
    }
}
