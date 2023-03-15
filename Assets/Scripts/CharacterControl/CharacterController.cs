using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    #region Parameters
    [Header("BasicMove")]
    //input
    public string xInput;
    public string jumpInput;
    public string attackInput;
    //walk
    public float walkSpeed;
    public float stopDeadzone;
    //jump
    public Transform groundPoint;
    public float rayLength;
    public float rayDistance;
    public float jumpSpeed;
    public float jumpSpeed_2;
    public float jumpSpeedUp;
    public float jumpSpeedUp_2;
    public float jumpSpeedDown;

    //private parameters
    //rigidbody
    private Rigidbody2D rigid;
    private float rigidVelocityx;
    //bool for input
    private bool jumpPressed;
    private bool attackPressed;
    //Ground check
    private bool isGrounded;
    //attack
    private int totalAttackTimes = 2;
    private int currentAttackTimes;
    [HideInInspector]
    public bool allowAttackInput;
    [HideInInspector]
    public bool canTriggerCombo;
    //jump
    [HideInInspector]
    public int jumpCount = 1;
    private int currentJumpCount;
    private bool isJump;
    #endregion

    #region Execute
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        //attack is allowed in the begining
        allowAttackInput = true;
        //set jump count
        jumpCount = 1;
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
        if(rigid.velocity.y > 0&&jumpCount==2&&currentJumpCount==0)
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
        //Debug.Log(jumpCount);
        //set current jump count
        if(isGrounded&&isJump)
        {
            currentJumpCount=jumpCount;
            isJump = false;
            //forbid jump input available when you in the air
            jumpPressed = false;
        }
        else if(!isGrounded&&!isJump)
        {
            currentJumpCount = jumpCount - 1;
        }
        //first jump
        if (jumpPressed && currentJumpCount>0)
        {
            //speed for double jump
            if(jumpCount == 2 && currentJumpCount == 1)
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
