using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundPlayMode
{
    Play,
    OneShotPlay,
    WaitOneShotPlay,
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoSingleton<SoundManager>
{
    [SerializeField]
    float oneShotPlayTime;


    [SerializeField]
    AudioClip respectIs;

    float oneShotPlayDelta;

    AudioSource mainSource;
    Vector3 soundPos;



    void Awake()
    {
        oneShotPlayDelta = .0f;
        mainSource = GetComponent<AudioSource>();

        PlayClip(respectIs, SoundPlayMode.OneShotPlay);
    }

    void Update()
    {
        oneShotPlayDelta += Time.deltaTime;
    }


    public void PlayClipToPosition(AudioClip clip, SoundPlayMode mode, Vector3 pos)
    {
        soundPos = pos;
        PlayClip(clip, mode);
    }

    public void PlayClip(AudioClip clip, SoundPlayMode mode)
    {
        if (clip == null)
        {
            return;
        }
        
        
        mainSource.clip = clip;


        switch (mode)
        {
            case SoundPlayMode.Play:
                if (!mainSource.isPlaying)
                {
                    mainSource.Play();
                }
                break;
            case SoundPlayMode.OneShotPlay:
                mainSource.PlayOneShot(mainSource.clip);
                break;
            case SoundPlayMode.WaitOneShotPlay:
                if (oneShotPlayTime < oneShotPlayDelta)
                {
                    oneShotPlayDelta = .0f;
                    AudioSource.PlayClipAtPoint(clip, soundPos);
                }
                break;
            default:
                break;
        }
    }
}
