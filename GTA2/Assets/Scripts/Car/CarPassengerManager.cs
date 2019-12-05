using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarManager))]
public class CarPassengerManager : MonoBehaviour
{
    public CarManager carManager;

    public Transform[] doorPositions;
    public Animator[] doorAnimator;
    public People[] passengers;

	//나중에 배열로 변경
	public bool isRunningOpenTheDoor = false;
	public bool isRunningCloseTheDoor = false;
	public bool[] isDoorOpen { get; set; } = new bool[2];
    
    void OnEnable()
    {
        carManager.OnDamage += OnDamaged;
        carManager.OnDestroy += OnCarDestroy;
        carManager.OnReturnKeyDown += OnReturnKeyDown;
        carManager.OnDriverGetOn += OnDriverGetOn;
        carManager.OnDriverGetOff += OnDriverGetOff;
    }

    void OnDisable()
    {
        carManager.OnDamage -= OnDamaged;
        carManager.OnDestroy -= OnCarDestroy;
        carManager.OnReturnKeyDown -= OnReturnKeyDown;
        carManager.OnDriverGetOn -= OnDriverGetOn;
        carManager.OnDriverGetOff -= OnDriverGetOff;
    }

    void OnCarDestroy(bool sourceIsPlayer)
    {
        foreach (var p in passengers)
        {
            if (p != null)
                p.Hurt(9999);
        }
    }
    void OnReturnKeyDown()
    {
        if(passengers[0] == GameManager.Instance.player)
        {
            StartCoroutine(GetOffTheCar(0));
        }
    }
    
   

	public IEnumerator OpenTheDoor(int idx)
	{
		isRunningOpenTheDoor = true;
		doorAnimator[idx].SetTrigger("Open");
		yield return new WaitForSeconds(0.5f);
		isDoorOpen[idx] = true;
	}
	public IEnumerator CloseTheDoor(int idx)
	{
		isRunningOpenTheDoor = false;
		doorAnimator[idx].SetTrigger("Close");
		yield return new WaitForSeconds(0.5f);
		isDoorOpen[idx] = false;
	}
	public void GetOnTheCar(People people, int idx)
	{
		if (carManager.carState == CarManager.CarState.destroied)
			return;
		isDoorOpen[0] = false;

		passengers[idx] = people;
		people.isDriver = true;
		DriverSetting(idx, true);

		if (people == GameManager.Instance.player)
		{
			CameraController.Instance.ChangeTarget(gameObject);
			CameraController.Instance.SetTrackingMode(CameraController.TrackingMode.car);
		}
		carManager.OnDriverGetOnEvent(people, idx);
	}
	public IEnumerator GetOffTheCar(int idx)
    {
        if (passengers[idx] == null)
            yield break;

        doorAnimator[idx].SetTrigger("Open");
        yield return new WaitForSeconds(0.5f);
        doorAnimator[idx].SetTrigger("Close");

        DriverSetting(idx, false);

        if (passengers[idx] == GameManager.Instance.player)
        {
            CameraController.Instance.ChangeTarget(passengers[idx].gameObject);
            CameraController.Instance.SetTrackingMode(CameraController.TrackingMode.human);
        }

		passengers[idx] = null;

        carManager.OnDriverGetOffEvent(passengers[idx], idx);
    }
    
	//OpenTheDoor(0) 먼저 호출후 확인하고 운전자 끌어내리기
    public void PullOutDriver()//운전자 끌어내리기.
    {
        if (passengers[0] == GameManager.Instance.player)
        {
			DriverSetting(0, false);

			CameraController.Instance.ChangeTarget(passengers[0].gameObject);
			CameraController.Instance.SetTrackingMode(CameraController.TrackingMode.human);
			passengers[0].Down();
			passengers[0] = null;

			carManager.OnDriverGetOffEvent(passengers[0], 0);
			
		}
        else //시민
        {
            if(passengers[0] != null)
            {
                DriverSetting(0, false);
                passengers[0].Down();

				WantedLevel.instance.CommitCrime(WantedLevel.CrimeType.stealCar, transform.position);

                DebugX.Log("차뺏기");
            }
        }
    }
    void DriverSetting(int idx, bool GetOn) //orGetoff
    {
        if (GetOn)
        {
            passengers[idx].GetComponent<Rigidbody>().isKinematic = true;
            passengers[idx].GetComponent<BoxCollider>().enabled = false;
            passengers[idx].GetComponentInChildren<SpriteRenderer>().enabled = false;
            passengers[idx].transform.SetParent(transform);
        }
        else if(passengers[idx] != null)
        {
            passengers[idx].transform.position = doorPositions[0].transform.position;
            passengers[idx].GetComponentInChildren<SpriteRenderer>().enabled = true;
            passengers[idx].GetComponent<Rigidbody>().isKinematic = false;
            passengers[idx].GetComponent<BoxCollider>().enabled = true;
            passengers[idx].isDriver = false;
            passengers[idx].transform.SetParent(null);
        }

    }
    void OnDriverGetOn(People people, int idx)
    {

    }

    void OnDriverGetOff(People people, int idx)
    {

    }

    void OnDamaged(bool sourceIsPlayer)
    {
        if (carManager.movement.curSpeed < 20.0f)
        {
            if (passengers[0] == null)
                return;
            if (passengers[0] == GameManager.Instance.player)
                return;
                for (int i = 0; i < doorPositions.Length; i++)
            {
                StartCoroutine(GetOffTheCar(i));
            }
        }
    }
}
