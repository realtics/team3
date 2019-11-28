using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponImage : MonoBehaviour
{
    [SerializeField]
    Sprite[] gunSpriteList;
    [SerializeField]
    Text text;
    [SerializeField]
    Image image;

    // Start is called before the first frame update
    public void SetGunSprite(GunState gs, int bulletCount)
    {
        if (gs == GunState.None)
        {
            image.color = new Color(1.0f, 1.0f, 1.0f, .0f);
            text.text = "";
            return;
        }

        int plusSize = 3;
        image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        image.sprite = gunSpriteList[((int)gs) - 1];
        image.rectTransform.sizeDelta = image.sprite.rect.size * plusSize;
        image.preserveAspect = true;
        text.text = bulletCount.ToString();



        SetPosition(gs);
    }



    void SetPosition(GunState gs)
    {
        if (gs == GunState.FireBottle || gs == GunState.Granade)
        {
            image.rectTransform.anchoredPosition = new Vector2(-60, -247);
        }

        else
        {
            // 가상 이상적인 좌표
            image.rectTransform.anchoredPosition = new Vector2(-30, -247);
        }
    }
}
