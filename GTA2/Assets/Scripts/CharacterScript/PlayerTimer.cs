using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTimer : MonoBehaviour
{
    // Start is called before the first frame update
	float respawnTime = 3.0f;
    float respawnTimer = 0.0f;
    float carOpenTime = 0.5f;
    float carOpenTimer = 0.0f;
    
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
