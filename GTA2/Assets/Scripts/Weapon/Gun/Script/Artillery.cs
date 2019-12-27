using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artillery : PlayerGun
{
    // Start is called before the first frame update
    [Header("Smoke Prefab")]
    public GameObject smokePref;


    CarManager carManager;
    bool changeArtDirKey;
    bool changeArtDirBtn;


    bool changeArtDirKeyRev;
    bool changeArtDirBtnRev;



    void Awake()
    {
        Init();
        carManager = GetComponentInParent<CarManager>();
    }
    

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        bulletCount = 1000;
        switch (carManager.carState)
        {
            case CarManager.CarState.idle:
                break;
            case CarManager.CarState.controlledByPlayer:
                UpdateArtDirKey();
                UpdateArtilleryDirection();
                UpdateArtillery();
                break;
            case CarManager.CarState.controlledByAi:
                break;
            case CarManager.CarState.destroied:
                break;
            default:
                break;
        }
    }


    void UpdateArtillery()
    {
        if (userObject == null || userObject == player.gameObject)
        {
            userObject = gameObject;
        }

        base.FixedUpdate();
    }

    protected override void UpdateShot()
    {
        if (isKeyShot || isButtonShot)
        {
            if (shootInterval < shootDelta && bulletCount > 0)
            {
                Bullet shotBullet = ShootSingleBullet(
                    transform.parent.transform.position + 
                    new Vector3(gunDir.x, .0f, gunDir.z)  * .4f);

                RocketSmoke shotEffect =
                    PoolManager.SpawnObject(smokePref).GetComponent<RocketSmoke>();

                shotEffect.SetTargetbullet(shotBullet.gameObject);
                MinusPlayerBulletCount();
                shootDelta = .0f;
            }
        }
    }
   
    void UpdateArtDirKey()
    {
        if (Input.GetKey(KeyCode.Tab) && Input.GetKey(KeyCode.RightArrow))
        {
            changeArtDirKey = true;
        }
        else
        {
            changeArtDirKey = false;
        }

        if (Input.GetKey(KeyCode.Tab) && Input.GetKey(KeyCode.LeftArrow))
        {
            changeArtDirKeyRev = true;
        }
        else
        {
            changeArtDirKeyRev = false;
        }
    }
    void UpdateArtilleryDirection()
    {
        if (changeArtDirBtn || changeArtDirKey)
        {
            transform.eulerAngles += new Vector3(.0f, Time.deltaTime * 60.0f); 
        }

        if (changeArtDirBtnRev || changeArtDirKeyRev)
        {
            transform.eulerAngles -= new Vector3(.0f, Time.deltaTime * 60.0f);
        }
    }


    public void UpdateArtDirRevButtonDown()
    {
        changeArtDirBtnRev = true;
    }

    public void UpdateArtDirRevButtonUp()
    {
        changeArtDirBtnRev = false;
    }

    public void UpdateArtDirButtonDown()
    {
        changeArtDirBtn = true;
    }

    public void UpdateArtDirButtonUp()
    {
        changeArtDirBtn = false;
    }
}