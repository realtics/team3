using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GetItemText : MonoBehaviour
{
    [Header("Value")]
    [SerializeField]
    float heightSize;
    [SerializeField]
    float turnOnTime;

    [Header("UI Component")]
    [SerializeField]
    Sprite[] activeItemTexts;
    [SerializeField]
    Sprite[] weaponItemTexts;
    [SerializeField]
    Image image;

    float turnOnDel;

    // Start is called before the first frame update
    void Start()
    {
        image.enabled = false;
        turnOnDel = .0f;
    }

    // Update is called once per frame
    void Update()
    {
        turnOnDel += Time.deltaTime;
        if (turnOnDel > turnOnTime)
        {
            image.enabled = false;
        }
    }

    public void SetText(ItemStatus itemState)
    {
        int itemIDX = (int)itemState;
        if (itemState > ItemStatus.ActiveItemStartIndex &&
            itemState < ItemStatus.ActiveItemEndIndex)
        {
            itemIDX -= (int)ItemStatus.ActiveItemStartIndex + 1;
            image.sprite = activeItemTexts[itemIDX];
        }
        else if (
            itemState > ItemStatus.GunStartIndex &&
            itemState < ItemStatus.GunEndIndex)
        {
            itemIDX -= (int)ItemStatus.GunStartIndex + 1;
            image.sprite = weaponItemTexts[itemIDX];
        }



        turnOnDel = .0f;
        image.enabled = true;


        // 종횡비를 맞춰서 이미지를 늘린다.
        float plusSize = heightSize / image.sprite.rect.height;
        image.rectTransform.sizeDelta = new Vector2(
            image.sprite.rect.width * plusSize,
            heightSize);
    }
}
