using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

[RequireComponent(typeof(CarManager))]
public class CarController : MonoBehaviour
{
    public CarManager carManager;

    Rigidbody rbody;

    public enum CarState
    {
        idle, controlledByPlayer, controlledByAi, destroied
    }
    public CarState carState = CarState.controlledByAi;

    public float maxSpeed;
    public float curSpeed;
    public float rotSpeed;

    Vector3[] oldForwards = new Vector3[20];
    Vector3 reboundForce = Vector3.zero;

    float inputH;
    float inputV;
    float joystickInputH, joystickInputV;

    //===============
    public GameObject mainDoorPosition;
    public People driver;
    public bool isDoorOpen { get; set; }
    float carDoorCloseTimer = 0.0f;
    float carDoorCloseTime = 2.0f;

    void Awake()
    {
        rbody = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        carState = CarState.controlledByAi;
        carManager.OnDestroy += OnCarDestroy;
    }

    void OnDisable()
    {
        carManager.OnDestroy -= OnCarDestroy;
    }

    void Update()
    {
        carManager.carEffects.DrawSkidMark(inputH, curSpeed);
        carManager.carEffects.AdjustEngineSound(curSpeed, carState);
        SelfDoorCheck();
    }

    void FixedUpdate()
    {
        if (carState == CarState.destroied)
            return;

        if (carState == CarState.controlledByAi)
            return;

        if (carState == CarState.idle)
        {
            inputH = 0;
            inputV = 0;
            return;
        }

        UpdateOldForwards();
        PlayerInput();
        MoveCarByInput();
    }

    void UpdateOldForwards()
    {
        for (int i = 0; i < oldForwards.Length - 1; i++)
        {
            oldForwards[i] = oldForwards[i + 1];
        }
        oldForwards[oldForwards.Length - 1] = transform.forward;
    }

    void PlayerInput()
    {
        if (joystickInputH == 0)
        {
            inputH = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            inputH = joystickInputH;
        }

        if (joystickInputV == 0)
        {
            inputV = Input.GetAxisRaw("Vertical");
        }
        else
        {
            inputV = joystickInputV;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            //FIXME : 타는중 내리는중 구현해서 그 때는 아무런 입력을 받지 않도록 변경
            Invoke("GetOffTheCar", 0.1f);
            //GetOffTheCar();
        }

    }

    public void AiInput(float h, float v)
    {
        inputH = h;
        inputV = v;
    }

    public void MoveCarByInput()
    {
        reboundForce *= 0.85f;

        curSpeed += 100 * inputV * Time.deltaTime;

        if (inputV == 0)
        {
            curSpeed *= 0.97f;
            if (Mathf.Abs(curSpeed) < 1)
                curSpeed = 0;
        }

        curSpeed *= 1 - (Mathf.Abs(inputH) / 70);

        float maxSpdMul = 1.0f;
        if (carState == CarState.controlledByAi)
            maxSpdMul = carManager.carAi.maxSpdMultiplier;

        curSpeed = Mathf.Clamp(curSpeed, (maxSpeed * maxSpdMul * carManager.carDamage.maxSpdMultiplier / 2) * -1, maxSpeed * maxSpdMul * carManager.carDamage.maxSpdMultiplier);

        transform.Rotate(0, inputH * rotSpeed * Time.deltaTime * (Mathf.Abs(curSpeed) / 400), 0);

        Vector3 dir = Vector3.zero;
        if (carState == CarState.controlledByPlayer && inputH != 0)
        {
            for (int i = 0; i < oldForwards.Length; i++)
            {
                dir += oldForwards[i];
            }
            dir.Normalize();
        }
        else
        {
            dir = transform.forward;
        }

        rbody.velocity = dir * curSpeed * Time.deltaTime + reboundForce;
    }

    void OnCarDestroy()
    {
        if (carState != CarState.destroied)
        {
            carState = CarState.destroied;
            if (driver != null)
                driver.Hurt(99999);
        }
    }

    public void GetOnTheCar(People driver)//only player
    {
        if (carState == CarState.destroied)
            return;

        isDoorOpen = false;
        this.driver = driver;
        carState = CarState.controlledByPlayer;
        driver.gameObject.SetActive(false);
        driver.transform.SetParent(transform);
        CameraController.Instance.ChangeTarget(gameObject);
        CameraController.Instance.SetTrackingMode(CameraController.TrackingMode.car);

        carManager.carAi.SetAiMaxSpeedMultiplier();
    }
    public void GetOffTheCar()//only player
    {
        print("내림");
        driver.gameObject.SetActive(true);
        carState = CarState.idle;
        driver.transform.SetParent(null);
        CameraController.Instance.ChangeTarget(driver.gameObject);
        CameraController.Instance.SetTrackingMode(CameraController.TrackingMode.human);
        driver = null;
        carManager.carAi.SetAiMaxSpeedMultiplier();

        curSpeed = 0;
    }
    public void PullOutOfATheCar()//차에 있는 사람 끌어내리기
    {
        if (driver == null)//NPC 끌어내리기
        {
            NPCSpawnManager.Instance.carDriverPool[0].gameObject.SetActive(true);
            NPCSpawnManager.Instance.carDriverPool[0].gameObject.transform.position = mainDoorPosition.transform.position;
            NPCSpawnManager.Instance.carDriverPool[0].Down();
        }
        else //player 임시로 
        {
            driver.transform.position = mainDoorPosition.transform.position;
            driver.transform.SetParent(null);
            driver.Down();
            driver.gameObject.SetActive(true);
        }
    }
    void OnDrawGizmos()
    {
        //Gizmos.color = Color.cyan;
        //Gizmos.DrawWireSphere(destination, 0.25f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Wall")
        {
            curSpeed *= 0.25f;
            Vector3 inDirection = transform.forward;
            reboundForce = Vector3.Reflect(inDirection, col.contacts[0].normal) * curSpeed * 0.15f;
            Debug.DrawLine(transform.position, transform.position - inDirection, Color.blue, 1f);
            Debug.DrawLine(transform.position, transform.position + reboundForce, Color.red, 1f);
        }
        else if (col.transform.tag == "Car")
        {
            curSpeed *= 0.25f;
        }

        if(col.transform.tag == "NPC" || col.transform.tag == "Player")
        {
            col.gameObject.GetComponent<People>().Hurt((int)Mathf.Abs(curSpeed)/2);

            if (col.gameObject.GetComponent<People>().isDown)
            {
                col.gameObject.GetComponent<People>().Hurt(500);
                print("뚜쉬");
            }
                
        }
    }

    void OnCollisionStay(Collision col)
    {
        if (col.transform.tag == "Wall" && Mathf.Abs(curSpeed) > 50)
        {
            curSpeed *= 0.9f;           
        }
    }

    // UI---------------------
    public People GetDriver()
    {
        return driver;
    }

    public void InputVertical(float value)
    {
        joystickInputV = value;
    }

    public void InputHorizon(float value)
    {
        joystickInputH = value;
    }

    public void InputReturn()
    {
        GetOffTheCar();
    }

    //CarDoor 관련 추가
    void SelfDoorCheck()
    {
        if(isDoorOpen)
        {
            if(carDoorCloseTimer > carDoorCloseTime)
            {
                carDoorCloseTimer = 0;
                isDoorOpen = false;
            }
            else
            {
                carDoorCloseTimer += Time.deltaTime;
            }
        }
    }
}
