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



    Gta2Data gta2GetData;
    HighScoreData highScoreData;

    void Awake()
    {
        AwakeGta2Data();
        AwakeHighScore();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnClickExitButton();
        }
    }

    void AwakeGta2Data()
    {
        JsonStreamer js = new JsonStreamer();
        gta2GetData = js.Load<Gta2Data>("Gta2Data.json");

        if (gta2GetData == null)
        {
            gta2GetData = new Gta2Data();
            gta2GetData.money = 0;
            gta2GetData.kills = 0;
            gta2GetData.gameTime = 0;

            js.Save(highScoreData, "Gta2Data.json");
            gta2GetData = js.Load<Gta2Data>("Gta2Data.json");
        }


        moneyText.text = gta2GetData.money.ToString();
        var ts = TimeSpan.FromSeconds(gta2GetData.gameTime);
        timeText.text = string.Format("{0:00}M {1:00}S", ts.Minutes, ts.Seconds);
        killsText.text = gta2GetData.kills.ToString();
    }
    void AwakeHighScore()
    {
        JsonStreamer js = new JsonStreamer();
        highScoreData = js.Load<HighScoreData>("HighScoreData.json");

        if (highScoreData == null)
        {
            highScoreData = new HighScoreData();
            highScoreData.highScore = 0;
            js.Save(highScoreData, "HighScoreData.json");
            highScoreData = js.Load<HighScoreData>("HighScoreData.json");
        }

        saveHighscoreText.text = CalculateHighScore(gta2GetData.money).ToString();
    }


    int CalculateHighScore(int score)
    {
        if (highScoreData.highScore < score)
        {
            JsonStreamer js = new JsonStreamer();
            highScoreData = new HighScoreData();
            highScoreData.highScore = score;
            js.Save(highScoreData, "HighScoreData.json");
        }
        else
        {
            score = highScoreData.highScore;
        }

        return score;
    }


    public void OnClickRePlayButton()
    {
        SceneManager.LoadScene(gta2GetData.curScene);
    }

    public void OnClickExitButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void OnClickNextButton()
    {
        SceneManager.LoadScene(gta2GetData.nextScene);
    }
}
