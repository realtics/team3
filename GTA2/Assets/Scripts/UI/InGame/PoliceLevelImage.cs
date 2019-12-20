using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PoliceLevelImage : MonoBehaviour
{
    [Header("UI Component")]
    [SerializeField]
    Sprite[] sprites;
    [SerializeField]
    Image image;
    [SerializeField]
    RectTransform rectTransform;
    
    float spriteChangeTime = .05f;
    float spriteChangeDelta;
    int spriteChangeIndex;

    // Start is called before the first frame update
    void Awake()
    {
        spriteChangeIndex = 0;
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
}
