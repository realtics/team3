using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartImageList : MonoBehaviour
{
    [SerializeField]
    private Sprite heartSprite;
    private Image[] heartImgList;



    private void Start()
    {
        heartImgList = GetComponentsInChildren<Image>();
        heartImgList[0].color = new Color(1.0f, .0f, .0f, 1.0f);
    }

    // Update is called once per frame
    public void SetHealthPoint(int hp)
    {
        InitImage();
        // 20인 이유는 100을 하트 5개로 나누기 때문이다. - 하트 개당 20의 체력
        int oneHeartHp = 20;
        int heartCount = hp / oneHeartHp;

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
        if (lastHeart == 0)
        {
            heartImgList[heartCount].rectTransform.localScale = new Vector3(.7f, .7f, .7f);
        }
    }

    private void InitImage()
    {
        for (int i = 0; i < heartImgList.Length; i++)
        {
            heartImgList[i].rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            heartImgList[i].enabled = true;
        }
    }

}
