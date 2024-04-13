using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource bgmAudioSource; //bgm
    public AudioSource sfxAudioSource; //sfx

    public AudioClip bgmClip;
    public List<AudioClip> sfxClips;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        BGMStart();
    }

    //bgm 재생
    public void BGMStart()
    {
        bgmAudioSource.clip = bgmClip;
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
    }

    //효과음 실행
    public void PlaySFX(int index)
    {
        sfxAudioSource.PlayOneShot(sfxClips[index]);
    }
}
