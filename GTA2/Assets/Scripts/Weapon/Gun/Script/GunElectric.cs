using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunElectric : PlayerGun
{
    [Header("Electric Value")]
    public int electricWaveMaxIndex;
    public float electricWaveArea;
    public float electricWaveAngle;



    List<GameObject> objectList;
    List<GameObject> noneTargetObjectList;
    List<BulletElectric> activeElectricList;


    public override void Init()
    {
		base.Init();
        InitGun();

        objectList = new List<GameObject>();
        noneTargetObjectList = new List<GameObject>();
        activeElectricList = new List<BulletElectric>();


        List<BulletElectric> bulletList = PoolManager.GetAllObject<BulletElectric>(bulletPref);
        foreach (var item in bulletList)
        {
            item.SetAreaAndMaxCount(electricWaveArea, electricWaveMaxIndex);
        }

        gameObject.SetActive(true);
        StartCoroutine(UpdateTarget());
    }

    IEnumerator UpdateTarget()
    {
        while (true)
        {
            yield return new WaitForSeconds(.1f);

            objectList.Clear();

            foreach (var npc in NPCSpawnManager.Instance.allNPC)
            {
                if (npc == null)
                {
                    continue;
                }
                objectList.Add(npc.gameObject);
            }
            foreach (var car in CarSpawnManager.Instance.allCars)
            {
                if (car == null)
                {
                    continue;
                }
                objectList.Add(car.gameObject);
            }
        }
    }


    protected override void FixedUpdate()
    {
        if (player == null)
        {
            InitPlayer();
        }
        if (player.isDie == true)
        {
            return;
        }
        if (bulletCount == 0)
        {
            return;
        }
        base.UpdateDirection();
        UpdateDelta();
        UpdateKeyInput();
        UpdateShot();
    }
    protected override void UpdateShot()
    {
        if (GameManager.Instance.playerCar != null)
        {
            return;
        }


        if (isKeyShot || isButtonShot)
        {
            if (shootInterval < shootDelta)
            {
                FireGun();
                UpdateElectric();
                shootDelta = .0f;
            }
        }
        
        else if (!(isKeyShot || isButtonShot))
        {
            StopGun();
        }
    }



    void FireGun()
    {
        List<BulletElectric> activeBulletList = new List<BulletElectric>();
        List<GameObject> targetObjects = GetObjectsInAttackCondition(userObject.transform.position, objectList);

        activeElectricList.Clear();
        noneTargetObjectList.Clear();
        foreach (var item in objectList)
        {
            if (item.gameObject.activeInHierarchy == false)
            {
                continue;
            }

            noneTargetObjectList.Add(item);
        }

        foreach (var target in targetObjects)
        {
            BulletElectric fireBullet = (BulletElectric)Shoot(userObject.transform.position);
            fireBullet.SetTarget(userObject, target);
            activeBulletList.Add(fireBullet);
            activeElectricList.Add(fireBullet);

            noneTargetObjectList.Remove(target);
        }

        if (activeElectricList.Count != 0)
        {
            MinusPlayerBulletCount();
            SFXPlay();
        }

        ElectricWave(noneTargetObjectList, activeBulletList, electricWaveMaxIndex);
    }


    void UpdateElectric()
    {
        foreach (var item in activeElectricList)
        {
            Vector3 electricVector = userObject.transform.position;
            electricVector.y = userObject.transform.position.y * 1.5f;

            item.SetBullet();
        }
    }

    void ElectricWave(List<GameObject> objectList, List<BulletElectric> prevBulletList,  int count)
    {
        if (count <= 0 || prevBulletList.Count == 0 || objectList.Count == 0)
        {
            return;
        }

        List<BulletElectric> nextWaveList = new List<BulletElectric>();

        foreach (var bulletObj in prevBulletList)
        {
            if (bulletObj.TargetObject() == null)
            {
                return;
            }

            Vector3 triggerPos = bulletObj.gameObject.transform.position;
            Vector3 rightVector = 
                bulletObj.gameObject.transform.right * bulletObj.gameObject.transform.localScale.x * 4.0f;
            triggerPos += rightVector;
            List<GameObject> targetObjects = GetObjectsInAttackCondition(bulletObj.TargetObject().transform.position, objectList);

            foreach (var target in targetObjects)
            {
                BulletElectric fireBullet = (BulletElectric)Shoot(triggerPos);
                fireBullet.SetTarget(bulletObj.TargetObject(), target);
                nextWaveList.Add(fireBullet);
                activeElectricList.Add(fireBullet);
                objectList.Remove(target);
            }

            ElectricWave(noneTargetObjectList, nextWaveList,--count);
        }
    }

    void StopGun()
    {
        if (activeElectricList == null)
        {
            return;
        }
        activeElectricList.Clear();
    }

    List<GameObject> GetObjectsInAttackCondition(Vector3 centerPos, List<GameObject> targetList)
    {
        List<GameObject> returnList = new List<GameObject>();
        foreach (var item in targetList)
        {
            if (CheckAttackRange(centerPos, item) && item.activeInHierarchy)
            {
                returnList.Add(item);
            }
        }

        return returnList;
    }

    bool CheckAttackRange(Vector3 waveCenterPos, GameObject targetObj)
    {
        // 거리 필터링
        Vector3 toTargetDir = targetObj.transform.position - waveCenterPos;
        if (electricWaveArea * electricWaveArea < toTargetDir.sqrMagnitude)
        {
            return false;
        }


        Vector3 toTargetDirNormalize = toTargetDir;
        toTargetDirNormalize.Normalize();

        // 방향 필터링
        float dotToTarget = Vector3.Dot(userObject.transform.forward, toTargetDirNormalize);

        if ((dotToTarget < .0f) ||
            (dotToTarget > Mathf.Sin(electricWaveAngle * Mathf.Deg2Rad)))
        {
            return false;
        }



        NPC checkNPC = targetObj.GetComponent<NPC>();
        if (checkNPC != null)
        {
            if ((checkNPC as People).isDie)
            {
                return false;
            }
        }

        #region 방향체크
        // 물체 감지 필터링 - 레이케스트를 쏘니 제일 마지막에 쓰는 것으로...
        // 퍼포먼스 상 차이가 없어서 일단 주석 처리... 퍼포먼스는 문제가 없으나 점유율이 높아진다...
        //Ray originRay = new Ray();
        //RaycastHit originRayHit = new RaycastHit();

        //originRay.origin = waveCenterPos;
        //originRay.direction = toTargetDir;
        //bool isHit = Physics.Raycast(originRay, out originRayHit, electricWaveArea);
        //Debug.DrawRay(originRay.origin, originRay.direction * toTargetDir.magnitude, Color.red, .0f);

        //if (isHit &&
        //    originRayHit.transform.gameObject != targetObj)
        //{
        //    return false;
        //}
        #endregion

        return true;
    }



    protected override void SFXPlay()
    {
        SoundManager.Instance.PlayClipToPosition(gunSound, SoundPlayMode.ObjectSFX, userObject.transform.position);
    }
}