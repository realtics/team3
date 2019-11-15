using UnityEngine;
using UnityEditor;

public abstract class NPC : People
{
    protected float findRange = 10.0f;
    
    //TODO : isDown없이 작동하게 수정
    protected bool isDown = false; 
    protected float downTimer = 0.0f;
    protected float downTime = 3.0f;

    float respawnTimer = 0.0f;
    float respawnTime = 5.0f;
    
    //TODO : abstract로 변경 예정
    public delegate void DownEventHandler();
    public event DownEventHandler Down;

    public delegate void RiseEventHandler();
    public event RiseEventHandler Rising;

    public delegate void RespawnEventHandler();
    public event RespawnEventHandler Respawn;

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

                if(Rising != null)
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

                if (Respawn != null)
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
        /*transform.LookAt(targetDirectionVector);
        Vector3 Pos = transform.position;

        Pos.x += transform.forward.x * Time.deltaTime * runSpeed;
        Pos.z += transform.forward.z * Time.deltaTime * runSpeed;

        transform.position = Pos;*/
    }
    protected void UpdateTargetDirection()
    {
        transform.Rotate(0, Random.Range(0, 360), 0);
    }
    
    private void OnTriggerEnter(Collider other)
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
            if(Down != null)
            {
                Down();
            }
            isDown = true;
            
            print("Punched");
        }
    }
}

