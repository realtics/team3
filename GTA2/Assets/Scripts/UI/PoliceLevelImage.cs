using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PoliceLevelImage : MonoBehaviour
{
    [SerializeField]
    Sprite[] sprites;
    [SerializeField]
    Image image;
    [SerializeField]
    RectTransform rectTransform;


    float moveHeightMaxSize = 25.0f;
    float originYPos;
    bool trueIsUp;

    float spriteChangeTime = .05f;
    float spriteChangeDelta;
    int spriteChangeIndex;

    // Start is called before the first frame update
    void Awake()
    {
        spriteChangeIndex = 0;
        originYPos = rectTransform.rect.y;
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

        if (sprites.Length <= spriteChangeIndex)
        {
            spriteChangeIndex = 0;
        }

        image.sprite = sprites[spriteChangeIndex];
    }

    void SetLevel(bool value)
    {
        if (!value)
        {
            image.sprite = null;
        }
    }
}
