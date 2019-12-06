using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSFXs : MonoBehaviour
{
    public List<AudioClip> clipList;

    public AudioClip FindClip(string name)
    {
        AudioClip returnClip = null;

        foreach (var item in clipList)
        {
            if (item.name == name)
            {
                returnClip = item;
                break;
            }
        }

        return returnClip;
    }
}
