using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : People
{
    public enum PLAYERSTATE_UNDER
    {
        IDLE,
        WALK,
        JUMP,
        DIE,
        LAND
    }
    private enum PLAYERSTATE_UPPER
    {
        IDLE,
        PUNCH,
        SHOT
    }

    private float playerMoveSpeed = 2.0f;
    //private float shotDelay = 0.0f;

    // Start is called before the first frame update
    [SerializeField]
    public PLAYERSTATE_UNDER playerStateUnder = PLAYERSTATE_UNDER.IDLE;
    [SerializeField]
    private PLAYERSTATE_UPPER playerStateUpper = PLAYERSTATE_UPPER.IDLE;
    public bool isAttack = false;
    private bool isPunch = false;
    private bool isJump = false;
    private float jumpTime = 1.5f;
    private float jumpTimer = 0.0f;
    private int money = 0;
    Rigidbody myRigidBody;
    private void Awake()
    {
        myRigidBody = GetComponent<Rigidbody>();
    }



    public int Money
    {
        get 
        {
            return money;
        }
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
    public LayerMask collisionLayer;
    RaycastHit hit;

    public Animator under;
    public Animator upper;
    public List<Gun> gunList;
    GUNSTATE curGunIndex = GUNSTATE.NONE;

    void Start()
    {
        moveSpeed = playerMoveSpeed;

     
        gunList[0].gameObject.SetActive(true);
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
        if (playerStateUnder == PLAYERSTATE_UNDER.DIE)
            return;
        //방향키 조작
        MoveControl();
        ActiveControl();
        WeaponSwap();

        //아무 행동도 하지 않을때 (하체)
        if (isAnyActive())
        {
            playerStateUnder = PLAYERSTATE_UNDER.IDLE;
        }

        Move();
        //오브젝트 조작(근처의 탈 것 등등)
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
            // 밑에 차가없음
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
        playerStateUnder = PLAYERSTATE_UNDER.DIE;
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
            playerStateUnder = PLAYERSTATE_UNDER.WALK;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            vDir = -1;
            playerStateUnder = PLAYERSTATE_UNDER.WALK;

        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            hDir = 1;
            playerStateUnder = PLAYERSTATE_UNDER.WALK;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            hDir = -1;
            playerStateUnder = PLAYERSTATE_UNDER.WALK;
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
                case GUNSTATE.NONE:
                    playerStateUpper = PLAYERSTATE_UPPER.PUNCH;
                    isPunch = true;
                    break;
                case GUNSTATE.FIREBOTTLE:
                case GUNSTATE.GRANADE:
                    isAttack = true;
                    playerStateUpper = PLAYERSTATE_UPPER.PUNCH;
                    break;
                case GUNSTATE.PISTOL:
                case GUNSTATE.DOUBLEPISTOL:
                case GUNSTATE.MACHINGUN:
                case GUNSTATE.SLEEPMACHINGUN:
                case GUNSTATE.ROCKETLAUNCHER:
                case GUNSTATE.ELECTRICGUN:
                case GUNSTATE.SHOTGUN:
                case GUNSTATE.FIREGUN:
                    isAttack = true;
                    playerStateUpper = PLAYERSTATE_UPPER.SHOT;
                    break;
                default:
                    break;
            }
        }
        if (!Input.GetKey(KeyCode.A))
        {
            playerStateUpper = PLAYERSTATE_UPPER.IDLE;
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

        playerStateUnder = PLAYERSTATE_UNDER.JUMP;
    }
    void Land()
    {
        isJump = false;
        transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        GetComponent<Rigidbody>().useGravity = true;
        playerStateUnder = PLAYERSTATE_UNDER.LAND;
        //playerStateUnder = PLAYERSTATE_UNDER.IDLE;
    }
    void WeaponSwap()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            if ((int)curGunIndex >= gunList.Count - 1)
            {
                gunList[(int)curGunIndex].gameObject.SetActive(false);
                curGunIndex = GUNSTATE.NONE;
            }
            else
            {
                gunList[(int)curGunIndex].gameObject.SetActive(false);
                curGunIndex++;
            }

            while (gunList[(int)curGunIndex].BulletCount == 0)
            {
                Debug.Log(curGunIndex);

                curGunIndex++;
                if ((int)curGunIndex == gunList.Count)
                {
                    curGunIndex = GUNSTATE.NONE;
                }
            }
            gunList[(int)curGunIndex].gameObject.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if ((int)curGunIndex <= 0)
            {
                gunList[(int)curGunIndex].gameObject.SetActive(false);
                curGunIndex = GUNSTATE.GRANADE;
            }
            else
            {
                gunList[(int)curGunIndex].gameObject.SetActive(false);
                curGunIndex--;
            }
            while (gunList[(int)curGunIndex].BulletCount == 0)
            {
                Debug.Log(curGunIndex);

                curGunIndex--;
                if ((int)curGunIndex == gunList.Count)
                {
                    curGunIndex = GUNSTATE.NONE;
                }
            }
            gunList[(int)curGunIndex].gameObject.SetActive(true);
        }
    }
    #endregion
}