using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField]
    HeartImageList heartListUI;
    [SerializeField]
    MoneyText moneyTextUI;
    [SerializeField]
    PoliceImageList policeListUI;
    [SerializeField]
    WeaponImage weaponUI;
    [SerializeField]
    GameEndUI gameEndUI;
    [SerializeField]
    LifeCount lifeCountUI;
    [SerializeField]
    GetItemText weaponTextUI;

    /// <summary>
    /// 플레이어에서 직접 참조하기 위해 public으로 품.
    /// </summary>
    /// //[SerializeField]
    //private bl_Joystick playerJoystick;
    public bl_Joystick playerJoystick;
    
    [SerializeField]
    GameObject humanJoystick;
    [SerializeField]
    GameObject carJoystick;
    [SerializeField]
    CarController targetCarControll;

    // Start is called before the first frame update
    private Player player;

    // TODO: 조이스틱 플레이어 휴먼 및 자동차 연결
    bool isLeftDown;
    bool isRightDown;
    bool isExcelDown;
    bool isBreakDown;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();

        // TODO: 조이스틱 차냐 사람이냐에 따라 방식 설정
        humanJoystick.GetComponentInChildren<bl_Joystick>().SetCanvas(GetComponent<Canvas>());
        heartListUI.SetMaxPlayerHp(player.GetHp());
    }

    // Update is called once per frame
    void Update()
    {
        moneyTextUI.SetMoney(GameManager.Instance.money);
        weaponUI.SetGunSprite(player.curGunIndex, player.gunList[(int)player.curGunIndex].bulletCount);
        
        heartListUI.SetHealthPoint(player.GetHp());
        lifeCountUI.UpdateLifeCount(GameManager.Instance.remains);

        UpdateDieUI();
        UpdateGetOffCar();
        UpdateButton();
    }

    // 컴퓨터용 체인저
    void UpdateGetOffCar()
    {
        if (targetCarControll == null)
        {
            return;
        }

        if (player.isDie)
        {
            return;
        }

        // 타겟 팅중인 차가 없으면
        if (targetCarControll.GetDriver() == null)
        {
            HumanUIMode();
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
    void UpdateDieUI()
    {
        if (player.isDie)
        {
            humanJoystick.SetActive(false);
            carJoystick.SetActive(false);
        }
    }


    public bool IsHumanUI()
    {
        return humanJoystick.activeInHierarchy;
    }
    public bool IsCarUI()
    {
        return carJoystick.activeInHierarchy;
    }



    public void TurnOnWastedSprite()
    {
        gameEndUI.TurnOnWastedSprite();
    }
    public void TurnOnBustedSprite()
    {
        gameEndUI.TurnOnBustedSprite();
    }
    public void TurnOnGameOverSprite()
    {
        gameEndUI.TurnOnGameOverSprite();
    }
    public void TurnOffEndUI()
    {
        gameEndUI.TurnOffEndUI();
    }

    public void TurnOnItemText(ItemStatus itemState)
    {
        weaponTextUI.SetText(itemState);
    }


    public void CarUIMode(CarController targetCar)
    {
        targetCarControll = targetCar;
        humanJoystick.SetActive(false);
        carJoystick.SetActive(true);
    }
    public void HumanUIMode()
    {
        if (player != null)
        {
            player.ShotButtonUp();
        }
        humanJoystick.SetActive(true);
        carJoystick.SetActive(false);
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
