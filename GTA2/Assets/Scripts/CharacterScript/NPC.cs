using UnityEngine;
using UnityEditor;

public abstract class NPC : People
{
    protected float findRange = 10.0f;
    protected bool isDown = false; //펀치 피격 후 누워있는것
    protected float DownTimer = 0.0f;
    protected float DownTime = 3.0f;

    float respawnTimer = 0.0f;
    float respawnTime = 5.0f;
    //맞고 누울때 호출
    /*나중에 abstract로 변경 예정*/
    public delegate void DownEventHandler();
    public event DownEventHandler Down;

    //맞고 누운다음 일어날때 사용할 델리게이트
    //자식 클래스에서 Rising에 붙을 메소드 연결
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
            DownTimer += Time.deltaTime;

            if(DownTimer > DownTime)
            {
                DownTimer = 0;
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
        if (player.GetComponent<Player>().IsAttack &&
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

