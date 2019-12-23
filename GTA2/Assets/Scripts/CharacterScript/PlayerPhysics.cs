using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    Rigidbody myRigidBody;
	public CarPassengerManager targetCar { get; set; }
	Transform carDoorTransform;

	void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
		//속도 너무 빠르면 Runover
        if (collision.gameObject.CompareTag("Car") && GameManager.Instance.player.isChasingCar &&
			collision.gameObject.GetComponent<CarManager>().movement.curSpeed < GameManager.Instance.player.runoverMinSpeedInChasing 
			// && Vector3.SqrMagnitude(carDoorTransform.position - transform.position) > 0.1f
			)
        {
            GameManager.Instance.player.Jump();
        }
    }
    
    public void ChaseTheCar(float moveSpeed)
    {
		LookAtCarDoor();

		if (Vector3.SqrMagnitude(carDoorTransform.position - transform.position) < 0.05f)
		{
			return;
		}
        myRigidBody.MovePosition(transform.position + (transform.forward * Time.deltaTime * moveSpeed));
    }
    public void MovePositionByInput(float hDir, float vDir, float moveSpeed)
    {
		//transform.position += (new Vector3(hDir, 0, vDir).normalized * Time.deltaTime * moveSpeed);
		myRigidBody.MovePosition(transform.position + new Vector3(hDir, 0, vDir).normalized * Time.deltaTime * moveSpeed);
	}
    public bool InChasingDistance()
    {
        if (Vector3.SqrMagnitude(transform.position -carDoorTransform.position) > 25)
            return true;
        else
            return false;
    }
    public void LookAtCarDoor()
    {
        transform.LookAt(new Vector3(carDoorTransform.position.x, transform.position.y, carDoorTransform.position.z));
    }
	public void LookAtCar()
	{
		transform.LookAt(new Vector3(targetCar.transform.position.x, transform.position.y, targetCar.transform.position.z));
		DebugX.DrawRay(transform.position, (targetCar.transform.position - transform.position), Color.blue);
	}

	public bool InStealingDistance()
    {
        if (Vector3.Distance(transform.position, carDoorTransform.position) < 0.3f)
            return true;
        else
            return false;
    }
	public bool IsGetOnDistance()
	{
		if (Vector3.Distance(transform.position, carDoorTransform.position) < 1.0f)
			return true;
		else
			return false;
	}
	public void SetCarDoorTransform(Transform carDoorTransform)
    {
        this.carDoorTransform = carDoorTransform;
    }
}