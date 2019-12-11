using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    [SerializeField]
    float oneShotPlayTime;
    float oneShotPlayDelta;

    AudioSource mainSource;



    void Awake()
    {
        oneShotPlayDelta = .0f;
        mainSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        oneShotPlayDelta += Time.deltaTime;
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
            mainSource.PlayOneShot(mainSource.clip);
        }
        else if (!playOneShot && !mainSource.isPlaying)
        {
            mainSource.Play();
        }
    }
}
