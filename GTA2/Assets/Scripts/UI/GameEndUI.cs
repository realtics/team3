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
    }
    public void TurnOnBustedSprite()
    {
        textImage.sprite = bustedSprite;
        textImage.enabled = true;
    }

    public void TurnOffEndUI()
    {
        textImage.enabled = false;
    }
}
