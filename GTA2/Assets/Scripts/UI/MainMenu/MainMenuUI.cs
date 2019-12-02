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


    int stageIndex = 1;
    int maxStageIndex = 3;
    int minStageIndex = 1;

    // Start is called before the first frame update
    void Start()
    {
        GotoStart();
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
        SceneManager.LoadScene(1);
    }


    void Update()
    {
        UpdateStageInfo();
        UpdateMenu();
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

    public void ExitGame()
    {
        Application.Quit();
    }
}
