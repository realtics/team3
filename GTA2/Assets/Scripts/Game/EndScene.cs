using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;


public class EndScene : MonoBehaviour
{
    [SerializeField]
    Text saveHighscoreText;
    [SerializeField]
    Text moneyText;
    [SerializeField]
    Text timeText;
    [SerializeField]
    Text killsText;



    void Awake()
    {
        var ts = TimeSpan.FromSeconds(1234);
        timeText.text = string.Format("{0:00}M {1:00}S", ts.Minutes, ts.Seconds);
    }

    public void OnClickRePlayButton()
    {
        SceneManager.LoadScene(1);
    }

    public void OnClickExitButton()
    {
        SceneManager.LoadScene(0);
    }
}
