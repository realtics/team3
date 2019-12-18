using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffect : MonoBehaviour
{
	public float RemainTime = 0.8f;
	private void OnEnable()
	{
		//시간재고 false
		Invoke(nameof(DisableThis), RemainTime);
	}
	void DisableThis()
	{
		gameObject.SetActive(false);
	}

}
