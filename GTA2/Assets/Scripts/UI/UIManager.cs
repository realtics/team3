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
    [SerializeField]
    private CarController targetCarControll;

    // Start is called before the first frame update
    private Player player;

    // TODO: 조이스틱 플레이어 휴먼 및 자동차 연결
    bool isLeftDown;
    bool isRightDown;
    bool isExcelDown;
    bool isBreakDown;

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

        UpdateGetOffCar();
        UpdateButton();
    }

    // 컴퓨터용 체인저
    private void UpdateGetOffCar()
    {
        if (targetCarControll == null)
        {
            return;
        }

        // 타겟 팅중인 차가 없으면
        if (targetCarControll.GetDriver() == null)
        {
            OutCar();
            targetCarControll = null;
        }
    }

    void UpdateButton()
    {
        if (isExcelDown)
        {
            targetCarControll.InputVertical(1.0f);
        }
        if (isBreakDown)
        {
            targetCarControll.InputVertical(-1.0f);
        }
        if (isLeftDown)
        {
            targetCarControll.InputHorizon(-1.0f);
        }
        if (isRightDown)
        {
            targetCarControll.InputHorizon(1.0f);
        }

        if (!isLeftDown && !isRightDown && targetCarControll != null)
        {
            targetCarControll.InputHorizon(.0f);
        }
        if (!isExcelDown && !isBreakDown && targetCarControll != null)
        {
            targetCarControll.InputVertical(.0f);
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






    #region HumanUI
    public void LSwapButtonDown()
    {
        player.SwapPrev();
    }
    public void RSwapButtonDown()
    {
        player.SwapNext();
    }
    public void JumpButtonDown()
    {
        player.JumpButtonDown();
    }
    public void EnterButtonDown()
    {
        player.SetChaseTargetCar();
    }
    public void ShotButtonDown()
    {
        player.ShotButtonDown();
    }
    public void ShotButtonUp()
    {
        player.ShotButtonUp();
    }



    #endregion

    #region Car UI
    public void InCar(CarController targetCar)
    {
        targetCarControll = targetCar;
        humanJoystick.SetActive(false);
        carJoystick.SetActive(true);
    }
    public void OutCar()
    {
        humanJoystick.SetActive(true);
        carJoystick.SetActive(false);
    }


    public void ExcelButtonDown()
    {
        isExcelDown = true;
    }
    public void BreakButtonDown()
    {
        isBreakDown = true;
    }
    public void RightButtonDown()
    {
        isRightDown = true;
    }
    public void LeftButtonDown()
    {
        isLeftDown = true;
    }


    public void ExcelButtonUp()
    {
        isExcelDown = false;
    }
    public void BreakButtonUp()
    {
        isBreakDown = false;
    }
    public void RightButtonUp()
    {
        isRightDown = false;
    }
    public void LeftButtonUp()
    {
        isLeftDown = false;
    }

    public void ReturnButtonDown()
    {
        targetCarControll.InputReturn();
    }
    #endregion
}
