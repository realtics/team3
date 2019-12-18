using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFire : PlayerGun
{
    // Start is called before the first frame update.
    float soundPlayInverval = .3f;
    float soundPlayDelta;


    public override void Init()
    {
        base.Init();
        base.InitGun();

        soundPlayDelta = .0f;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        soundPlayDelta += Time.deltaTime;
    }

    protected override void SFXPlay()
    {
        if (shotSFXName != null && soundPlayDelta > soundPlayInverval)
        {
            SoundManager.Instance.PlayClip(gunSound, SoundPlayMode.OneShotPosPlay);
            soundPlayDelta = .0f;
        }
    }
}
