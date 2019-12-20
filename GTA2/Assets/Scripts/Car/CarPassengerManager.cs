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
        if(doors[0].passenger == People.PeopleType.Player)
        {
			StartCoroutine(GetOffTheCar(0));
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
		carManager.OnDoorOpenEvent(idx);

		doors[idx].doorState = DoorState.opening;
		doors[idx].animator.SetTrigger("Open");

		yield return new WaitForSeconds(0.5f);

		doors[idx].doorState = DoorState.open;
	}

	public IEnumerator CloseTheDoor(int idx)
	{
		carManager.OnDoorCloseEvent(idx);

		doors[idx].doorState = DoorState.closing;
		doors[idx].animator.SetTrigger("Close");

		yield return new WaitForSeconds(0.5f);

		doors[idx].doorState = DoorState.close;
	}

	public void GetOnTheCar(People.PeopleType passengerType, int idx = 0, bool isBustedCar = false)
	{
		if (carManager.carState == CarManager.CarState.destroied)
			return;

		doors[idx].passenger = passengerType;

        if (passengerType == People.PeopleType.Player || isBustedCar)
		{
            GameManager.Instance.playerCar = this;
            GameManager.Instance.player.PlayerDriverSetting(true, idx);
            
			CameraController.Instance.ChangeTarget(gameObject);
			CameraController.Instance.SetTrackingMode(CameraController.TrackingMode.car);
		}

		carManager.OnDriverGetOnEvent(passengerType, idx);
	}
	
	public IEnumerator GetOffTheCar(int idx = 0, bool isRunaway = false)
    {
        if (doors[idx].passenger == People.PeopleType.None)
            yield break;

		StartCoroutine(OpenTheDoor(idx));

        yield return new WaitForSeconds(0.5f);

		if (carManager.carState == CarManager.CarState.destroied)
			yield break;

		if (!isRunaway)
			StartCoroutine(CloseTheDoor(idx));

        switch (doors[idx].passenger)
        {
            case People.PeopleType.Player:
                GameManager.Instance.player.PlayerDriverSetting(false, idx);
                CameraController.Instance.ChangeTarget(GameManager.Instance.player.gameObject);
                CameraController.Instance.SetTrackingMode(CameraController.TrackingMode.human);
				GameManager.Instance.player.transform.position = doors[idx].transform.position;
				GameManager.Instance.playerCar = null;
                break;
            case People.PeopleType.Citizen:
                GameObject citizenDriver = PoolManager.SpawnObject(NPCSpawnManager.Instance.citizen.gameObject);
                citizenDriver.transform.position = doors[idx].transform.position;
                break;
            case People.PeopleType.Police:
                GameObject policeDriver = PoolManager.SpawnObject(NPCSpawnManager.Instance.police.gameObject);
                policeDriver.transform.position = doors[idx].transform.position;
                break;
			case People.PeopleType.Doctor:
				GameObject doctorDriver = PoolManager.SpawnObject(NPCSpawnManager.Instance.doctor.gameObject);
				doctorDriver.GetComponent<Doctor>().ambulanceCar = carManager;
				doctorDriver.GetComponent<Doctor>().idx = idx;
				doctorDriver.transform.position = doors[idx].transform.position;
				break;
			default:
                break;
        }
		
        doors[idx].passenger = People.PeopleType.None;
        carManager.OnDriverGetOffEvent(doors[idx].passenger, idx);
    }
    
	//OpenTheDoor(0) 먼저 호출후 확인하고 운전자 끌어내리기
    public void PullOutDriver(int idx = 0)//운전자 끌어내리기.
    {
        if (doors[idx].passenger == People.PeopleType.Player)
        {
			GameManager.Instance.player.PlayerDriverSetting(false, idx);
			CameraController.Instance.ChangeTarget(GameManager.Instance.player.gameObject);
			CameraController.Instance.SetTrackingMode(CameraController.TrackingMode.human);
			GameManager.Instance.player.transform.position = doors[idx].transform.position;
			doors[idx].passenger = People.PeopleType.None;
			carManager.OnDriverGetOffEvent(doors[idx].passenger, idx);
			GameManager.Instance.playerCar = null;
			GameManager.Instance.player.Down();
		}
		else if(doors[idx].passenger == People.PeopleType.Doctor)
		{
			GameObject doctorDriver = PoolManager.SpawnObject(NPCSpawnManager.Instance.doctor.gameObject);
			doctorDriver.GetComponent<Doctor>().ambulanceCar = carManager;
			doctorDriver.GetComponent<Doctor>().idx = 0;

			doors[idx].passenger = People.PeopleType.None;
			doctorDriver.transform.position = doors[idx].transform.position;
			doctorDriver.GetComponent<Doctor>().Down();
			

			WantedLevel.instance.CommitCrime(WantedLevel.CrimeType.stealCar, transform.position);
			DebugX.Log("차뺏기");
		}
        else //시민
        {
            if(doors[idx].passenger != People.PeopleType.None)
            {
                GameObject citizenDriver = PoolManager.SpawnObject(NPCSpawnManager.Instance.citizen.gameObject);

				doors[idx].passenger = People.PeopleType.None;
                citizenDriver.transform.position = doors[idx].transform.position;
                citizenDriver.GetComponent<Citizen>().Down();

				WantedLevel.instance.CommitCrime(WantedLevel.CrimeType.stealCar, transform.position);

                DebugX.Log("차뺏기");
            }
        }
		for(int i = 1; i < doors.Length; i++)
		{
			StartCoroutine(GetOffTheCar(i));
		}
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
}
