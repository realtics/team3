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
    [Header("Sound Value")]
    [SerializeField]
    float posPlayVolume;
    [SerializeField]
    float oneShotPlayTime;
    [SerializeField]
    float oneShotOBJPlayTime;
    [SerializeField]
    float poolResetValue;

    [Header("Pool")]
    [SerializeField]
    int poolCount;
    [SerializeField]
    GameObject sourcePref;
    
    [Header("Mixer")]
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

    [Header("Start")]
    [SerializeField]
    AudioClip respectIs;


    float uiPlayDelta;
    float humanPlayDelta;
    float objPlayDelta;
    float gunPlayDelta;
    float carPlayDelta;
    float explosionPlayDelta;

    List<AudioSource> audioSources;
    AudioSource ActiveSource;

    void Awake()
    {
        uiPlayDelta = 1.0f;
        PoolManager.WarmPool(sourcePref, poolCount);
        audioSources = PoolManager.GetAllObject<AudioSource>(sourcePref);

        foreach (var item in audioSources)
            item.gameObject.transform.parent = transform;

        PlayClip(respectIs, SoundPlayMode.UISFX);

        StartCoroutine(UpdateSource());
    }

    void Update()
    {
        UpdateDelta();
    }


    IEnumerator UpdateSource()
    {
        while (true)
        {
            yield return new WaitForSeconds(poolResetValue);
            foreach (var item in audioSources)
            {
                if (!item.isPlaying)
                {
                    PoolManager.ReleaseObject(item.gameObject);
                }
            }
        }
    }

    void UpdateDelta()
    {
        uiPlayDelta += Time.deltaTime;
        humanPlayDelta += Time.deltaTime;
        objPlayDelta += Time.deltaTime;
        gunPlayDelta += Time.deltaTime;
        carPlayDelta += Time.deltaTime;
        explosionPlayDelta += Time.deltaTime;
    }


    void FindSource()
    {
        ActiveSource = PoolManager.SpawnObject(sourcePref).GetComponent<AudioSource>();
    }

    bool SetMode(AudioClip clip, SoundPlayMode mode)
    {
        if (clip == null)
        {
            return false;
        }

        ActiveSource.clip = clip;
        switch (mode)
        {
            case SoundPlayMode.UISFX:
                ActiveSource.outputAudioMixerGroup = uiSFXmixer;
                break;
            case SoundPlayMode.HumanSFX:
                ActiveSource.outputAudioMixerGroup = humanSFXmixer;
                break;
            case SoundPlayMode.ObjectSFX:
                ActiveSource.outputAudioMixerGroup = objSFXmixer;
                break;
            case SoundPlayMode.GunSFX:
                ActiveSource.outputAudioMixerGroup = objSFXmixer;
                break;
            case SoundPlayMode.CarSFX:
                ActiveSource.outputAudioMixerGroup = carSFXmixer;
                break;
            case SoundPlayMode.ExplosionSFX:
                ActiveSource.outputAudioMixerGroup = explosionSFXmixer;
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
        FindSource();
        if (!SetMode(clip, mode))
        {
            return;
        }

        if (!CheckDelta(mode))
        {
            return;
        }

        ActiveSource.volume = 1.0f;
        ActiveSource.spatialBlend = .0f;
        ActiveSource.rolloffMode = AudioRolloffMode.Logarithmic;
        ActiveSource.Play();        
    }

    public void PlayClipToPosition(AudioClip clip, SoundPlayMode mode, Vector3 pos)
    {
        FindSource();
        if (!SetMode(clip, mode))
        {
            return;
        }

        if (!CheckDelta(mode))
        {
            return;
        }

        ActiveSource.volume = posPlayVolume;
        ActiveSource.spatialBlend = 1.0f;
        ActiveSource.rolloffMode = AudioRolloffMode.Linear;
        ActiveSource.gameObject.transform.position = pos;
        ActiveSource.Play();
    }
}
