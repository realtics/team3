using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    // Start is called before the first frame update
    public int money;
    public int hp;
    public GunState userWeapon;
    public int policeLevel;

    private Player userPlayer;


    void Start()
    {
        userPlayer = GetComponent<Player>();
    }
}
