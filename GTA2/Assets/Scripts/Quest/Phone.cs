using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phone : MonoBehaviour
{
    // Start is called before the first frame update
    public string phoneName;


    bool isRing;
    bool prevRing;
    Animator phoneAnimator;
    void Start()
    {
        phoneAnimator = GetComponentInChildren<Animator>();
        phoneAnimator.SetBool("IsRinging", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isRing && prevRing != isRing)
        {
            phoneAnimator.SetBool("IsRinging", true);
            prevRing = isRing;
        }
        else if(!isRing && prevRing != isRing)
        {
            phoneAnimator.SetBool("IsRinging", false);
            prevRing = isRing;
        }
    }

    void SetRing()
    {
        isRing = true;
    }
    void SetIdle()
    {
        isRing = false;
    }
}
