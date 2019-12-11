using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phone : MonoBehaviour
{
    // Start is called before the first frame update
    bool isStart;
    Quest motherQuest;
    Animator phoneAnimator;
    Player userPlayer;

    void Start()
    {
        motherQuest = GetComponentInParent<Quest>();
        phoneAnimator = GetComponentInChildren<Animator>();
        userPlayer = GameObject.FindWithTag("Player").GetComponent<Player>();
        SetRing();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStart)
        {
            phoneAnimator.SetBool("IsRinging", true);
        }
        else if (isStart)
        {
            phoneAnimator.SetBool("IsRinging", false);
        }
    }

    void SetRing()
    {
        isStart = false;
    }
    void SetIdle()
    {
        isStart = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != userPlayer.gameObject)
        {
            return;
        }

        if (isStart)
        {
            return;
        }

        SetIdle();
        motherQuest.StartQuest();
    }
}
