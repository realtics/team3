using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CarDoor
{
	public Transform transform;
	public Animator animator;
	public People.PeopleType passenger;
	public CarPassengerManager.DoorState doorState;
}

[RequireComponent(typeof(CarManager))]
public class CarPassengerManager : MonoBehaviour
{
	public CarManager carManager;

	public CarDoor[] doors;
	public enum DoorState
	{
		close, opening, open, closing
	}

	void OnEnable()
	{
		carManager.OnDamage += OnDamaged;
		carManager.OnDestroy += OnCarDestroyInPlayer;
		carManager.OnReturnKeyDown += OnReturnKeyDown;
		carManager.OnDriverGetOn += OnDriverGetOn;
		carManager.OnDriverGetOff += OnDriverGetOff;

		Init();
		StartCoroutine(DoorCheck());
	}

	void OnDisable()
	{
		carManager.OnDamage -= OnDamaged;
		carManager.OnDestroy -= OnCarDestroyInPlayer;
		carManager.OnReturnKeyDown -= OnReturnKeyDown;
		carManager.OnDriverGetOn -= OnDriverGetOn;
		carManager.OnDriverGetOff -= OnDriverGetOff;

		StopAllCoroutines();
	}

	public void Init()
	{
		for (int i = 0; i < doors.Length; i++)
		{
			doors[i].passenger = People.PeopleType.None;
			doors[i].doorState = DoorState.close;
		}

		switch (carManager.carType)
		{
			case CarManager.CarType.citizen:
				doors[0].passenger = People.PeopleType.Citizen;
				break;
			case CarManager.CarType.police:
				doors[0].passenger = People.PeopleType.Police;
				doors[1].passenger = People.PeopleType.Police;
				break;
			case CarManager.CarType.ambulance:
				doors[0].passenger = People.PeopleType.Doctor;
				doors[1].passenger = People.PeopleType.Doctor;
				break;
			default:
				doors[0].passenger = People.PeopleType.Citizen;
				break;
		}
	}

	void OnCarDestroyInPlayer(bool sourceIsPlayer)
	{
		if (carManager.carState == CarManager.CarState.controlledByPlayer)
			GameManager.Instance.player.Hurt(9999);
	}

	void OnReturnKeyDown()
	{
		if (doors[0].passenger == People.PeopleType.Player)
		{
			GetOffTheCar(0);
		}
	}

	public IEnumerator DoorCheck()
	{
		while (true)
		{
			yield return new WaitForSeconds(3.0f);

			if (doors[0].passenger == People.PeopleType.None)
				continue;

			for (int i = 0; i < doors.Length; i++)
			{
				if (doors[i].doorState == DoorState.open)
					StartCoroutine(CloseTheDoor(i));
			}
		}
	}

	public IEnumerator OpenTheDoor(int idx)
	{
		if (doors[idx].doorState == DoorState.open ||
			doors[idx].doorState == DoorState.opening)
			yield break;

		carManager.OnDoorOpenEvent(idx);

		doors[idx].doorState = DoorState.opening;
		doors[idx].animator.SetTrigger("Open");

		yield return new WaitForSeconds(0.5f);

		doors[idx].doorState = DoorState.open;

		yield return new WaitForSeconds(1f);

		if (doors[0].passenger != People.PeopleType.None)
			StartCoroutine(CloseTheDoor(idx));
	}

	public IEnumerator CloseTheDoor(int idx)
	{
		if (doors[idx].doorState == DoorState.close ||
			doors[idx].doorState == DoorState.closing)
			yield break;

		carManager.OnDoorCloseEvent(idx);

		doors[idx].doorState = DoorState.closing;
		doors[idx].animator.SetTrigger("Close");

		yield return new WaitForSeconds(0.5f);

		doors[idx].doorState = DoorState.close;
	}

	void OnDriverGetOn(People.PeopleType peopleType, int idx)
	{
	}

	void OnDriverGetOff(People.PeopleType peopleType, int idx)
	{
	}

	void OnDamaged(bool sourceIsPlayer)
	{
	}

	public void GetOnTheCar(People.PeopleType passengerType, int idx = 0)
	{
		if (carManager.carState == CarManager.CarState.destroied)
			return;

		doors[idx].passenger = passengerType;

		if (passengerType == People.PeopleType.Player)
		{
			GameManager.Instance.playerCar = this;
			GameManager.Instance.player.PlayerDriverSetting(true, idx);

			CameraController.Instance.ChangeTarget(gameObject);
			CameraController.Instance.SetTrackingMode(CameraController.TrackingMode.car);
		}

		carManager.OnDriverGetOnEvent(passengerType, idx);
	}

	public void GetOffTheCar(int idx = 0, bool isRunaway = false, bool forced = false)
	{
		if (doors[idx].passenger == People.PeopleType.None)
			return;

		if (carManager.carState == CarManager.CarState.destroied)
			return;

		carManager.OnDriverGetOffEvent(doors[idx].passenger, idx);
		StartCoroutine(OpenTheDoor(idx));

		GameObject driver = null;
		switch (doors[idx].passenger)
		{
			case People.PeopleType.Player:
				GameManager.Instance.player.PlayerDriverSetting(false, idx);
				CameraController.Instance.ChangeTarget(GameManager.Instance.player.gameObject);
				CameraController.Instance.SetTrackingMode(CameraController.TrackingMode.human);
				driver = GameManager.Instance.player.gameObject;
				GameManager.Instance.playerCar = null;
				break;
			case People.PeopleType.Citizen:
				driver = PoolManager.SpawnObject(NPCSpawnManager.Instance.citizenPrefab.gameObject);
				break;
			case People.PeopleType.Police:
				driver = PoolManager.SpawnObject(NPCSpawnManager.Instance.policePrefab.gameObject);
				break;
			case People.PeopleType.Doctor:
				driver = PoolManager.SpawnObject(NPCSpawnManager.Instance.doctorPrefab.gameObject);
				driver.GetComponent<Doctor>().targetCar = carManager;
				driver.GetComponent<Doctor>().idx = idx;
				break;
			default:
				break;
		}

		if (driver != null)
		{
			driver.transform.position = doors[idx].transform.position;

			if (forced)
			{
				for (int i = 1; i < doors.Length; i++)
				{
					GetOffTheCar(i);
				}

				driver.GetComponent<People>().Down();
				WantedLevel.instance.CommitCrime(WantedLevel.CrimeType.stealCar, transform.position);
			}
		}

		doors[idx].passenger = People.PeopleType.None;
	}		
}