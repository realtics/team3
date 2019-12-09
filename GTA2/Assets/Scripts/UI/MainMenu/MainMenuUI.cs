using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField]
    Canvas startCanvas;
    [SerializeField]
    Canvas selectCanvas;


    [SerializeField]
    Image activeStageImage;
    [SerializeField]
    Text activeStageText;
    [SerializeField]
    Sprite[] stageSprites;
    [SerializeField]
    Text highscoreText;
    [SerializeField]
    GameObject exitUI;



    int stageIndex = 1;
    int maxStageIndex = 3;
    int minStageIndex = 1;

    // Start is called before the first frame update
    void Awake()
    {
        CloseExitWindow();
        GotoStart();
        SetHighscore();
    }


    void SetHighscore()
    {
        JsonStreamer js = new JsonStreamer();
        HighScoreData highData = js.Load<HighScoreData>("HighScoreData.json");
        highscoreText.text = "HIGH SCORE: ";


        if (highData != null)
        {
            highscoreText.text += highData.highScore.ToString();
        }
        else
        {
            highscoreText.text += "0";
        }
    }

    public void GotoStart()
    {
        startCanvas.gameObject.SetActive(true);
        selectCanvas.gameObject.SetActive(false);
    }

    public void GotoSelect()
    {
        startCanvas.gameObject.SetActive(false);
        selectCanvas.gameObject.SetActive(true);
    }


    public void NextStageBtn()
    {
        stageIndex++;
        if (stageIndex >= maxStageIndex)
        {
            stageIndex = maxStageIndex;
        }
    }
    public void PrevStageBtn()
    {
        stageIndex--;
        if (stageIndex <= minStageIndex)
        {
            stageIndex = minStageIndex;
        }
    }

    public void StartGameBtn()
    {
        SceneManager.LoadScene("Stage" + stageIndex.ToString());
    }


    void Update()
    {
        UpdateStageInfo();
        UpdateMenu();
        UpdateExit();
    }

    void UpdateStageInfo()
    {
        activeStageImage.sprite = stageSprites[stageIndex - 1];
        activeStageText.text = stageIndex.ToString();
    }

    void UpdateMenu()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            if (startCanvas.gameObject.activeInHierarchy)
            {

            }
            else if (selectCanvas.gameObject.activeInHierarchy)
            {
                GotoStart();
            }
        }
    }
    void UpdateExit()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (exitUI.activeInHierarchy)
            {
                CloseExitWindow();
            }
            else if (!exitUI.activeInHierarchy)
            {
                exitUI.SetActive(true);
            }
        }
    }


    public void CloseExitWindow()
    {
        exitUI.SetActive(false);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
