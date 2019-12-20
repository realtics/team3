using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundPlayMode
{
    UISFX,
    HumanSFX,
    ObjectSFX,
    GunSFX,
    CarSFX,
    ExplosionSFX,
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoSingleton<SoundManager>
{
    [SerializeField]
    float posPlayVolume;
    [SerializeField]
    float oneShotPlayTime;
    [SerializeField]
    float oneShotOBJPlayTime;

    [SerializeField]
    AudioClip respectIs;

    [SerializeField]
    AudioMixerGroup uiSFXmixer;
    [SerializeField]
    AudioMixerGroup humanSFXmixer;
    [SerializeField]
    AudioMixerGroup objSFXmixer;
    [SerializeField]
    AudioMixerGroup carSFXmixer;
    [SerializeField]
    AudioMixerGroup explosionSFXmixer;


    float uiPlayDelta;
    float humanPlayDelta;
    float objPlayDelta;
    float gunPlayDelta;
    float carPlayDelta;
    float explosionPlayDelta;

    AudioSource SFXSource;

    void Awake()
    {
        uiPlayDelta = 1.0f;
        SFXSource = GetComponent<AudioSource>();

        PlayClip(respectIs, SoundPlayMode.UISFX);
    }

    void Update()
    {
        uiPlayDelta += Time.deltaTime;
        humanPlayDelta += Time.deltaTime;
        objPlayDelta += Time.deltaTime;
        gunPlayDelta += Time.deltaTime;
        carPlayDelta += Time.deltaTime;
        explosionPlayDelta += Time.deltaTime;
    }


    bool SetMode(AudioClip clip, SoundPlayMode mode)
    {
        if (clip == null)
        {
            return false;
        }

        SFXSource.clip = clip;
        switch (mode)
        {
            case SoundPlayMode.UISFX:
                SFXSource.outputAudioMixerGroup = uiSFXmixer;
                break;
            case SoundPlayMode.HumanSFX:
                SFXSource.outputAudioMixerGroup = humanSFXmixer;
                break;
            case SoundPlayMode.ObjectSFX:
                SFXSource.outputAudioMixerGroup = objSFXmixer;
                break;
            case SoundPlayMode.GunSFX:
                SFXSource.outputAudioMixerGroup = objSFXmixer;
                break;
            case SoundPlayMode.CarSFX:
                SFXSource.outputAudioMixerGroup = carSFXmixer;
                break;
            case SoundPlayMode.ExplosionSFX:
                SFXSource.outputAudioMixerGroup = explosionSFXmixer;
                break;
            default:
                break;
        }

        return true;
    }

    bool CheckDelta(SoundPlayMode mode)
    {
        switch (mode)
        {
            case SoundPlayMode.UISFX:
                if (uiPlayDelta >= oneShotPlayTime)
                {
                    uiPlayDelta = .0f;
                    return true;
                }
                break;
            case SoundPlayMode.HumanSFX:
                if (humanPlayDelta >= oneShotPlayTime)
                {
                    humanPlayDelta = .0f;
                    return true;
                }
                break;
            case SoundPlayMode.ObjectSFX:
                if (objPlayDelta >= /*oneShotPlayTime*/oneShotOBJPlayTime)
                {
                    objPlayDelta = .0f;
                    return true;
                }
                break;
            case SoundPlayMode.GunSFX:
                if (gunPlayDelta >= oneShotPlayTime)
                {
                    gunPlayDelta = .0f;
                    return true;
                }
                break;
            case SoundPlayMode.CarSFX:
                if (carPlayDelta >= oneShotPlayTime)
                {
                    carPlayDelta = .0f;
                    return true;
                }
                break;
            case SoundPlayMode.ExplosionSFX:
                if (explosionPlayDelta >= oneShotPlayTime)
                {
                    explosionPlayDelta = .0f;
                    return true;
                }
                break;
            default:
                break;
        }

        return false;
    }

    public void PlayClip(AudioClip clip, SoundPlayMode mode)
    {
        if (!SetMode(clip, mode))
        {
            return;
        }

        if (!CheckDelta(mode))
        {
            return;
        }

        SFXSource.rolloffMode = AudioRolloffMode.Logarithmic;
        SFXSource.PlayOneShot(SFXSource.clip);        
    }

    public void PlayClipToPosition(AudioClip clip, SoundPlayMode mode, Vector3 pos)
    {
        if (!SetMode(clip, mode))
        {
            return;
        }

        if (!CheckDelta(mode))
        {
            return;
        }

        SFXSource.rolloffMode = AudioRolloffMode.Linear;
        AudioSource.PlayClipAtPoint(clip, pos, posPlayVolume);
    }
}
