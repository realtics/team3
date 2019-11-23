using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletElectric : Bullet
{
    // Start is called before the first frame update
    public Vector3 targetToVector;
    public DigitalRuby.LightningBolt.LightningBoltScript myLightning;
    

    GameObject myTarget;
    float electricWaveArea;



    protected override void Start()
    {
        base.Start();
        myLightning.Started();
    }

    public override void SetBullet(GunState type, Vector3 triggerPos, Vector3 dir, float bullettoSize)
    {
        base.SetBullet(type, triggerPos, dir, bullettoSize);
    }

    public void SetArea(float area)
    {
        electricWaveArea = area;
    }

    // Null이 들어올 경우 타겟의 최신화가 이루어지지 않는다.
    // 이미 타겟이 잡힌 경우란 것이다.
    public void SetTarget(GameObject obj)
    {
        
        if (obj != null)
        {
            myTarget = obj;
        }


        NPC checkNPC = myTarget.GetComponent<NPC>();
        if (checkNPC != null)
        {
            if((checkNPC as People).isDie)
            {
                gameObject.SetActive(false);
            }
        }
        

        targetToVector =
            myTarget.gameObject.transform.position -
            gameObject.transform.position;

        if (targetToVector.sqrMagnitude > electricWaveArea * electricWaveArea)
        {
            gameObject.SetActive(false);
        }


        SetScale(targetToVector);
        SetRotate();
        SetPosition(targetToVector);


        // 이렇게 두번 해야 라인랜더러가 안 겹친다...
        myLightning.Updated();
    }





    protected override void Update()
    {
        base.Update();
    }


    public void UpdateBullet(GunState type, Vector3 triggerPos, Vector3 dir, float bullettoSize)
    {
        base.SetBullet(type, triggerPos, dir, bullettoSize);
        SetTarget(null);
    }


    void SetScale(Vector3 targetToVector)
    {
        Vector3 localScale = transform.localScale;
        float targetToVecSize = (targetToVector).magnitude;

        transform.localScale = new Vector3(
            targetToVecSize * .11f,
            localScale.y,
            localScale.z) ;
    }
    void SetRotate()
    {
        gameObject.transform.LookAt(myTarget.transform, Vector3.up);
        gameObject.transform.localEulerAngles += Vector3.up * 270.0f;
    }
    void SetPosition(Vector3 targetToVector)
    {
        Vector3 setVector = targetToVector * .5f;
        setVector.y = targetToVector.y;

        transform.position += setVector;
    }
}
