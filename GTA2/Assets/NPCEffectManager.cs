using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCEffectManager : MonoSingleton<NPCEffectManager>
{
	public GameObject BloodAnim;
	public List<GameObject> BloodAnimList;
	public GameObject burnAnim;
	public List<GameObject> BurnAnimList;

	private void Awake()
	{
		PoolManager.WarmPool(BloodAnim.gameObject, 10);
		BloodAnimList.AddRange(PoolManager.GetAllObject(BloodAnim.gameObject));

		PoolManager.WarmPool(burnAnim.gameObject, 50);
		BurnAnimList.AddRange(PoolManager.GetAllObject(burnAnim.gameObject));
	}
	public GameObject SpawnBloodEffect(GameObject people)
	{
		GameObject insNPC = PoolManager.SpawnObject(BloodAnim.gameObject, people.transform.position, Quaternion.identity);
		insNPC.transform.SetParent(people.transform);
		return insNPC;
	}
	public void ReleaseBloodEffect(GameObject bloodEffect)
	{
		PoolManager.ReleaseObject(bloodEffect);
	}
	public GameObject SpawnBurnedEffect(GameObject people)
	{
		GameObject insNPC = PoolManager.SpawnObject(burnAnim.gameObject, people.transform.position, Quaternion.identity);
		insNPC.transform.Rotate(90, 0, 0);
		insNPC.transform.SetParent(people.transform);
		return insNPC;
	}
	public void ReleaseBurnedEffect(GameObject burnedEffect)
	{
		PoolManager.ReleaseObject(burnedEffect);
	}
}
