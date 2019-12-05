using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class Citizen : NPC
{
    public Animator animator;
    public bool isRunaway { get; set; }
        [SerializeField]
    float patternChangeTimer;
    [SerializeField]
    float patternChangeInterval;
    float runawayTime = 5.0f;

    Rigidbody rigidbody;
    float minIdleTime = 0.3f;
    float maxIdleTime = 1.0f;
    float minWalkTime = 10.0f;
    float maxWalkTime = 15.0f;

    Vector3 RunawayVector;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
		gameManager = GameManager.Instance;
	}

    void Start()
    {
        patternChangeInterval = Random.Range(minIdleTime, maxIdleTime);
        patternChangeTimer = patternChangeInterval;
        StartCoroutine(ActivityByState());
        
        if(isDriver)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<BoxCollider>().enabled = false;
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
    }
    void Update()
    {
        animator.SetBool("isWalk", isWalk);
        animator.SetBool("isPunch", isPunch);
        animator.SetBool("isDie", isDie);
        animator.SetBool("isDown", isDown);

        base.PeopleUpdate();
        if (isDie || isDown)
            return;

        TimerCheck();
    }
    private void FixedUpdate()
    {
        if (isDie || isDown)
            return;
        if (isRunaway)
        {
            base.RunAway();
        }
        else if (isWalk)
        {
            base.Move();
        }
    }
    IEnumerator ActivityByState()
    {
        while(true)
        {
            if (isDie || isDown)
                yield break;
            
            else if (isWalk)
            {
                base.Raycast();
            }

            yield return new WaitForSeconds(0.3f);
        }
    }
    void TimerCheck()
    {
        patternChangeTimer += Time.deltaTime;

        if (DetectedPlayerAttack())
        {
            SetRunaway();
        }
        else
            PatternChange();
    }
    void SetRunaway()
    {
        patternChangeTimer = 0.0f;
        patternChangeInterval = runawayTime;
        Vector3 playerPosition = GameManager.Instance.player.transform.position;
        RunawayVector = new Vector3(playerPosition.x, transform.position.y, playerPosition.z);
        transform.LookAt(RunawayVector);
        transform.Rotate(0, 180, 0);
        isRunaway = true;
        isWalk = true;
    }
    void PatternChange()
    {
        if (patternChangeTimer > patternChangeInterval)
        {
            patternChangeTimer = 0.0f;
            if (isRunaway)
            {
                patternChangeInterval = Random.Range(minIdleTime, maxIdleTime);
                isRunaway = false;
                //isWalk = false;
            }
            else if (isWalk)
            {
                patternChangeInterval = Random.Range(minIdleTime, maxIdleTime);
                isWalk = false;
            }
            else
            {
                patternChangeInterval = Random.Range(minWalkTime, maxWalkTime);
                isWalk = true;
            }
        }
    }
    #region override_method
    public override void Down()
    {
        isDown = true;
        SetRunaway();
    }

    public override void Rising()
    {
        transform.LookAt(new Vector3(GameManager.Instance.player.transform.position.x, transform.position.y, GameManager.Instance.player.transform.position.z));
        transform.Rotate(0, 180, 0);
        isRunaway = true;
        patternChangeTimer = 0.0f;
    }
    protected override void Die()
    {
        isDie = true;
        GameManager.Instance.IncreaseMoney(money);
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<BoxCollider>().enabled = false;
    }
    public override void Respawn()
    {
        patternChangeTimer = 0;
        isDie = false;
        hp = 100;
        NPCSpawnManager.Instance.NPCRepositioning(this);
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<BoxCollider>().enabled = true;
        print("Citizen Respawn");
    }
    #endregion
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall" && collision.gameObject.tag == "Car" && isRunaway)
        {
            transform.Rotate(0, Random.Range(90, 270), 0);
        }
    }
}