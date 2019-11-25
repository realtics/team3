using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponImage : MonoBehaviour
{
    [SerializeField]
    Sprite[] gunSpriteList;
    [SerializeField]
    Text myText;

    Image myImage;

    // Start is called before the first frame update
    void Start()
    {
        myImage = GetComponent<Image>();
    }

    public void SetGunSprite(GunState gs, int bulletCount)
    {
        if (gs == GunState.None)
        {
            myImage.color = new Color(1.0f, 1.0f, 1.0f, .0f);
            myText.text = "";
            return;
        }

        int plusSize = 3;
        myImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        myImage.sprite = gunSpriteList[((int)gs) - 1];
        myImage.rectTransform.sizeDelta = new Vector2(
            myImage.sprite.rect.width * plusSize, 
            myImage.sprite.rect.height * plusSize);

        myText.text = bulletCount.ToString();



        SetPosition(gs);
    }



    void SetPosition(GunState gs)
    {
        if (gs == GunState.FireBottle || gs == GunState.Granade)
        {
            myImage.rectTransform.anchoredPosition = new Vector2(-60, -247);
        }

        else
        {
            // 가상 이상적인 좌표
            myImage.rectTransform.anchoredPosition = new Vector2(-30, -247);
        }
    }
}
