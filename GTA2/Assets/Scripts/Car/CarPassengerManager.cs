using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarManager))]
public class CarPassengerManager : MonoBehaviour
{
    public CarManager carManager;

    public Transform leftDoorPosition;
    public Transform rightDoorPosition;
    public Animator leftDoorAnimator;
    public Animator rightDoorAnimator;

    public People[] passengers;
    public bool isLeftDoorOpen { get; set; }
    public bool isRightDoorOpen { get; set; }

    void OnEnable()
    {
        carManager.OnDestroy += OnCarDestroy;
        carManager.OnReturnKeyDown += OnReturnKeyDown;
    }

    void OnDisable()
    {
        carManager.OnDestroy -= OnCarDestroy;
        carManager.OnReturnKeyDown -= OnReturnKeyDown;
    }

    void OnCarDestroy()
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
            GetOffTheCar(0);
    }

    public void GetOnTheCar(People people)
    {
        if (carManager.carState == CarManager.CarState.destroied)
            return;

        isLeftDoorOpen = false;
        passengers[0] = people;

        people.GetComponentInChildren<SpriteRenderer>().enabled = false;
        people.transform.SetParent(transform);

        if (people == GameManager.Instance.player)
        {
            CameraController.Instance.ChangeTarget(gameObject);
            CameraController.Instance.SetTrackingMode(CameraController.TrackingMode.car);
        }

        carManager.OnDriverGetOnEvent(people);
    }

    public void GetOffTheCar(int idx)
    {
        if (passengers[idx] == null)
            return;

        passengers[idx].transform.SetParent(null);
        passengers[idx].GetComponentInChildren<SpriteRenderer>().enabled = true;

        if(passengers[idx] == GameManager.Instance.player)
        {
            CameraController.Instance.ChangeTarget(passengers[idx].gameObject);
            CameraController.Instance.SetTrackingMode(CameraController.TrackingMode.human);
        }

        passengers[idx] = null;

        carManager.OnDriverGetOffEvent();
    }

    public void PullOutDriver()//운전자 끌어내리기.
    {
        if (passengers[0] == GameManager.Instance.player)
        {
            passengers[0].transform.position = leftDoorPosition.position;
            passengers[0].transform.SetParent(null);
            passengers[0].Down();
            passengers[0].gameObject.SetActive(true);
        }
        else
        {
            NPCSpawnManager.Instance.carDriverPool[0].gameObject.SetActive(true);
            NPCSpawnManager.Instance.carDriverPool[0].gameObject.transform.position = leftDoorPosition.position;
            NPCSpawnManager.Instance.carDriverPool[0].Down();
        }
    }
}
