using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemStatus
{
    // 아이템
    Heath,
    Armor,

    // 총
    PistolGun,
    DoublePistolGun,
    MachineGun,
    SleepMachineGun,
    RocketLauncherGun,
    ElectricGun,
    ShotGun,
    FireGun,
    FireBottleGun,
    GranadeGun,

    // TODO: 아래는 차 아이템 구현할 것
}


public class Item : MonoBehaviour
{
    // Start is called before the first frame update
    public ItemStatus itemType;
    public int itemCount;

    [SerializeField]
    Sprite[] spriteAnimation;

    SpriteRenderer mySpriteRender;
    float animationTime = .3f;
    float animationDelta = .0f;
    int aniIdx = 0;

    void Start()
    {
        mySpriteRender = GetComponent<SpriteRenderer>();
    }

    void Update()
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
}
