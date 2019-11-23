using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GetItemText : MonoBehaviour
{
    [SerializeField]
    float heightSize;
    [SerializeField]
    float turnOnTime;

    [SerializeField]
    Sprite[] activeItemTexts;
    [SerializeField]
    Sprite[] weaponItemTexts;


    Image myImage;
    float turnOnDel;

    // Start is called before the first frame update
    void Start()
    {
        myImage = GetComponent<Image>();
        myImage.enabled = false;
        turnOnDel = .0f;
    }

    // Update is called once per frame
    void Update()
    {
        turnOnDel += Time.deltaTime;
        if (turnOnDel > turnOnTime)
        {
            myImage.enabled = false;
        }
    }

    public void SetText(ItemStatus itemState)
    {
        int itemIDX = (int)itemState;
        if (itemState > ItemStatus.ActiveItemStartIndex &&
            itemState < ItemStatus.ActiveItemEndIndex)
        {
            itemIDX -= (int)ItemStatus.ActiveItemStartIndex + 1;
            myImage.sprite = activeItemTexts[itemIDX];
        }
        else if (
            itemState > ItemStatus.GunStartIndex &&
            itemState < ItemStatus.GunEndIndex)
        {
            itemIDX -= (int)ItemStatus.GunStartIndex + 1;
            myImage.sprite = weaponItemTexts[itemIDX];
        }



        turnOnDel = .0f;
        myImage.enabled = true;


        // 종횡비를 맞춰서 이미지를 늘린다.
        float plusSize = heightSize / myImage.sprite.rect.height;
        myImage.rectTransform.sizeDelta = new Vector2(
            myImage.sprite.rect.width * plusSize,
            heightSize);
    }
}
