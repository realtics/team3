using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public enum SignalColor
    {
        Green, Red, Yellow
    }
    public SignalColor signalColor = SignalColor.Red;

    public SpriteRenderer fakeLight;
    public GameObject lightObjGreen;
	public GameObject lightObjYellow;
	public GameObject lightObjRed;
    public BoxCollider boxColliderForCar;
    public BoxCollider boxColliderForPed1;
    public BoxCollider boxColliderForPed2;

    public void ToggleSignal()
    {
		StopAllCoroutines();
		StartCoroutine(ToggleSignalCor());
    }

	IEnumerator ToggleSignalCor()
	{
		if (signalColor == SignalColor.Green)
		{
			signalColor = SignalColor.Yellow;
			fakeLight.color = new Color(1, 1, 0, 0.2f);
			lightObjGreen.SetActive(false);
			lightObjYellow.SetActive(true);
			lightObjRed.SetActive(false);

			boxColliderForCar.enabled = true;

			yield return new WaitForSeconds(2f);

			signalColor = SignalColor.Red;
			fakeLight.color = new Color(1, 0, 0, 0.2f);
			lightObjGreen.SetActive(false);
			lightObjYellow.SetActive(false);
			lightObjRed.SetActive(true);
			
			boxColliderForPed1.enabled = false;
			boxColliderForPed2.enabled = false;
		}
		else
		{
			yield return new WaitForSeconds(2f);

			signalColor = SignalColor.Green;
			fakeLight.color = new Color(0, 1, 0, 0.2f);
			lightObjGreen.SetActive(true);
			lightObjYellow.SetActive(false);
			lightObjRed.SetActive(false);

			boxColliderForPed1.enabled = true;
			boxColliderForPed2.enabled = true;
			boxColliderForCar.enabled = false;

		}
	}

	void OnDrawGizmos()
    {
        if(signalColor == SignalColor.Green)
        {
            Gizmos.color = Color.green;
        }
		else if(signalColor == SignalColor.Yellow)
		{
			Gizmos.color = Color.yellow;
		}
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawSphere(transform.position, 0.3f);
    }
}
