using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemKillFrenzy : Item
{
    [SerializeField]
    GunState frenzyGun;
    [SerializeField]
    int killCount;
    [SerializeField]
    float maxTime;

    protected virtual void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject != userPlayer.gameObject)
        {
            return;
        }

        
        ActiveOff();
        SoundManager.Instance.PlayClip(soundClip, SoundPlayMode.ObjectSFX);
        QuestManager.Instance.StartKillFrenzy(frenzyGun, killCount, maxTime);
    }
}
