using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class Citizen : NPC
{
    void Awake()
    {
		gameManager = GameManager.Instance;
	}

    void Start()
    {
		base.NPCInit();
        StartCoroutine(ActivityByState());

        if(isDriver)
        {
			SettingToGetOnTheCar();
        }
    }
    void Update()
    {
        base.PeopleUpdate();
		base.NPCUpdate();
		if (isDie || isDown)
            return;

        TimerCheck();
    }
	
	void FixedUpdate()
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
	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Wall" && collision.gameObject.tag == "Car" && isRunaway)
		{
			transform.Rotate(0, Random.Range(90, 270), 0);
		}
	}
	#region lowlevelCode
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
            base.SetRunaway();
        }
        else
            PatternChange();
    }
    
   
	#endregion
	#region override_method
	public override void Down()
    {
		boxCollider.isTrigger = true;
		rigidbody.isKinematic = true;
		isDown = true;
        base.SetRunaway();
    }

    public override void Rising()
    {
        transform.LookAt(new Vector3(GameManager.Instance.player.transform.position.x, transform.position.y, GameManager.Instance.player.transform.position.z));
        transform.Rotate(0, 180, 0);
        isRunaway = true;
        patternChangeTimer = 0.0f;
    }
   
    public override void Respawn()
    {
        patternChangeTimer = 0;
        isDie = false;
        hp = 100;
        NPCSpawnManager.Instance.NPCRepositioning(this);
		rigidbody.isKinematic = false;
        boxCollider.enabled = true;
        print("Citizen Respawn");
    }
    #endregion
}