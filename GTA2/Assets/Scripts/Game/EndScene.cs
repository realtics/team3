using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScene : MonoBehaviour
{
    public void OnClickRePlayButton()
    {
        SceneManager.LoadScene(1);
    }

    public void OnClickExitButton()
    {
        SceneManager.LoadScene(0);
    }
}
