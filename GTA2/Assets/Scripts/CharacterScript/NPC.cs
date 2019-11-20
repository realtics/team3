using UnityEngine;
using UnityEditor;

public abstract class NPC : People
{
    //HumanCtr스크립트 참조 코드
    RaycastHit hit;
    float distToObstacle = Mathf.Infinity;
    TrafficLight trafficLight = null;
    protected Vector3 destination;
    public LayerMask collisionLayer;
    public bool isDestReached;

    protected float findRange = 10.0f;
    protected bool isDown;
    protected float downTimer = 0.0f;
    protected float downTime = 3.0f;

    protected float punchRange = 0.2f;
    protected float shotRange = 5.0f;
    protected float outofRange = 20.0f;

    float respawnTimer = 0.0f;
    float respawnTime = 5.0f;
    int money = 10; //사망시 플레이어에게 주는 돈

    public abstract void Down();
    public abstract void Rising();
    public abstract void Respawn();

    public GameObject player;
    protected void NPCInit()
    {
        player = GameObject.FindWithTag("Player");
    }
    protected void NPCUpdate()
    {
        if(isDown)
        {
            downTimer += Time.deltaTime;

            if(downTimer > downTime)
            {
                downTimer = 0;
                isDown = false;

                Rising();
            }
        }
        else if(isDie)
        {
            respawnTimer += Time.deltaTime;

            if (respawnTimer > respawnTime)
            {
                respawnTimer = 0;
                isDie = false;

                Respawn();
            }
        }
    }
    protected bool DectectedPlayerAttack()
    {
        if (player.GetComponent<Player>().isAttack &&
            findRange > Vector3.Distance(transform.position, player.transform.position))
            return true;
        else
            return false;
    }
    protected virtual void RunAway()
    {
        Vector3 Pos = transform.position;

        Pos.x += transform.forward.x * Time.deltaTime * runSpeed;
        Pos.z += transform.forward.z * Time.deltaTime * runSpeed;

        transform.position = Pos;
    }
    protected virtual void ChasePlayer()
    {
        transform.LookAt(player.transform.position);

        Vector3 Pos = transform.position;
        Pos.x += transform.forward.x * Time.deltaTime * runSpeed;
        Pos.z += transform.forward.z * Time.deltaTime * runSpeed;
        transform.position = Pos;
    }
    protected void UpdateTargetDirection()
    {
        transform.Rotate(0, Random.Range(0, 360), 0);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet") || other.CompareTag("PlayerFireBullet"))
        {
            Bullet HitBullet = other.GetComponent<Bullet>();

            //HitBullet.누가쐈는지
            Hurt(HitBullet.bulletDamage);
            other.gameObject.SetActive(false);
        }
        else if(other.CompareTag("PlayerPunch"))
        {
            Down();
            isDown = true;
            
            print("Punched");
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
    protected void Raycast()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, 0.5f, collisionLayer))
        {
            if (hit.transform.tag == "TrafficLight")
            {
                if (Vector3.Dot(transform.forward, hit.transform.forward) < -0.8f)
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
            }
        }
        else
        {
            distToObstacle = Mathf.Infinity;
        }

        DrawRaycastDebugLine();
    }
    protected bool InPunchRange()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < punchRange)
            return true;
        else
            return false;
    }
    protected bool InShotRange()
    {
        if (Vector3.Distance(player.transform.position, transform.position) < shotRange)
            return true;
        else
            return false;
    }
    protected bool PlayerOutofRange()
    {
        if (Vector3.Distance(player.transform.position, transform.position) > outofRange)
            return true;
        else
            return false;
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
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(destination, 0.25f);
        Gizmos.DrawWireSphere(destination, 1);
        //Handles.Label(destination, "destination");
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
    public void IncreaseMoney()
    {
        player.GetComponent<Player>().money += money;
    }
    #endregion
}