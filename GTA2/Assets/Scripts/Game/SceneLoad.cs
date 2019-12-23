using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour
{
	public Image loadingBar;

    void Start()
    {
		int targetSceneIdx = PlayerPrefs.GetInt("TargetSceneIdx");
		StartCoroutine(LoadScene(targetSceneIdx));
	}
    
	IEnumerator LoadScene(int idx)
	{
		AsyncOperation asyncOper = SceneManager.LoadSceneAsync(idx);
		//asyncOper.allowSceneActivation = false;

		while (!asyncOper.isDone)
		{
			loadingBar.fillAmount = asyncOper.progress;
			yield return null;
		}
	}
}
