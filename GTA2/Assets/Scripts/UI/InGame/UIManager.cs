using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UIManager : MonoSingleton<UIManager>
{
    [Header("UI Component")]
    [SerializeField]
    HeartImageList heartListUI;
    [SerializeField]
    MoneyText moneyTextUI;
    [SerializeField]
    WeaponImage weaponUI;
    [SerializeField]
    LifeCount lifeCountUI;
    [SerializeField]
    GetItemText weaponTextUI;
    [SerializeField]
    PoliceLevel policeLevelUI;
    [SerializeField]
    Image carNumberUI;
    [SerializeField]
    GameObject pauseUI;


    [Header("UI Sound")]
    [SerializeField]
    AudioClip wastedClip;
    [SerializeField]
    AudioClip bustedClip;
    [SerializeField]
    AudioClip gameOverClip;


    /// <summary>
    /// 플레이어에서 직접 참조하기 위해 public으로 품.
    /// </summary>
    [Header("UI JoyStick")]
    public bl_Joystick playerMoveJoystick;
    public bl_Joystick playerWeaponJoystick;

    [SerializeField]
    GameObject humanJoystick;
    [SerializeField]
    GameObject carJoystick;
    [SerializeField]
    CarManager targetCar;

    // Start is called before the first frame update
    Player player;

    // TODO: 조이스틱 플레이어 휴먼 및 자동차 연결
    bool isLeftDown;
    bool isRightDown;
    bool isExcelDown;
    bool isBreakDown;

    Text carNumberText;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();

        // TODO: 조이스틱 차냐 사람이냐에 따라 방식 설정
        playerMoveJoystick.SetCanvas(GetComponent<Canvas>());
        playerWeaponJoystick.SetCanvas(GetComponent<Canvas>());

        player.SetHpDefault();
        heartListUI.SetMaxPlayerHp(player.playerData.maxHp);
        carNumberUI.gameObject.SetActive(false);
        carNumberText = carNumberUI.GetComponentInChildren<Text>();

        pauseUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        moneyTextUI.SetMoney(GameManager.Instance.money);
        heartListUI.SetHealthPoint(player.GetHp());
        lifeCountUI.UpdateLifeCount(GameManager.Instance.remains);

        UpdateDieUI();
        UpdateGetOffCar();
        UpdateButton();
        UpdatePause();


        if ((int)player.curGunIndex < 0 || (int)player.curGunIndex > (int)GunState.Granade)
        {
            return;
        }
        weaponUI.SetSpriteSizePos(player.curGunIndex, player.gunList[(int)player.curGunIndex].bulletCount);
    }

    // 컴퓨터용 체인저
    void UpdateGetOffCar()
    {
        if (targetCar == null)
        {
            return;
        }

        if (player.isDie)
        {
            return;
        }

        // 차에 탄 사람이 타겟팅중인 차가 없으면
        if (targetCar.passengerManager.doors[0].passenger != People.PeopleType.Player)
        {
            HumanUIMode();
            targetCar = null;
        }
    }

    void UpdateButton()
    {
        if (isExcelDown)
        {
            targetCar.input.InputVertical(1.0f);
        }
        if (isBreakDown)
        {
            targetCar.input.InputVertical(-1.0f);
        }
        if (isLeftDown)
        {
            targetCar.input.InputHorizon(-1.0f);
        }
        if (isRightDown)
        {
            targetCar.input.InputHorizon(1.0f);
        }

        if (!isLeftDown && !isRightDown && targetCar != null)
        {
            targetCar.input.InputHorizon(.0f);
        }
        if (!isExcelDown && !isBreakDown && targetCar != null)
        {
            targetCar.input.InputVertical(.0f);
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
    void UpdatePause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseUI.activeInHierarchy)
            {
                ClosePauseWindow();
            }
            else if (!pauseUI.activeInHierarchy)
            {
                Time.timeScale = .0f;
                pauseUI.SetActive(true);
            }
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



    public void TurnOnWasted()
    {
        SoundManager.Instance.PlayClip(wastedClip, SoundPlayMode.UISFX);
        QuestUIManager.Instance.ToastTitle("WASTED");
    }
    public void TurnOnBusted()
    {
        SoundManager.Instance.PlayClip(bustedClip, SoundPlayMode.UISFX);
        QuestUIManager.Instance.ToastTitle("BUSTED");
    }
    public void TurnOnGameOver()
    {
        SoundManager.Instance.PlayClip(gameOverClip, SoundPlayMode.UISFX);
        QuestUIManager.Instance.ToastTitle("GAME OVER");
    }

    public void TurnOnItemText(ItemStatus itemState)
    {
        weaponTextUI.SetText(itemState);
    }


    public void CarUIMode(CarManager targetCar)
    {
        AllCarButtonUp();
        this.targetCar = targetCar;
        humanJoystick.SetActive(false);
        carJoystick.SetActive(true);
        carNumberUI.gameObject.SetActive(true);
        carNumberText.text = targetCar.gameObject.name.ToUpper();
    }
    public void HumanUIMode()
    {
        if (player != null)
        {
            player.ShotButtonUp();
        }
        AllCarButtonUp();
        carNumberUI.gameObject.SetActive(false);
        humanJoystick.SetActive(true);
        carJoystick.SetActive(false);
    }

    public void SetPoliceLevel(int value)
    {
        policeLevelUI.SetPoliceLevel(value);
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
        player.Jump();
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

    void AllCarButtonUp()
    {
        isExcelDown = false;
        isBreakDown = false;
        isRightDown = false;
        isLeftDown = false;
    }

    public void ReturnButtonDown()
    {
        targetCar.input.InputReturn();
    }
    #endregion


    public void ClosePauseWindow()
    {
        Time.timeScale = 1.0f;
        pauseUI.SetActive(false);
    }

    public void ExitGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainMenu");
    }
}
