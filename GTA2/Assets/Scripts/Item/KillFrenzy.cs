using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillFrenzy : Item
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
        SoundManager.Instance.PlayClip(soundClip, SoundPlayMode.OneShotPlay);
        QuestManager.Instance.StartKillFrenzy(frenzyGun, killCount, maxTime);
    }
}
