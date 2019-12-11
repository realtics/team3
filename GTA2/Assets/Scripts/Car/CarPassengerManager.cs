using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarManager))]
public class CarPassengerManager : MonoBehaviour
{
    public CarManager carManager;
    public Transform[] doorPositions;
    public Animator[] doorAnimator;
    //public People.PeopleType passengerType = People.PeopleType.Citizen;
    public People.PeopleType[] passengers;

	//나중에 배열로 변경
	public bool isRunningOpenTheDoor = false;
	public bool isRunningCloseTheDoor = false;
	public bool[] isDoorOpen { get; set; } = new bool[3];
    
    void OnEnable()
    {
        carManager.OnDamage += OnDamaged;
        carManager.OnDestroy += OnCarDestroyInPlayer;
        carManager.OnReturnKeyDown += OnReturnKeyDown;
        carManager.OnDriverGetOn += OnDriverGetOn;
        carManager.OnDriverGetOff += OnDriverGetOff;
		StartCoroutine(DoorCheck());
    }

    void OnDisable()
    {
        carManager.OnDamage -= OnDamaged;
        carManager.OnDestroy -= OnCarDestroyInPlayer;
        carManager.OnReturnKeyDown -= OnReturnKeyDown;
        carManager.OnDriverGetOn -= OnDriverGetOn;
        carManager.OnDriverGetOff -= OnDriverGetOff;
		StopCoroutine(DoorCheck());
    }

    void OnCarDestroyInPlayer(bool sourceIsPlayer)
    {
        foreach (var p in passengers)
        {
            if (carManager.carState == CarManager.CarState.controlledByPlayer)
                GameManager.Instance.player.Hurt(9999);
        }
    }
    void OnReturnKeyDown()
    {
        if(passengers[0] == People.PeopleType.Player)
        {
			StartCoroutine(GetOffTheCar(0));
		}
    }
	public IEnumerator DoorCheck()
	{
		while (true)
		{
			yield return new WaitForSeconds(3.0f);
			if (passengers[0] == People.PeopleType.None)
				continue;

			for (int i = 0; i < doorAnimator.Length; i++)
			{
				if (isDoorOpen[i])
					doorAnimator[i].SetTrigger("Close");
			}
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
	public void GetOnTheCar(People.PeopleType passengerType, int idx = 0, bool isBustedCar = false)
	{
		if (carManager.carState == CarManager.CarState.destroied)
			return;
		passengers[idx] = passengerType;

        if (passengerType == People.PeopleType.Player || isBustedCar)
		{
            GameManager.Instance.playerCar = this;
            GameManager.Instance.player.PlayerDriverSetting(true, idx);
            
			CameraController.Instance.ChangeTarget(gameObject);
			CameraController.Instance.SetTrackingMode(CameraController.TrackingMode.car);
		}
		carManager.OnDriverGetOnEvent(passengerType, idx);
	}
	
	public IEnumerator GetOffTheCar(int idx)
    {
        if (passengers[idx] == People.PeopleType.None)
            yield break;

        doorAnimator[idx].SetTrigger("Open");
        yield return new WaitForSeconds(0.5f);
        doorAnimator[idx].SetTrigger("Close");

        switch (passengers[idx])
        {
            case People.PeopleType.Player:
                GameManager.Instance.player.PlayerDriverSetting(false, idx);
                CameraController.Instance.ChangeTarget(GameManager.Instance.player.gameObject);
                CameraController.Instance.SetTrackingMode(CameraController.TrackingMode.human);
				GameManager.Instance.player.transform.position = doorPositions[idx].position;
				GameManager.Instance.playerCar = null;
                break;
            case People.PeopleType.Citizen:
                GameObject citizenDriver = PoolManager.SpawnObject(NPCSpawnManager.Instance.citizen.gameObject);
                citizenDriver.transform.position = doorPositions[idx].position;
                break;
            case People.PeopleType.Police:
                GameObject policeDriver = PoolManager.SpawnObject(NPCSpawnManager.Instance.police.gameObject);
                policeDriver.transform.position = doorPositions[idx].position;
                break;
            default:
                break;
        }
		
        passengers[idx] = People.PeopleType.None;
        carManager.OnDriverGetOffEvent(passengers[idx], idx);
    }
    
	//OpenTheDoor(0) 먼저 호출후 확인하고 운전자 끌어내리기
    public void PullOutDriver(int idx = 0)//운전자 끌어내리기.
    {
        if (passengers[idx] == People.PeopleType.Player)
        {
			//DriverSetting(0, false);
			CameraController.Instance.ChangeTarget(GameManager.Instance.player.gameObject);
			CameraController.Instance.SetTrackingMode(CameraController.TrackingMode.human);
            GameManager.Instance.player.Down();
			passengers[idx] = People.PeopleType.None;
			carManager.OnDriverGetOffEvent(passengers[idx], idx);
		}
        else //시민
        {
            if(passengers[idx] != People.PeopleType.None)
            {
                GameObject citizenDriver = PoolManager.SpawnObject(NPCSpawnManager.Instance.citizen.gameObject);
                
                passengers[idx] = People.PeopleType.None;
                citizenDriver.transform.position = doorPositions[idx].position;
                citizenDriver.GetComponent<Citizen>().Down();

				WantedLevel.instance.CommitCrime(WantedLevel.CrimeType.stealCar, transform.position);

                DebugX.Log("차뺏기");
            }
        }
    }
	bool IsPassengerAllEmpty()
	{
		int i = 0;
		for (i = 0; i < passengers.Length; i++)
		{
			if (passengers[i] != People.PeopleType.None)
				break;
		}
		if (i == passengers.Length - 1 )
			return true;
		else
			return false;
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
    public void passengersInit()
    {
        switch (carManager.carType)
        {
            case CarManager.CarType.citizen:
                passengers[0] = People.PeopleType.Citizen;
                break;
            case CarManager.CarType.police:
                passengers[0] = People.PeopleType.Police;
                passengers[1] = People.PeopleType.Police;
                break;
            default:
                passengers[0] = People.PeopleType.Citizen;
                break;
        }
    }
}
