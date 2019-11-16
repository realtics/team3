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

    /// <summary>
    /// 플레이어에서 직접 참조하기 위해 public으로 품.
    /// </summary>
    /// //[SerializeField]
    //private bl_Joystick playerJoystick;
    public bl_Joystick playerJoystick;
    [SerializeField]
    private GameObject humanJoystick;
    [SerializeField]
    private GameObject carJoystick;

    // Start is called before the first frame update
    private Player player;

    // TODO: 조이스틱 플레이어 휴먼 및 자동차 연결
    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();

        // TODO: 조이스틱 차냐 사람이냐에 따라 방식 설정
        humanJoystick.SetActive(true);
        carJoystick.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        moneyTextUI.SetMoney(player.money);
        weaponUI.SetGunSprite(player.curGunIndex, player.gunList[(int)player.curGunIndex].bulletCount);
        heartListUI.SetHealthPoint(player.GetHp());

        InputChanger();
    }

    // 컴퓨터용 체인저
    private void InputChanger()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            humanJoystick.SetActive(!humanJoystick.activeInHierarchy);
            carJoystick.SetActive(!carJoystick.activeInHierarchy);
        }
    }

    public bool isHumanUI()
    {
        return humanJoystick.activeInHierarchy;
    }
    public bool isCarUI()
    {
        return carJoystick.activeInHierarchy;
    }
}
