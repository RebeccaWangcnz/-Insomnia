using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    #region Parameters
    [Header("BasicMove")]
    //********************input********************
    [Tooltip("the input for move horizontally")]
    public string xInput;
    [Tooltip("the input for jumping")]
    public string jumpInput;
    [Tooltip("the input for attacking")]
    public string attackInput;
    //*********************walk**********************
    [Tooltip("the speed for walking")]
    public float walkSpeed;
    [Tooltip("the max velocityx for stop walking")] 
    public float stopDeadzone;
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

    //private parameters
    //*******************************rigidbody********************
    //player's rigidbody
    private Rigidbody2D rigid;
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
    }
    // Update is called once per frame
    void Update()
    {
        //get horizontal speed
        rigidVelocityx = Input.GetAxis(xInput) * walkSpeed;
        //get jump input
        if (Input.GetButtonDown(jumpInput))
            jumpPressed = true;
        //get attack input
        if (Input.GetButtonDown(attackInput))
            attackPressed = true;
        //change the face
        if(rigidVelocityx!=0)
            transform.localScale =new Vector3( Mathf.Sign(rigidVelocityx), 1, 1);
    }
    private void FixedUpdate()
    {
        //check grounded
        isGrounded= IsGrounded();
        //set horizontal speed(in order to stop directly, give it a stopDeadzone)
        if (Mathf.Abs(rigidVelocityx * Time.deltaTime) > stopDeadzone)
            rigid.velocity = new Vector2(rigidVelocityx*Time.deltaTime, rigid.velocity.y);
        else
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        //jump execute
        Jump();
        //execute attack
        if(attackPressed&&allowAttackInput)
        {
            //set attack combo
            if(canTriggerCombo)
            {
                currentAttackTimes=(++currentAttackTimes)% totalAttackTimes;
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
        //adjust up and down jump feel
        //this is the speed for double jump
        if(rigid.velocity.y > 0&&jumpChances==2&&currentJumpCount==0)
        {
            rigid.velocity -= new Vector2(0, jumpSpeedUp_2 * Time.deltaTime);
        }
        else if(rigid.velocity.y>0)
        {
            rigid.velocity -=new Vector2(0, jumpSpeedUp * Time.deltaTime) ;
        }
        else if(rigid.velocity.y<0)
        {
            rigid.velocity -= new Vector2(0, jumpSpeedDown * Time.deltaTime);
        }
    }
    #endregion

    #region Function
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
                //forbid jump input available when you in the air
                jumpPressed = false;
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
                rigid.velocity = new Vector2(rigid.velocity.x, jumpSpeed_2 * Time.deltaTime);
            //speed for first jump
            else
                rigid.velocity = new Vector2(rigid.velocity.x, jumpSpeed * Time.deltaTime);
            currentJumpCount--;
            isJump = true;
            jumpPressed = false;
        }
    }
    private bool IsGrounded()
    {
        int layermask = ~(1 << 6);
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
    #endregion
}
