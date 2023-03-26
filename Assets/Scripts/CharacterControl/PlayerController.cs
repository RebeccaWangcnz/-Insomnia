using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
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
    [Tooltip("the input for upper input")]
    public string yInput;
    [Tooltip("the input for interact")]
    public string interactInput;
    //*********************walk**********************
    [Tooltip("the speed for walking")]
    public float walkSpeed;
    [Tooltip("the max velocityx for stop walking")]
    public float walkStopDeadZone;
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
    [Header("Interation")]
    //************************interact*******************************
    [Tooltip("the length of eye detect")]
    public float eyeRayLength;

    //private parameters
    //*******************************rigidbody********************
    //player's rigidbody
    [HideInInspector]
    public Rigidbody2D rigid;
    //the x velocity for player's rigid
    private float rigidVelocityx;
    //the actual speed of player
    private float playerSpeed;
    private float playerStopDeadZone;
    //*****************************bool for input***********************
    //whether jump is pressed
    private bool jumpPressed;
    //whether attack is pressed
    private bool attackPressed;
    //whther interact is pressed
    private bool interactPressed;
    //****************************Ground check****************
    //whether player is on the ground
    private bool isGrounded;
    //****************************attack*****************************
    //combo times
    private int totalAttackTimes = 2;
    //current combo times
    private int currentAttackTimes;
    //check if it is the upper attack
    private bool isUpAttack;
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
    //**********************Animator***********************************
    [HideInInspector]
    public Animator upperAnimator;
    [HideInInspector]
    public Animator lowerAnimator;
    //******************interact****************************
    //check if player is holding the box
    private bool holdBox;
    //the box that player is pushing
    private BoxComponent boxBePushed;
    //*************collision***************************
    //general layer mask (mask player and confiner)
    private int layerMask;
    //**************other******************************
    //the direction of player
    private float faceDirection;

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
        //get animator
        var animators = GetComponentsInChildren<Animator>();
        upperAnimator = animators[0];
        lowerAnimator = animators[1];
        //set layer mask
        layerMask= ~(1 << 6) & ~(1 << 8);
        //set speed
        playerSpeed = walkSpeed;
        playerStopDeadZone = walkStopDeadZone;
        faceDirection = Mathf.Sign(rigidVelocityx);
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
                InteractInput();
                MoveInput();
                JumpInput();
                AttackInput();
                ChangePlayerFace();
                break;
            case PlayerState.PushBox:
                MoveInput();
                InteractInput();
                break;
            case PlayerState.PrepareHook:
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
        //Interact
        Interact();
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
        rigidVelocityx = Input.GetAxis(xInput) * playerSpeed;
    }
    private void JumpInput()
    {
        //get jump input
        if (Input.GetButtonDown(jumpInput))
        {
            upperAnimator.SetBool("grounded", false);
            lowerAnimator.SetBool("grounded", false);
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
        {
            attackPressed = true;
        }
        //check the attack direction
        if (Input.GetAxis(yInput) > 0)
        {
            isUpAttack = true;
        }
        else
            isUpAttack = false;
    }
    private void ChangePlayerFace()
    {
        //change the face
        if (rigidVelocityx != 0)
            faceDirection = Mathf.Sign(rigidVelocityx);
        transform.localScale = new Vector3(faceDirection, 1, 1);
    }
    private void InteractInput()
    {
        if(Input.GetButtonDown(interactInput))
        {
            interactPressed = true;
        }
        else if(Input.GetButtonUp(interactInput))
        {
            interactPressed = false;
        }

    }

    //**************************************Execute(FixedUpdate)************************************
   private void Interact()
   {
        Debug.DrawRay(transform.position, faceDirection * transform.right * eyeRayLength, Color.yellow);
        //if just hold the box
        if (interactPressed&&!holdBox)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, faceDirection * transform.right, eyeRayLength, layerMask);
            if(hit.collider&&hit.collider.GetComponent<BoxComponent>())
            {
                holdBox = true;
                //chenge state
                playerState = PlayerState.PushBox;
                //get the box
                boxBePushed = hit.collider.GetComponent<BoxComponent>();
                //make box follow player
                boxBePushed.transform.SetParent(transform);
                boxBePushed.GetComponent<Rigidbody2D>().constraints =RigidbodyConstraints2D.FreezeRotation;
                //set speed
                playerSpeed = boxBePushed.pushSpeed;
                playerStopDeadZone = boxBePushed.pushStopDeadZone;
            }
            //if interactable is a door
            else if(hit.collider&& hit.collider.GetComponent<DoorTriggerComponent>())
            {
                var door = hit.collider.GetComponentInParent<DoorComponent>();
                Debug.Log("open");
                door.doorCollider.enabled = false;
            }
        }
        else if(interactPressed && holdBox)
        {
            //animation
            if (boxBePushed.transform.localScale.x*rigidVelocityx>0)
            {
                upperAnimator.SetBool("push", true);
                upperAnimator.SetBool("pull", false);
            }
            else if(boxBePushed.transform.localScale.x *rigidVelocityx < 0)
            {
                upperAnimator.SetBool("push", false);
                upperAnimator.SetBool("pull", true);
            }
            //forbid that player is not in the state
            if(playerState!=PlayerState.PushBox)
            {
                playerState = PlayerState.PushBox;
            }
            boxBePushed.GetComponent<Rigidbody2D>().velocity = rigid.velocity;
        }
        //if just unhold the box
        else if(!interactPressed&&holdBox)
        {
            //animation
            upperAnimator.SetBool("push", false);
            upperAnimator.SetBool("pull", false);
            //other settings
            holdBox = false;
            playerState = PlayerState.Normal;
            boxBePushed.transform.SetParent(null);
            boxBePushed.GetComponent<Rigidbody2D>().constraints |= RigidbodyConstraints2D.FreezePositionX;
            // boxBePushed.GetComponent<BoxCollider2D>().enabled = true;
            playerSpeed = walkSpeed;
            playerStopDeadZone = walkStopDeadZone;
            boxBePushed = null;
        }
   }
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
        if(playerState==PlayerState.Normal|| playerState == PlayerState.PushBox)
        {
            upperAnimator.SetFloat("movespeed", Mathf.Abs(rigidVelocityx * Time.deltaTime));
            lowerAnimator.SetFloat("movespeed", Mathf.Abs(rigidVelocityx * Time.deltaTime));
            //set horizontal speed(in order to stop directly, give it a stopDeadzone)
            if (Mathf.Abs(rigidVelocityx * Time.deltaTime) > playerStopDeadZone)
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
            upperAnimator.SetBool("grounded", true);
            lowerAnimator.SetBool("grounded", true);
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
            upperAnimator.SetBool("grounded", false);
            lowerAnimator.SetBool("grounded", false);
            isAir = true;
            currentJumpCount = jumpChances - 1;
        }
        else
        {
            upperAnimator.SetBool("grounded", false);
            lowerAnimator.SetBool("grounded", false);
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
            //set player state
            //playerState = PlayerState.Attack;
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
            Evently.Instance.Publish(new NormalAttackEvent(currentAttackTimes,isUpAttack));
            //reset attackPressed
            attackPressed = false;
            //disabled attack input
            allowAttackInput = false;
        }
    }
    private bool IsGrounded()
    {
        Vector3 rayStart_1 = groundPoint.position - new Vector3(rayDistance,0,0);
        Vector3 rayStart_2 = groundPoint.position + new Vector3(rayDistance,0,0);
        if (Physics2D.Raycast(rayStart_1, -transform.up,rayLength,layerMask)|| Physics2D.Raycast(rayStart_2, -transform.up, rayLength, layerMask)) 
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
            upperAnimator.SetBool("down", false);
            lowerAnimator.SetBool("down", false);
            rigid.velocity -= new Vector2(0, jumpSpeedUp_2 * Time.deltaTime);
            //if holding jump button,give a force up to jump higher
            if (holdJump)
                rigid.velocity += new Vector2(0, speedForHolding_2 * Time.deltaTime);
        }
        else if (rigid.velocity.y > 0)
        {
            upperAnimator.SetBool("down", false);
            lowerAnimator.SetBool("down", false);
            rigid.velocity -= new Vector2(0, jumpSpeedUp * Time.deltaTime);
            if (holdJump)
                rigid.velocity += new Vector2(0, speedForHolding * Time.deltaTime);
        }
        else if (rigid.velocity.y < 0)
        {
            upperAnimator.SetBool("down", true);
            lowerAnimator.SetBool("down", true);
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
        //rigid.velocity = new Vector2(0, rigid.velocity.y);
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
