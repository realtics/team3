using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class NPCGun : Gun
{

    // Start is called before the first frame update
    protected override void InitGun()
    {
        base.InitGun();
        transform.eulerAngles = new Vector3(90.0f, 0.0f, 90.0f);
    }



    public void StartShot()
    {
        isPrevShot = false;
        isShot = true;
    }

    public void StopShot()
    {
        isPrevShot = true;
        isShot = false;
    }
}
