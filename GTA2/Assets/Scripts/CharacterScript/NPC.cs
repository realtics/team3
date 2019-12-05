using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class NPC : People
{
    //HumanCtr스크립트 참조 코드
    float distToObstacle = Mathf.Infinity;
    protected Vector3 destination;
    public bool isDestReached;

    protected float findRange = 10.0f;

    //sqrMagnitude 사용해서 제곱함.
    protected float punchRange = 0.08f;
    protected float shotRange = 10.0f;
    protected float chaseRange = 7.0f;
    protected float outofRange = 400.0f;
	protected GameManager gameManager;

	protected int money = 10; //사망시 플레이어에게 주는 돈
	
	void OnEnable()
    {
		//StartCoroutine(DisableIfOutOfCamera());
	}

    void OnDisable()
    {
        StopAllCoroutines();
    }
        
    protected bool DetectedPlayerAttack()
    {
        if (GameManager.Instance.player.isAttack &&
            findRange > Vector3.Distance(transform.position, GameManager.Instance.player.transform.position))
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
		Vector3 lookAtVector = new Vector3(GameManager.Instance.player.transform.position.x, transform.position.y, GameManager.Instance.player.transform.position.z);

        transform.LookAt(lookAtVector);
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
            HitBullet.Explosion();

            if (isDie == true)
            {
                GameManager.Instance.killCount++;
            }
        }
        else if(other.CompareTag("PlayerPunch"))
        {
            Down();
            
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
        //DrawRaycastDebugLine();
    }
    protected bool InPunchRange()
    {
        if (Vector3.SqrMagnitude(GameManager.Instance.player.transform.position - transform.position) < punchRange)
            return true;
        else
            return false;
    }
    protected bool InShotRange()
    {
        if (Vector3.SqrMagnitude(GameManager.Instance.player.transform.position - transform.position) < shotRange)
            return true;
        else
            return false;
    }
    protected bool InChaseRange() //차에 타고있을때 플레이어 내리기 시도하는 거리
    {
        if (Vector3.SqrMagnitude(GameManager.Instance.player.transform.position - transform.position) < chaseRange)
            return true;
        else
            return false;
    }
    protected bool PlayerOutofRange()
    {
        if (Vector3.SqrMagnitude(GameManager.Instance.player.transform.position - transform.position) > outofRange)
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

    #endregion

    IEnumerator DisableIfOutOfCamera()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);

            Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
            float offset = 3f;
            if (pos.x < 0 - offset ||
                pos.x > 1 + offset ||
                pos.y < 0 - offset ||
                pos.y > 1 + offset)
                gameObject.SetActive(false);            
        }
    }
}