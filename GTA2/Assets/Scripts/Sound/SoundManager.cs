using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    [SerializeField]
    SoundSFXs mainVoiceSFXs;
    [SerializeField]
    SoundSFXs gunShotSFXs;

    AudioSource mainVoiceSource;
    AudioSource gunShotSource;


    void Awake()
    {
        mainVoiceSource = gameObject.AddComponent<AudioSource>();
        gunShotSource = gameObject.AddComponent<AudioSource>();
    }



    public void PlayMainVoice(string name)
    {
        PlaySource(mainVoiceSource, mainVoiceSFXs, name, true);
    }

    public void PlayGunShot(string name)
    {
        PlaySource(gunShotSource, gunShotSFXs, name, true);
    }





    void PlaySource(AudioSource audioSource, SoundSFXs soundSFXs, string name, bool playOverride)
    {
        audioSource.clip = soundSFXs.FindClip(name);

        if (playOverride)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }
        else if(!playOverride && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
