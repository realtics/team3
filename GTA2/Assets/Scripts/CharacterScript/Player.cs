using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : People //TODO : 상체하체 나누지 않고 한 스프라이트로, 애니메이터 육망성 만들지 않고..
{
    //Animator
    public bool isWalk { get; set; }
    public bool isShot { get; set; }
    public bool isPunch { get; set; }
    public bool isJump { get; set; }

    float playerMoveSpeed = 2.0f;
    
    public GunState curGunIndex { get; set; }
    

    public bool isAttack { get; set; }
    public int money { get; set; }
    
    float jumpTime = 1.5f;
    float jumpTimer = 0.0f;
    
    Rigidbody myRigidBody;
    RaycastHit hit;

    public LayerMask collisionLayer;
    public Animator animator;
    
    public List<Gun> gunList;

    // UI메니저 추가 - 조이스틱 상황에 맞게 키보드 동작을 위함
    [SerializeField]
    UIManager uiManager;
    void Awake()
    {
        myRigidBody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        moveSpeed = playerMoveSpeed;
        gunList[(int)GunState.None].bulletCount = 1;
        gunList[(int)GunState.None].gameObject.SetActive(true);
    }

    private void Update()
    {
        animator.SetBool("isWalk", isWalk);
        animator.SetBool("isShot", isShot);
        animator.SetBool("isPunch", isPunch);
        animator.SetBool("isJump", isJump);
        animator.SetBool("isDie", isDie);
    }
    private void FixedUpdate()
    {
        UpdateInput();
        UpdateTargetRotation();
        UpdateSlerpedRotation();
        TimerCheck();
    }

    void UpdateInput()
    {
        if (isDie)
            return;
        
        //방향키 조작
        MoveControlKeyboard();
        MoveControlJoystick();
        ActiveControl();
        WeaponSwap();

        Move();
        //TODO : 오브젝트 조작(근처의 탈 것 등등)
    }
    public void MoveControlJoystick()
    {
        if (Mathf.Abs(uiManager.playerJoystick.Horizontal) < 0.01f && Mathf.Abs(uiManager.playerJoystick.Vertical) < 0.01f)
            return;
        
        hDir = uiManager.playerJoystick.Horizontal / 5.0f;
        vDir = uiManager.playerJoystick.Vertical / 5.0f;
    }
    void TimerCheck()
    {
        if (isJump)
            LandCheck();
    }
    void LandCheck()
    {
        Debug.DrawRay(transform.position, transform.up * -1, Color.red);

        jumpTimer += Time.deltaTime;

        if (jumpTimer > jumpTime)
        {
            // 밑에 차가없으면 Land
            if (!(Physics.Raycast(transform.position, transform.up * -1, out hit, 1f, collisionLayer)
                && hit.transform.CompareTag("Car")))
            {
                jumpTimer = 0.0f;
                Land();
            }
        }
    }
    protected override void Die()
    {
        isDie = true;
    }

    #region lowlevelCode
    protected override void Move()
    {
        myRigidBody.MovePosition(transform.position + (new Vector3(hDir, 0, vDir) * Time.deltaTime * moveSpeed));
    }
    public int GetHp()
    {
        return hp;
    }
    void MoveControlKeyboard()
    {
        if(Input.GetAxisRaw("Vertical") == 0 && Input.GetAxisRaw("Horizontal") == 0)
        {
            isWalk = false;
            vDir = 0;
            hDir = 0;
        }
        else
        {
            vDir = Input.GetAxisRaw("Vertical"); //GetAxis
            hDir = Input.GetAxisRaw("Horizontal");
            isWalk = true;
        }
        print(vDir + " " + vDir);
    }
    bool isAnyActive()
    {
        if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) && 
            !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow) &&
            !isJump)
            return true;
        else
            return false;
    }

    /// <summary>
    ///  UI 상호작용을 위한 수정 - 유아이 끄면 컴터로도 됨
    /// </summary>
    void ActiveControl()
    {
        if (Input.GetKeyDown(KeyCode.A) && !isJump)
        {
            ShotButtonDown();
        }
        if (!Input.GetKey(KeyCode.A))
        {
            ShotStop();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            JumpButtonDown();
        }
    }
    void Jump()
    {
        if (isJump)
            return;
        isJump = true;
        GetComponent<Rigidbody>().useGravity = false;
        transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
    }
    void Land()
    {
        isJump = false;
        transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        GetComponent<Rigidbody>().useGravity = true;
    }
    void WeaponSwap()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            SwapNext();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            SwapPrev();
        }
    }
    public void SwapNext()
    {
        if ((int)curGunIndex <= 0)
        {
            gunList[(int)curGunIndex].gameObject.SetActive(false);
            curGunIndex = GunState.Granade;
        }
        else
        {
            gunList[(int)curGunIndex].gameObject.SetActive(false);
            curGunIndex--;
        }
        while (gunList[(int)curGunIndex].bulletCount == 0)
        {
            curGunIndex--;
            if ((int)curGunIndex == gunList.Count)
            {
                curGunIndex = GunState.None;
            }
        }
        gunList[(int)curGunIndex].gameObject.SetActive(true);
        Debug.Log(curGunIndex);
    }
    public void SwapPrev()
    {
        if ((int)curGunIndex >= gunList.Count - 1)
        {
            gunList[(int)curGunIndex].gameObject.SetActive(false);
            curGunIndex = GunState.None;
        }
        else
        {
            gunList[(int)curGunIndex].gameObject.SetActive(false);
            curGunIndex++;
        }

        while (gunList[(int)curGunIndex].bulletCount == 0)
        {
            curGunIndex++;
            if ((int)curGunIndex == gunList.Count)
            {
                curGunIndex = GunState.None;
            }
        }
        gunList[(int)curGunIndex].gameObject.SetActive(true);
        Debug.Log(curGunIndex);
    }
    //TODO : UIManager에 있는것이 좋을 것 같음.
    public void JumpButtonDown()
    {
        Jump();
    }

    public void ShotButtonDown()
    {
        if (isJump)
        {
            return;
        }

        gunList[(int)curGunIndex].UpdateBottonDown();
        switch (curGunIndex)
        {
            case GunState.None:
                isPunch = true;
                break;
            case GunState.FireBottle:
            case GunState.Granade:
                isPunch = true;
                isAttack = true;
                break;
            case GunState.Pistol:
            case GunState.DoublePistol:
            case GunState.Machinegun:
            case GunState.SleepMachinegun:
            case GunState.RocketLauncher:
            case GunState.Electric:
            case GunState.ShotGun:
            case GunState.FireGun:
                isShot = true;
                isAttack = true;
                break;
            default:
                break;
        }
    }

    public void ShotButtonUp()
    {
        gunList[(int)curGunIndex].UpdateBottonUp();
        ShotStop();
    }

    void ShotStop()
    {
        isShot = false;
        isAttack = false;
        isPunch = false;
    }
    #endregion
}