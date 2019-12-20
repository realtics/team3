using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartImageList : MonoBehaviour
{
    Image[] heartImgList;
    int maxPlayerHp;


    void Start()
    {
        heartImgList = GetComponentsInChildren<Image>();
        heartImgList[0].color = new Color(1.0f, .0f, .0f, 1.0f);
    }

    public void SetMaxPlayerHp(int maxHp)
    {
        maxPlayerHp = maxHp;
    }


    // Update is called once per frame
    public void SetHealthPoint(int hp)
    {
        InitImage();

        // 체력 고갈 처리
        if (hp <= 0)
        {
            for (int i = 0; i < heartImgList.Length; i++)
            {
                heartImgList[i].enabled = false;
            }

            return;
        }


        int oneHeartHp = maxPlayerHp / 5;
        int heartCount = hp / oneHeartHp;


        // 체력이 가득하다.
        if (heartCount == heartImgList.Length)
        {
            return;
        }




        for (int i = heartImgList.Length; i > heartCount + 1; i--)
        {
            heartImgList[i - 1].enabled = false;
        }

        int lastHeartValue = hp - (heartCount * oneHeartHp);
        int lastHeart = ((lastHeartValue * 2) / oneHeartHp );

        // 하트가 반개
        if (lastHeart == 0 && heartCount <= heartImgList.Length)
        {
            heartImgList[heartCount].rectTransform.localScale = new Vector3(.7f, .7f, .7f);
        }
    }

    void InitImage()
    {
        for (int i = 0; i < heartImgList.Length; i++)
        {
            heartImgList[i].rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            heartImgList[i].enabled = true;
        }
    }

}
