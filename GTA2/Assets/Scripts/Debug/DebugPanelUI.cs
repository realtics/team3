using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugPanelUI : MonoBehaviour
{
    public GameObject debugPanel;

    public void ToggleDebugPanel()
    {
        debugPanel.SetActive(!debugPanel.activeSelf);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AddMoney()
    {
        GameManager.Instance.IncreaseMoney(1000);
    }

    public void IncreaseWantedLevel()
    {
		WantedLevel.instance.IncreaseWantedLevel();
    }

    public void ResetWantedLevel()
    {
		WantedLevel.instance.ResetWantedLevel();
    }

    public void AddPlayerHealth()
    {
        GameManager.Instance.player.Hurt(-9999);
    }

    public void TPplayer()
    {
        GameManager.Instance.player.transform.position = new Vector3(-14.8f, 0, -21);
    }

    public void GiveWeapon()
    {
        foreach (var g in GameManager.Instance.player.gunList)
        {
            g.bulletCount += 999;
        }
    }
}
