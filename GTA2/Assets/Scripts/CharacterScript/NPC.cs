using UnityEngine;
using UnityEditor;

public abstract class NPC : People
{
    protected float findRange = 10.0f;

    protected bool isDown = false;
    protected float downTimer = 0.0f;
    protected float downTime = 3.0f;

    float respawnTimer = 0.0f;
    float respawnTime = 5.0f;

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
            Down();
            isDown = true;
            
            print("Punched");
        }
    }
}

