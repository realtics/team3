using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTimer : MonoBehaviour
{
    // Start is called before the first frame update
    //Timer
    float jumpTime = 1.5f;
    float jumpTimer = 0.0f;
    float jumpMinTime = 0.5f;
    float respawnTime = 3.0f;
    float respawnTimer = 0.0f;
    float carOpenTime = 0.5f;
    float carOpenTimer = 0.0f;
    float bustedCheckTime = 3.0f;
    float bustedCheckTimer = 3.0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool RespawnTimerCheck()
    {
        respawnTimer += Time.deltaTime;

        if (respawnTime < respawnTimer)
        {
            respawnTimer = 0.0f;
            return true;
        }
        return false;
    }

    public bool BustedTimerCheck()
    {
        bustedCheckTimer += Time.deltaTime;
        if (bustedCheckTimer > bustedCheckTime)
        {
            bustedCheckTimer = 0.0f;
            return true;
        }
        return false;
    }
    public bool JumpTimerCheck()
    {
        jumpTimer += Time.deltaTime;

        if (jumpTimer > jumpTime)
        {
            jumpTimer = 0.0f;
            return true;
        }
        return false;
    }
    public bool JumpMinTimeCheck()
    {
        if (jumpTimer > jumpMinTime)
            return true;
        else
            return false;
    }
    public bool CarOpenTimerCheck()
    {
        carOpenTimer += Time.deltaTime;
        
        if (carOpenTimer > carOpenTime)
        {
            carOpenTimer = 0.0f;
            return true;
        }
        return false;
    }
}
