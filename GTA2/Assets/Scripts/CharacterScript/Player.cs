using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : People //TODO : 상체하체 나누지 않고 한 스프라이트로, 애니메이터 육망성 만들지 않고..
{
    public enum PlayerStateUnder
    {
        IDLE,
        WALK,
        JUMP,
        DIE,
        LAND
    }
    private enum PlayerStateUpper
    {
        IDLE,
        PUNCH,
        SHOT
    }

    private float playerMoveSpeed = 2.0f;

    public PlayerStateUnder playerStateUnder = PlayerStateUnder.IDLE;
    private PlayerStateUpper playerStateUpper = PlayerStateUpper.IDLE;
        
    public bool isAttack { get; set; }
    public int money { get; set; }

    private bool isPunch = false;
    private bool isJump = false;
    private float jumpTime = 1.5f;
    private float jumpTimer = 0.0f;
    
    Rigidbody myRigidBody;

    private void Awake()
    {
        myRigidBody = GetComponent<Rigidbody>();
    }

    public bool IsAttack
    {
        get
        {
            return isAttack;
        }
    }
    public bool IsPunch
    {
        get
        {
            return isPunch;
        }
    }

    private RaycastHit hit;
    private GunState curGunIndex = GunState.None;

    public LayerMask collisionLayer;

    public Animator under;
    public Animator upper;
    public List<Gun> gunList;
    

    void Start()
    {
        moveSpeed = playerMoveSpeed;

        gunList[(int)GunState.None].bulletCount = 1;
        gunList[(int)GunState.None].gameObject.SetActive(true);
    }

    private void Update()
    {
        under.SetInteger("PlayerState", (int)playerStateUnder);
        upper.SetInteger("PlayerState", (int)playerStateUpper);
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
        if (playerStateUnder == PlayerStateUnder.DIE)
            return;
        //방향키 조작
        MoveControl();
        ActiveControl();
        WeaponSwap();

        //아무 행동도 하지 않을때 (하체)
        if (isAnyActive())
        {
            playerStateUnder = PlayerStateUnder.IDLE;
        }

        Move();
        //TODO : 오브젝트 조작(근처의 탈 것 등등)
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
        playerStateUnder = PlayerStateUnder.DIE;
    }

    #region lowlevelCode
    protected override void Move()
    {
        //Vector3 Pos = transform.position;

        //Pos.x += hDir * Time.deltaTime * moveSpeed;
        //Pos.z += vDir * Time.deltaTime * moveSpeed;

        //transform.position = Pos;

        myRigidBody.MovePosition(transform.position + (new Vector3(hDir, 0, vDir) * Time.deltaTime * moveSpeed));
    }
    void MoveControl()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            vDir = 1;
            playerStateUnder = PlayerStateUnder.WALK;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            vDir = -1;
            playerStateUnder = PlayerStateUnder.WALK;

        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            hDir = 1;
            playerStateUnder = PlayerStateUnder.WALK;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            hDir = -1;
            playerStateUnder = PlayerStateUnder.WALK;
        }
        if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow))
        {
            vDir = 0;
        }
        if (!Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            hDir = 0;
        }
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
    void ActiveControl()
    {
        if (Input.GetKeyDown(KeyCode.A) && !isJump)
        {
            switch (curGunIndex)
            {
                case GunState.None:
                    playerStateUpper = PlayerStateUpper.PUNCH;
                    isPunch = true;
                    break;
                case GunState.FireBottle:
                case GunState.Granade:
                    isAttack = true;
                    playerStateUpper = PlayerStateUpper.PUNCH;
                    break;
                case GunState.Pistol:
                case GunState.DoublePistol:
                case GunState.Machinegun:
                case GunState.SleepMachinegun:
                case GunState.RocketLauncher:
                case GunState.Electric:
                case GunState.ShotGun:
                case GunState.FireGun:
                    isAttack = true;
                    playerStateUpper = PlayerStateUpper.SHOT;
                    break;
                default:
                    break;
            }
        }
        if (!Input.GetKey(KeyCode.A))
        {
            playerStateUpper = PlayerStateUpper.IDLE;
            isAttack = false;
            isPunch = false;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Jump();
        }
    }
    void Jump()
    {
        if (isJump)
            return;
        isJump = true;
        GetComponent<Rigidbody>().useGravity = false;
        transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);

        playerStateUnder = PlayerStateUnder.JUMP;
    }
    void Land()
    {
        isJump = false;
        transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        GetComponent<Rigidbody>().useGravity = true;
        playerStateUnder = PlayerStateUnder.LAND;
        //playerStateUnder = PLAYERSTATE_UNDER.IDLE;
    }
    void WeaponSwap()
    {
        if (Input.GetKeyDown(KeyCode.X))
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
        if (Input.GetKeyDown(KeyCode.Z))
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
    }
    #endregion
}