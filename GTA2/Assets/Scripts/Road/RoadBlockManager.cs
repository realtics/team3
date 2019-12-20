using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBlockManager : MonoBehaviour
{
	public GameObject[] roadblocks;

	void Start()
	{
		StartCoroutine(EnableRoadBlock());
	}

	IEnumerator EnableRoadBlock()
	{
		while (true)
		{
			yield return new WaitForSeconds(3f);

			if (WantedLevel.instance.level <= 1)
				continue;

			GameObject go = FindClosetRoadBlock(GameManager.Instance.player.transform.position);
			if (go == null)
				continue;

			go.SetActive(true);
		}
	}

	GameObject FindClosetRoadBlock(Vector3 origin)
	{
		GameObject closest = null;
		float closestDist = Mathf.Infinity;

		foreach (var rb in roadblocks)
		{
			if (rb.activeSelf)
				continue;

			Vector3 pos = Camera.main.WorldToViewportPoint(rb.transform.position);
			float offset = 0.5f;
			if (pos.x >= 0 - offset && pos.x <= 1 + offset && pos.y >= 0 - offset && pos.y <= 1 + offset)
				continue;

			var dist = (rb.transform.position - origin).sqrMagnitude;
			if (dist < closestDist)
			{
				closest = rb;
				closestDist = dist;
			}
		}

		return closest;
	}
}
