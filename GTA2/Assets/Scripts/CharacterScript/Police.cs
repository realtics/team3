using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Police : NPC
{
    public bool isWalk { get; set; }
    public bool isShot { get; set; }
    public bool isPunch { get; set; }
    public bool isJump { get; set; }
    public bool isStealing { get; set; }
    public bool isChasePlayer { get; set; }

    float patternChangeTimer = 3.0f;
    float patternChangeInterval = 3.0f;
    float attackInterval = 1.0f;
    float punchRange = 2.0f;
    float curAttackCoolTime = 0.0f;

    //HumanCtr스크립트 참조 코드
    private Vector3 destination;
    private RaycastHit hit;
    private float distToObstacle = Mathf.Infinity;
    private TrafficLight trafficLight = null;
    public bool isDestReached = true;

    GameObject targetCar;
    float playerMoveSpeed = 2.0f;

    public GunState curGunIndex { get; set; }

    public bool isAttack { get; set; }
    public int money { get; set; }

    float jumpTime = 1.5f;
    float jumpTimer = 0.0f;

    Rigidbody myRigidBody;

    public LayerMask collisionLayer;
    public Animator animator;
    public Gun gunList;

    private void Awake()
    {
        myRigidBody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        base.NPCInit();

        patternChangeInterval = Random.Range(3.0f, 500.0f);
        patternChangeTimer = patternChangeInterval;
    }

    // Start is called before the first frame update
    private void Update()
    {
        animator.SetBool("isWalk", isWalk);
        animator.SetBool("isShot", isShot);
        animator.SetBool("isPunch", isPunch);
        animator.SetBool("isJump", isJump);
        animator.SetBool("isDie", isDie);

        base.NPCUpdate();

        TimerCheck();
        //ActivityByState();
    }
    private void ActivityByState()
    {
        if(isChasePlayer)
        {
            //ChasePlayer();
        }
        else
        {
            Raycast();
            Move();
        }
    }

    #region RefHumanCtr
    protected override void Move()
    {
        if (isDestReached)
        {
            return;
        }

        Vector3 dir = new Vector3(destination.x, transform.position.y, destination.z) - transform.position;

        transform.rotation = Quaternion.LookRotation(dir);//Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 0.4f);

        if (distToObstacle != Mathf.Infinity)
            return;

        transform.Translate(transform.forward * moveSpeed * Time.deltaTime, Space.World);
    }

    void Raycast()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, 0.5f, collisionLayer))
        {
            if (hit.transform.tag == "TrafficLight")
            {
                if (trafficLight == null)
                    trafficLight = hit.transform.GetComponent<TrafficLight>();

                if (trafficLight.signalColor == TrafficLight.SignalColor.SC_Green)
                {
                    distToObstacle = hit.distance;
                }
                else
                {
                    distToObstacle = Mathf.Infinity;
                }
            }
            else
            {
                distToObstacle = hit.distance;
                trafficLight = null;
            }
        }
        else
        {
            distToObstacle = Mathf.Infinity;
        }

        DrawRaycastDebugLine();
    }

    void DrawRaycastDebugLine()
    {
        if (distToObstacle < Mathf.Infinity)
        {
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * 0.5f, Color.blue);
        }
    }
    public void SetDestination(Vector3 pos)
    {
        destination = pos;
        isDestReached = false;
    }

    public void Stop()
    {
        destination = transform.position;
        isDestReached = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(destination, 0.25f);
        Gizmos.DrawWireSphere(destination, 1);
        //Handles.Label(destination, "destination");
    }
    #endregion
    protected override void Die() //리스폰 필요
    {
        if (!isDie)
        {
            player.GetComponent<Player>().money += 10;
        }

        isDie = true;
        GetComponent<Rigidbody>().isKinematic = false;
    }

    //TODO : IDLE Timer, Walk Timer 따로만들기
    private void TimerCheck()
    {
        if (isDie)
            return;
        patternChangeTimer += Time.deltaTime;

        if (isChasePlayer)
        {
            //TODO : player와 일정 거리 이상 멀어지면 그만 쫓기
        }
        else
        {
            patternChangeInterval = Random.Range(3.0f, 500.0f);
            if (isWalk)
            {
                isWalk = false;
            }
            else
            {
                isWalk = true;
            }
        }
    }
   
    #region override method
    public override void Down()
    {
        isDown = true;
    }

    public override void Rising()
    {
        //경찰은 맞아도 그대로 할 거함.
        isDown = false;
    }

    public override void Respawn()
    {
        patternChangeTimer = 0;
        isDie = false;
        isWalk = false;
        hp = 100;
        GameObject.FindWithTag("SpawnManager").GetComponent<SpawnManager>().NPCRepositioning(this);
        print("Citizen Respawn");
    }
    #endregion
    
}
