using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemStatus
{
    // 아이템
    ActiveItemStartIndex,
    Heath,
    Armor,
    ActiveItemEndIndex,

    // 총
    GunStartIndex,
    PistolGun,
    DoublePistolGun,
    MachineGun,
    SleepMachineGun,
    RocketLauncherGun,
    ElectricGun,
    FireGun,
    ShotGun,
    FireBottleGun,
    GranadeGun,
    GunEndIndex,

    // TODO: 아래는 차 아이템 구현할 것
}


public class Item : MonoBehaviour
{
    // Start is called before the first frame update
    public ItemStatus itemType;
    public int itemCount;
    public float RespawnTime;
    float RespawnDelta;

    [SerializeField]
    Sprite[] spriteAnimation;

    SpriteRenderer mySpriteRender;
    SphereCollider mySphereCollider;
    float animationTime = .3f;
    float animationDelta = .0f;
    int aniIdx = 0;


    Player userPlayer;
    void Start()
    {
        mySpriteRender = GetComponent<SpriteRenderer>();
        mySphereCollider = GetComponent<SphereCollider>();
        userPlayer = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    void Update()
    {
        UpdateSprite();
        UpdateRespawn();
    }

    void UpdateSprite()
    {
        animationDelta += Time.deltaTime;
        if (animationDelta > animationTime)
        {
            animationDelta = .0f;
            aniIdx++;
        }

        if (aniIdx >= spriteAnimation.Length)
        {
            aniIdx = 0;
        }

        mySpriteRender.sprite = spriteAnimation[aniIdx];
    }

    void UpdateRespawn()
    {
        RespawnDelta += Time.deltaTime;
        if (RespawnTime < RespawnDelta)
        {
            ActiveOn();

            RespawnDelta = .0f;
        }
    }

    void ActiveOn()
    {
        mySpriteRender.enabled = true;
        mySphereCollider.enabled = true;
    }

    void ActiveOff()
    {
        RespawnDelta = .0f;
        mySpriteRender.enabled = false;
        mySphereCollider.enabled = false;
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject != userPlayer.gameObject)
        {
            return;
        }


        // 해당아이템이 총 아이템일 경우
        if (itemType > ItemStatus.GunStartIndex && itemType < ItemStatus.GunEndIndex)
        {
            userPlayer.gunList[(int)itemType - (int)ItemStatus.GunStartIndex].bulletCount += itemCount;
            ActiveOff();
        }
    }
}
