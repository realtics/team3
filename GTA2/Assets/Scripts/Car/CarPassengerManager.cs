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
    public bool isLeftDoorOpen { get; set; }
    public bool isRightDoorOpen { get; set; }
    
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
    
    public void GetOnTheCar(People people, int idx)
    {
        if (carManager.carState == CarManager.CarState.destroied)
            return;
        isLeftDoorOpen = false;

        passengers[idx] = people;
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
    
    public void PullOutDriver()//운전자 끌어내리기.
    {
        if (passengers[0] == GameManager.Instance.player)
        {
            passengers[0].transform.position = doorPositions[0].position;
            passengers[0].transform.SetParent(null);
            passengers[0].gameObject.SetActive(true);
            passengers[0].Down();
        }
        else //시민
        {
            GameManager.Instance.IncreaseWantedLevel(1.0f); // 임시.
            if(passengers[0] != null)
            {
                DriverSetting(0, false);
                passengers[0].Down();
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
