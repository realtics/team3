using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoSingleton<SoundManager>
{
    [SerializeField]
    float oneShotPlayTime;
    float oneShotPlayDelta;

    AudioSource mainSource;
    Vector3 soundPos;



    void Awake()
    {
        oneShotPlayDelta = .0f;
        mainSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        oneShotPlayDelta += Time.deltaTime;
    }


    public void PlayClipFromPosition(AudioClip clip, bool playOneShot, Vector3 pos)
    {
        soundPos = pos;
        PlayClip(clip, playOneShot);
    }

    public void PlayClip(AudioClip clip, bool playOneShot)
    {
        if (clip == null)
        {
            return;
        }
        
        
        mainSource.clip = clip;

        if (playOneShot && oneShotPlayTime < oneShotPlayDelta)
        {
            oneShotPlayDelta = .0f;
            AudioSource.PlayClipAtPoint(clip, soundPos);
        }
        else if (!playOneShot && !mainSource.isPlaying)
        {
            mainSource.Play();
        }
    }
}
