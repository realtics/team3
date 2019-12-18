using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBlock : MonoBehaviour
{
	public Transform[] fencePositions;
	public RoadBlockFence[] fences;

	public Transform[] policeCarPositions;
	public LayerMask carLayerMask;

    void OnEnable()
    {
        for (int i = 0; i < fences.Length; i++)
        {
			fences[i].transform.position = fencePositions[i].position;
        }

        for (int i = 0; i < policeCarPositions.Length; i++)
        {
			RaycastHit hit;
			if(Physics.Raycast(policeCarPositions[i].position + Vector3.up, Vector3.down, 
				out hit, 3, carLayerMask))
			{
				hit.transform.gameObject.SetActive(false);
			}

			CarManager cm = CarSpawnManager.Instance.SpawnPoliceCar(policeCarPositions[i].position);

			if(cm == null)
			{
				gameObject.SetActive(false);
				return;
			}

			cm.carState = CarManager.CarState.idle;
			for (int j = 0; j < cm.passengerManager.passengers.Length; j++)
			{
				cm.passengerManager.passengers[j] = People.PeopleType.None;
			}
			cm.transform.position = policeCarPositions[i].position;
			cm.transform.forward = policeCarPositions[i].transform.forward;
        }

		StopAllCoroutines();
		StartCoroutine(DisableIfOutOfCamera());
    }

	IEnumerator DisableIfOutOfCamera()
	{
		while (true)
		{
			Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
			float offset = 2f;
			if (pos.x < 0 - offset ||
				pos.x > 1 + offset ||
				pos.y < 0 - offset ||
				pos.y > 1 + offset)
			{
				gameObject.SetActive(false);
			}				

			yield return new WaitForSeconds(1.0f);
		}
	}
}
