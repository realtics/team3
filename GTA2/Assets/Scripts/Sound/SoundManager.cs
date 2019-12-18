using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundPlayMode
{
    UISFX,
    Play,
    OneShotPlay,
    OneShotPosPlay,
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoSingleton<SoundManager>
{
    [SerializeField]
    float oneShotPlayTime;
	
    [SerializeField]
    AudioClip respectIs;

    [SerializeField]
    AudioMixerGroup uiSFXmixer;
    [SerializeField]
    AudioMixerGroup SFXmixer;


    float oneShotPlayDelta;

    AudioSource SFXSource;
    Vector3 soundPos;



    void Awake()
    {
        oneShotPlayDelta = .0f;
        SFXSource = GetComponent<AudioSource>();

        PlayClip(respectIs, SoundPlayMode.UISFX);
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


        SFXSource.clip = clip;


        switch (mode)
        {
            case SoundPlayMode.UISFX:
                SFXSource.outputAudioMixerGroup = uiSFXmixer;
                SFXSource.PlayOneShot(SFXSource.clip);
                break;
            case SoundPlayMode.Play:
                if (!SFXSource.isPlaying)
                {
                    SFXSource.Play();
                }
                break;
            case SoundPlayMode.OneShotPlay:
                if (oneShotPlayDelta >= oneShotPlayTime)
                {
                    SFXSource.outputAudioMixerGroup = SFXmixer;
                    SFXSource.PlayOneShot(SFXSource.clip);
                    oneShotPlayDelta = .0f;
                }
                break;
            case SoundPlayMode.OneShotPosPlay:
                if (oneShotPlayDelta >= oneShotPlayTime)
                {
                    AudioSource.PlayClipAtPoint(clip, soundPos);
                    oneShotPlayDelta = .0f;
                }
                break;
            default:
                break;
        }
    }
}
