using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DebugPanelUI : MonoBehaviour
{
	[Header("Debug")]
	public GameObject debugPanel;
	public Reporter debugReporter;

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
		GameManager.Instance.IncreaseMoney(100000);
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

	public void PopUPRepoter()
	{
		debugReporter.ShowReporter();
	}

	public void GiveWeapon()
	{
		foreach (var g in GameManager.Instance.player.gunList)
		{
			g.bulletCount += 999;
		}
	}
	public void PlayerBust()
	{
		GameManager.Instance.player.isBusted = true;
		GameManager.Instance.player.Hurt(9999);
	}
	public void PlayerWast()
	{
		GameManager.Instance.player.Hurt(9999);
	}
	public void AddPlayerCarHealth()
	{
		if (GameManager.Instance.playerCar != null)
		{
			GameManager.Instance.playerCar.carManager.damage.AddCheatHp();
		}
	}
}
