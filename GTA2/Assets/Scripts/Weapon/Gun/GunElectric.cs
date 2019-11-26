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

    void Start()
    {
        gunType = GunState.Electric;
        bulletPoolCount = 50;

        objectList = new List<GameObject>();
        noneTargetObjectList = new List<GameObject>();
        activeElectricList = new List<BulletElectric>();

        foreach (var citizen in NPCSpawnManager.Instance.activeNPCList)
        {
            objectList.Add(citizen.gameObject);
        }

        foreach (var car in CarSpawnManager.Instance.activeCarList)
        {
            objectList.Add(car.gameObject);
        }

        InitGun();
        base.InitBullet("Electric");

        foreach (var item in bulletList)
        {
            ((BulletElectric)item).SetArea(electricWaveArea);
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
            MinusPlayerBulletCount();
            fireBullet.SetTarget(target);
            activeBulletList.Add(fireBullet);
            activeElectricList.Add(fireBullet);

            noneTargetObjectList.Remove(target);
        }

        if (activeElectricList.Count != 0)
        {
            SFXPlay();
        }

        ElectricWave(noneTargetObjectList, activeBulletList, electricWaveMaxIndex);
    }


    void UpdateElectric()
    {
        foreach (var item in activeElectricList)
        {
            Vector3 electricVector = item.transform.position;
            electricVector.y = userObject.transform.position.y;
        }
    }

    void ElectricWave(List<GameObject> objectList, List<BulletElectric> nextBulletList,  int count)
    {
        if (count <= 0 || nextBulletList.Count == 0 || objectList.Count == 0)
        {
            return;
        }

        List<BulletElectric> nextWaveList = new List<BulletElectric>();

        foreach (var bulletObj in nextBulletList)
        {
            Vector3 triggerPos = bulletObj.gameObject.transform.position;
            Vector3 rightVector = 
                bulletObj.gameObject.transform.right * bulletObj.gameObject.transform.localScale.x * 4.0f;
            triggerPos += rightVector;
            List<GameObject> targetObjects = GetObjectsInAttackRange(triggerPos, objectList);

            foreach (var target in targetObjects)
            {
                
                BulletElectric fireBullet = (BulletElectric)Shoot(triggerPos);
                fireBullet.SetTarget(target);
                nextWaveList.Add(fireBullet);
                activeElectricList.Add(fireBullet);
                objectList.Remove(target);
            }

            ElectricWave(noneTargetObjectList, nextWaveList,--count);
        }
    }

    void StopGun()
    {
        DisableAllBullet();
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