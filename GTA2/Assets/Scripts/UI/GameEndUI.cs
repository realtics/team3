using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameEndUI : MonoBehaviour
{
    [SerializeField]
    Sprite wastedSprite;

    [SerializeField]
    Sprite bustedSprite;

    [SerializeField]
    Sprite gameOverSprite;


    float heightSize = 200.0f;
    Image textImage;

    // Start is called before the first frame update
    void Start()
    {
        textImage = GetComponentInChildren<Image>();
        TurnOffEndUI();
    }


    public void TurnOnWastedSprite()
    {
        textImage.sprite = wastedSprite;
        textImage.enabled = true;
        SetSize();
    }
    public void TurnOnBustedSprite()
    {
        textImage.sprite = bustedSprite;
        textImage.enabled = true;
        SetSize();
    }
    public void TurnOnGameOverSprite()
    {
        textImage.sprite = gameOverSprite;
        textImage.enabled = true;
        SetSize();
    }

    public void TurnOffEndUI()
    {
        textImage.enabled = false;
    }

    void SetSize()
    {
        // 종횡비를 맞춰서 이미지를 늘린다.
        float plusSize = heightSize / textImage.sprite.rect.height;
        textImage.rectTransform.sizeDelta = new Vector2(
            textImage.sprite.rect.width * plusSize,
            heightSize);
    }
}
