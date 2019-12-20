using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletElectric : Bullet
{
    // Start is called before the first frame update

    [SerializeField]
    DigitalRuby.LightningBolt.LightningBoltScript lightning;
    

    Vector3 targetToVector;
    GameObject startObject;
    GameObject targetObject;
    float electricWaveArea;
    int maxWaveCount;


    public GameObject TargetObject()
    {
        return targetObject;
    }


    protected override void Awake()
    {
        base.Awake();
        lightning.Started();
    }

    public override void SetBullet(GunState type, Vector3 triggerPos, Vector3 dir, float bullettoSize)
    {
        base.SetBullet(type, triggerPos, dir, bullettoSize);
    }

    public void SetAreaAndMaxCount(float area, int maxWaveCount)
    {
        electricWaveArea = area;
        this.maxWaveCount = maxWaveCount;
    }

    
    // Null이 들어올 경우 타겟의 최신화가 이루어지지 않는다.
    // 이미 타겟이 잡힌 경우란 것이다.
    public void SetTarget(GameObject origin, GameObject obj)
    {
        if (origin != null)
        {
            startObject = origin;
        }
        if (obj != null)
        {
            targetObject = obj;
        }

        if (!CheckStartTarget())
        {
            isLife = false;
            PoolManager.ReleaseObject(gameObject);
            return;
        }
        
        targetToVector =
            targetObject.transform.position -
            startObject.transform.position;

        SetScale(targetToVector);
        SetRotate();
        SetPosition(targetToVector);


        // 이렇게 두번 해야 라인랜더러가 안 겹친다... - 이전 상태에서 최신화가 된다.
        lightning.Updated();
        lightning.Updated();
    }



    bool CheckStartTarget()
    {
        if (targetObject == null || startObject == null)
        {
            return false;
        }

        if (!startObject.activeInHierarchy)
        {
            return false;
        }

        if (!targetObject.activeInHierarchy)
        {
            return false;
        }

        NPC npc = startObject.GetComponent<NPC>();
        if (npc != null && npc.isDie)
        {
            return false;
        }

        npc = targetObject.GetComponent<NPC>();
        if (npc != null && npc.isDie)
        {
            return false;
        }


        return true;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }


    public void UpdateBullet(GunState type, Vector3 triggerPos, Vector3 dir, float bullettoSize)
    {
        // base.SetBullet(type, triggerPos, dir, bullettoSize);
        SetTarget(null, null);
    }


    void SetScale(Vector3 targetToVector)
    {
        Vector3 localScale = transform.localScale;
        float targetToVecSize = (
            new Vector2(startObject.transform.position.x, startObject.transform.position.z) -
            new Vector2(targetObject.transform.position.x, targetObject.transform.position.z)).magnitude;

        transform.localScale = new Vector3(
            targetToVecSize * .11f,
            localScale.y,
            localScale.z) ;
    }
    void SetRotate()
    {
        gameObject.transform.LookAt(targetObject.transform);
        gameObject.transform.localEulerAngles += Vector3.up * 270.0f;
    }
    void SetPosition(Vector3 targetToVector)
    {
        Vector3 setVector = targetToVector * .5f;
        // setVector.y = targetToVector.y;

        transform.position = setVector + startObject.transform.position;
    }

    public override void Explosion()
    {
        base.Explosion();
        targetObject = null;
    }
}
