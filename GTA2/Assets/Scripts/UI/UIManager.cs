using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private HeartImageList heartListUI;
    [SerializeField]
    private MoneyText moneyTextUI;
    [SerializeField]
    private PoliceImageList policeListUI;
    [SerializeField]
    private WeaponImage weaponUI;
    [SerializeField]
    private HumanUI humanJoystickUI;
    [SerializeField]
    private CarUI carJoystickUI;



    // Start is called before the first frame update
    private Player userPlayer;



    private void Start()
    {
        userPlayer = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame

}
