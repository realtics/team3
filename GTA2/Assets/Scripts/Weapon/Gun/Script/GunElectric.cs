using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunElectric : PlayerGun
{
    public int electricWaveMaxIndex;
    public float electricWaveArea;
    public float electricWaveAngle;



    List<GameObject> objectList;
    List<GameObject> noneTargetObjectList;

    List<BulletElectric> activeElectricList;
    public override void Init()
    {
		base.Init();

        objectList = new List<GameObject>();
        noneTargetObjectList = new List<GameObject>();
        activeElectricList = new List<BulletElectric>();

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

        InitGun();


        List<GameObject> bulletList = PoolManager.GetAllObject(bulletPref);
        foreach (var item in bulletList)
        {
            item.GetComponent<BulletElectric>().SetAreaAndMaxCount(electricWaveArea, electricWaveMaxIndex);
        }
    }


    protected override void Update()
    {
        if (player.isDie == true)
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
        List<GameObject> targetObjects = GetObjectsInAttackRange(userObject.transform.position, objectList);

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

            item.UpdateBullet(gunType, electricVector, gunDir, .0f);
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
            Vector3 triggerPos = bulletObj.gameObject.transform.position;
            Vector3 rightVector = 
                bulletObj.gameObject.transform.right * bulletObj.gameObject.transform.localScale.x * 4.0f;
            triggerPos += rightVector;
            List<GameObject> targetObjects = GetObjectsInAttackRange(bulletObj.TargetObject().transform.position, objectList);

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
        activeElectricList.Clear();
    }

    List<GameObject> GetObjectsInAttackRange(Vector3 centerPos, List<GameObject> targetList)
    {
        List<GameObject> returnList = new List<GameObject>();
        foreach (var item in targetList)
        {
            if (CheckAttackRange(centerPos, item))
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
        // 물체 감지 필터링 - 레이케스트를 소니 제일 마지막에 쓰는 것으로...
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
}