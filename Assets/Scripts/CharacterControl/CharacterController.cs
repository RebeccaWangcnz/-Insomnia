using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    #region Parameters
    [Header("Params for Test")]
    public HookComponent hook_test;
    [Header("Player Info")]
    //*********************************player state*************
    public PlayerState playerState;
    public Camera mainCamera;
    [Header("BasicMove")]
    //********************input********************
    [Tooltip("the input for move horizontally")]
    public string xInput;
    [Tooltip("the input for jumping")]
    public string jumpInput;
    [Tooltip("the input for attacking")]
    public string attackInput;
    [Tooltip("the input for finding hook")]
    public string hookModeInput;
    //*********************walk**********************
    [Tooltip("the speed for walking")]
    public float walkSpeed;
    [Tooltip("the max velocityx for stop walking")]
    public float stopDeadzone;
    [Tooltip("the propertion for player's move afterbighook")]
    public float AfterBigHookSpeed;
    //********************jump*********************
    [Tooltip("the ground point of player,used for ray which checks the ground")]
    public Transform groundPoint;
    [Tooltip("the ray length for checking the grounded")]
    public float rayLength;
    [Tooltip("the distance between player's 2 feet")]
    public float rayDistance;
    [Tooltip("the jump speed for the first jump")]
    public float jumpSpeed;
    [Tooltip("the jump speed for the second jump")]
    public float jumpSpeed_2;
    [Tooltip("adjust the rate for player's first jump when player is rising up")]
    public float jumpSpeedUp;
    [Tooltip("adjust the rate for player's second jump when player is rising up")]
    public float jumpSpeedUp_2;
    [Tooltip("adjust the rate when player is falling down")]
    public float jumpSpeedDown;
    [Tooltip("the speed up for player holding jump button")]
    public float speedForHolding;
    [Tooltip("the speed up for player's second jump by holding jump button")]
    public float speedForHolding_2;

    //private parameters
    //*******************************rigidbody********************
    //player's rigidbody
    [HideInInspector]
    public Rigidbody2D rigid;
    //the x velocity for player's rigid
    private float rigidVelocityx;
    //*****************************bool for input***********************
    //whether jump is pressed
    private bool jumpPressed;
    //whether attack is pressed
    private bool attackPressed;
    //****************************Ground check****************
    //whether player is on the ground
    private bool isGrounded;
    //****************************attack*****************************
    //combo times
    private int totalAttackTimes = 2;
    //current combo times
    private int currentAttackTimes;
    //whether attack is allowed
    [HideInInspector]
    public bool allowAttackInput;
    //whether player triggers combo
    [HideInInspector]
    public bool canTriggerCombo;
    //**************************jump******************************
    [HideInInspector]
    //the number of jump chances for player
    public int jumpChances = 1;
    //check how many chances of jumping is remained 
    private int currentJumpCount;
    //check whether player is in the process of jumping
    private bool isJump;
    //check whether player is not jump but in the air
    private bool isAir;
    //player hold the jump button to get higher
    private bool holdJump;
    //***********************hook***********************************
    //hook state and inHooking, hook state contains preparehook and inhooking 2 states
    //check if the hookButton is pressed,this is for fixedupdate to get the info of update
    [HideInInspector]
    public bool hookPressed;
    //store the initialSpeed after hook
    [HideInInspector]
    public float initialSpeedAfterHook;
    #endregion

    #region Execute
    private void Awake()
    {
        //get rigidbody
        rigid = GetComponent<Rigidbody2D>();
        //attack is allowed in the begining
        allowAttackInput = true;
        //set jump count
        jumpChances = 1;
        //set player state
        playerState = PlayerState.Normal;
    }
    // Update is called once per frame
    void Update()
    {
        //change player's color to show the states
        ChangeColor();
        //change state to hook
        HookModeInput();
        switch (playerState)
        {
            case PlayerState.Normal:
            case PlayerState.AfterBigHook:
                MoveInput();
                JumpInput();
                AttackInput();
                ChangePlayerFace();
                break;
            case PlayerState.PrepareHook:
                //ResetMove();
                //ResetJump();
                //ResetAttack();
                StartHookingInput();
                Evently.Instance.Publish(new FindingHookEvent(mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.transform.position.z*-1))));
                break;
            case PlayerState.Hook:
                StartHookingInput();
                break;
        }
    }
    private void FixedUpdate()
    {
        //check grounded
        isGrounded= IsGrounded();
        //player move
        Move();
        //jump execute
        Jump();
        //execute attack
        Attack();
        //adjust up and down jump feel
        AdjustJump();
        //start hook
        HookExecute();
    }
    #endregion

    #region Function
    //****************************Input(Update)************************************

    private void HookModeInput()
    {
        if (Input.GetButtonDown(hookModeInput))
        {
            Evently.Instance.Publish(new EnterOrExitHookModeEvent());
        }
    }
    private void StartHookingInput()
    {
        //player can press mouse0 when in the hook state to start hook move
        if (Input.GetButtonDown(attackInput))
        {
            Evently.Instance.Publish(new ResetHookParamsEvent());
            hookPressed = true; 
            //when hook is available then the game continue
            if(hook_test)
                Time.timeScale = 1.0f;
        }
        else if (Input.GetButtonUp(attackInput))
        {
            hookPressed = false;
        }
    }
    private void MoveInput()
    {
        //get horizontal speed
        rigidVelocityx = Input.GetAxis(xInput) * walkSpeed;
    }
    private void JumpInput()
    {
        //get jump input
        if (Input.GetButtonDown(jumpInput))
        {
            jumpPressed = true;
            holdJump = true;
        }
        else if (Input.GetButtonUp(jumpInput))
        {
            holdJump = false;
        }
    }
    private void AttackInput()
    {
        //get attack input
        if (Input.GetButtonDown(attackInput))
            attackPressed = true;
    }
    private void ChangePlayerFace()
    {
        //change the face
        if (rigidVelocityx != 0)
            transform.localScale = new Vector3(Mathf.Sign(rigidVelocityx), 1, 1);
    }

    //**************************************Execute(FixedUpdate)************************************
   private void HookExecute()
   {
        if (hookPressed&&hook_test)
        {
            playerState = PlayerState.Hook;
            //execute hook
            Evently.Instance.Publish(new ExecuteHookEvent(hook_test));
        }
        //to check if the player release the mouse0(has entered the hook state)
        else if (playerState == PlayerState.Hook&&hook_test)
        {
            Evently.Instance.Publish(new AfterHookEvent(hook_test));
        }
    }
    private void Move()
    {
        //when there's no initial speed
        if(playerState==PlayerState.Normal)
        {
            //set horizontal speed(in order to stop directly, give it a stopDeadzone)
            if (Mathf.Abs(rigidVelocityx * Time.deltaTime) > stopDeadzone)
                rigid.velocity = new Vector2(rigidVelocityx * Time.deltaTime, rigid.velocity.y);
            else
                rigid.velocity = new Vector2(0, rigid.velocity.y);
        }
        else if(playerState==PlayerState.AfterBigHook)
        {
            //后退有无力感，但是前进还不错
            rigid.velocity =new Vector2(initialSpeedAfterHook+ rigidVelocityx * Time.deltaTime*AfterBigHookSpeed, rigid.velocity.y);
            //可以模拟很好的阻力，但是前进会很扯
            //rigid.velocity += new Vector2(rigidVelocityx * Time.deltaTime * AfterBigHookSpeed, 0);
        }
    }
    private void Jump()
    {
        //Debug.Log(currentJumpCount);
        //Debug.Log(jumpChances);
        if(isGrounded)
        {
            //reset currentJumpCount
            currentJumpCount=jumpChances;
            //when player jump down to the ground
            if (isJump||isAir)
            {
                //set isJump false
                isJump = false;
                isAir = false;
                //stop hold jump
                holdJump = false;
                //forbid jump input available when you in the air
                jumpPressed = false;
                //the player state maybe afterbighook, so we set it to normal
                playerState=PlayerState.Normal;
            }
        }
        //when player is in the air but not because of the jump, player's jump chances should minus 1
        else if(!isGrounded&&!isJump)
        {
            isAir = true;
            currentJumpCount = jumpChances - 1;
        }
        //first jump
        if (jumpPressed && currentJumpCount>0)
        {
            //speed for double jump
            if(jumpChances == 2 && currentJumpCount == 1)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, jumpSpeed_2 * Time.deltaTime);
            }
            //speed for first jump
            else
                rigid.velocity = new Vector2(rigid.velocity.x, jumpSpeed * Time.deltaTime);
            currentJumpCount--;
            isJump = true;
            jumpPressed = false;
        }
    }
    private void Attack()
    {
        if (attackPressed && allowAttackInput)
        {
            //set attack combo
            if (canTriggerCombo)
            {
                currentAttackTimes = (++currentAttackTimes) % totalAttackTimes;
            }
            else
            {
                currentAttackTimes = 0;
            }
            //Debug.Log(currentAttackTimes);
            //attack
            Evently.Instance.Publish(new NormalAttackEvent(currentAttackTimes));
            //reset attackPressed
            attackPressed = false;
            //disabled attack input
            allowAttackInput = false;
        }
    }
    private bool IsGrounded()
    {
        int layermask = ~(1 << 6)&~(1<<8);
        Vector3 rayStart_1 = groundPoint.position - new Vector3(rayDistance,0,0);
        Vector3 rayStart_2 = groundPoint.position + new Vector3(rayDistance,0,0);
        if (Physics2D.Raycast(rayStart_1, -transform.up,rayLength,layermask)|| Physics2D.Raycast(rayStart_2, -transform.up, rayLength,layermask)) 
        {
            Debug.DrawRay(rayStart_1, -transform.up*rayLength,Color.yellow);
            Debug.DrawRay(rayStart_2, -transform.up*rayLength,Color.yellow);
            return true;
        }
        return false;

    }
    private void AdjustJump()
    {
        //Debug.Log(holdJump);
        //this is the speed for double jump
        if (rigid.velocity.y > 0 && jumpChances == 2 && currentJumpCount == 0)
        {
            rigid.velocity -= new Vector2(0, jumpSpeedUp_2 * Time.deltaTime);
            //if holding jump button,give a force up to jump higher
            if (holdJump)
                rigid.velocity += new Vector2(0, speedForHolding_2 * Time.deltaTime);
        }
        else if (rigid.velocity.y > 0)
        {
            rigid.velocity -= new Vector2(0, jumpSpeedUp * Time.deltaTime);
            if (holdJump)
                rigid.velocity += new Vector2(0, speedForHolding * Time.deltaTime);
        }
        else if (rigid.velocity.y < 0)
        {
            rigid.velocity -= new Vector2(0, jumpSpeedDown * Time.deltaTime);
        }
    }
    //*********************************Reset Params*********************************************
    private void ResetAttack()
    {
        currentAttackTimes = 0;
        attackPressed = false;
        StartHookingInput();
    }
    private void ResetMove()
    {
        rigidVelocityx = 0;
    }
    private void ResetJump()
    {
        currentJumpCount = jumpChances;
        isJump = false;
        isAir = false;
        holdJump = false;
        jumpPressed = false;
    }
    private void ResetHook()
    {
        hookPressed = false;
    }
    //******************************Other*****************************************************
    //该方法只用于测试
    private void ChangeColor()
    {
        if(playerState==PlayerState.PrepareHook)
        {
            GetComponentInChildren<SpriteRenderer>().color = Color.yellow;
        }
        else
        {
            GetComponentInChildren<SpriteRenderer>().color = new Color(150,184,255);
        }
    }
    #endregion
}
