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
        carManager.OnDestroy += OnCarDestroy;
        carManager.OnReturnKeyDown += OnReturnKeyDown;
    }

    void OnDisable()
    {
        carManager.OnDestroy -= OnCarDestroy;
        carManager.OnReturnKeyDown -= OnReturnKeyDown;
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
    
    public void GetOnTheCar(People people)
    {
        if (carManager.carState == CarManager.CarState.destroied)
            return;
        isLeftDoorOpen = false;
        passengers[0] = people;
        passengers[0].GetComponent<Rigidbody>().isKinematic = true;
        passengers[0].GetComponent<BoxCollider>().enabled = false;
        people.GetComponentInChildren<SpriteRenderer>().enabled = false;
        people.transform.SetParent(transform);

        if (people == GameManager.Instance.player)
        {
            CameraController.Instance.ChangeTarget(gameObject);
            CameraController.Instance.SetTrackingMode(CameraController.TrackingMode.car);
        }
        carManager.OnDriverGetOnEvent(people);
    }

    public IEnumerator GetOffTheCar(int idx)
    {
        doorAnimator[idx].SetTrigger("Open");
        yield return new WaitForSeconds(0.5f);
        doorAnimator[idx].SetTrigger("Close");
        if (passengers[idx] == null)
            yield break;

        passengers[idx].transform.SetParent(null);
        passengers[idx].GetComponentInChildren<SpriteRenderer>().enabled = true;

        passengers[0].GetComponent<Rigidbody>().isKinematic = false;
        passengers[0].GetComponent<BoxCollider>().enabled = true;
        passengers[idx].isDriver = false;
        
        //passengers[idx].transform.position = doorPositions[idx].position;

        if (passengers[idx] == GameManager.Instance.player)
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
            passengers[0].transform.position = doorPositions[0].position;
            passengers[0].transform.SetParent(null);
            passengers[0].Down();
            passengers[0].gameObject.SetActive(true);
        }
        else
        {
            GameManager.Instance.IncreaseWantedLevel(1.0f); // 임시.
            if(passengers[0] != null)
            {
                passengers[0].gameObject.transform.SetParent(null);
                passengers[0].gameObject.transform.position = doorPositions[0].position;
                //new Vector3(doorPositions[0].position.x, 1, doorPositions[0].position.z);
                passengers[0].gameObject.SetActive(true);
                
                passengers[0].Down();
            }
            //passengers[0].gameObject.transform.SetParent(GameObject.FindWithTag("SpawnManager"));
            //NPCSpawnManager.Instance.carDriverPool[0].Down();
        }
    }
}
