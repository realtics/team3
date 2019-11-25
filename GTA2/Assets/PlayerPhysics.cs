using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    [Header("이 오브젝트와 충돌할 레이어")]
    public LayerMask collisionLayer;
    Rigidbody myRigidBody;
    RaycastHit hit;
    Transform carDoorTransform;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Car") && GameManager.Instance.player.isChasingCar)
        {
            GameManager.Instance.player.Jump();
        }
    }
    public bool IsCarExistBelow()
    {
        if (Physics.Raycast(transform.position, transform.up * -1, out hit, 1f, collisionLayer)
            && hit.transform.CompareTag("Car"))
            return true;
        else
            return false;
    }
    public void ChaseTheCar(float moveSpeed)
    {
        myRigidBody.MovePosition(transform.position + (transform.forward * Time.deltaTime * moveSpeed));
    }
    public void MovePositionByInput(float hDir, float vDir, float moveSpeed)
    {
        transform.position += (new Vector3(hDir, 0, vDir).normalized * Time.deltaTime * moveSpeed);
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