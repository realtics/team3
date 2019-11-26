using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PoliceLevelImage : MonoBehaviour
{
    [SerializeField]
    Sprite[] mySprites;
    Image myImage;
    RectTransform myRectTransform;


    float moveHeightMaxSize = 25.0f;
    float originYPos;
    bool trueIsUp;

    float spriteChangeTime = .05f;
    float spriteChangeDelta;
    int spriteChangeIndex;

    // Start is called before the first frame update
    void Start()
    {
        myImage = GetComponent<Image>();
        spriteChangeIndex = 0;
        myRectTransform = GetComponent<RectTransform>();
        originYPos = myRectTransform.rect.y;
        trueIsUp = true;
    }

    // Update is called once per frame
    void Update()
    {
        spriteChangeDelta += Time.deltaTime;
        if (spriteChangeTime < spriteChangeDelta)
        {
            ChangeSprite();
        }
    }

    void ChangeSprite()
    {
        spriteChangeDelta = .0f;
        spriteChangeIndex++;

        if (mySprites.Length <= spriteChangeIndex)
        {
            spriteChangeIndex = 0;
        }

        myImage.sprite = mySprites[spriteChangeIndex];
    }

    void SetLevel(bool value)
    {
        if (!value)
        {
            myImage.sprite = null;
        }
    }
}
