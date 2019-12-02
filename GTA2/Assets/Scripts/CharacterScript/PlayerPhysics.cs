using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    Rigidbody myRigidBody;
    
    Transform carDoorTransform;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Car") && GameManager.Instance.player.isChasingCar)
        {
            GameManager.Instance.player.Jump();
        }
    }
    
    public void ChaseTheCar(float moveSpeed)
    {
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
    public bool InStealingDistance()
    {
        if (Vector3.Distance(transform.position, carDoorTransform.position) < 0.3f)
            return true;
        else
            return false;
    }
    public void SetCarDoorTransform(Transform carDoorTransform)
    {
        this.carDoorTransform = carDoorTransform;
    }
}