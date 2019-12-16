using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBlockFence : MonoBehaviour
{
	public Rigidbody rbody;
	public Animator anim;

	void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag != "Ground")
		{
			anim.SetTrigger("Fall");
		}
	}
}
