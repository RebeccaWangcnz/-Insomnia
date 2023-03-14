using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [Header("BasicMove")]
    //input
    public string xInput;
    public string jumpInput;
    //walk
    public float walkSpeed;
    public float stopDeadzone;
    //jump
    public Transform groundPoint;
    public float rayLength;
    public float rayDistance;
    public float jumpSpeed;
    public float jumpSpeedUp;
    public float jumpSpeedDown;
    public int jumpCount;

    //private parameters
    //rigidbody
    private Rigidbody2D rigid;
    private float rigidVelocityx;
    //bool for input
    private bool jumpPressed;
    //Ground check
    private bool isGrounded;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        //get horizontal speed
        rigidVelocityx = Input.GetAxis(xInput) * walkSpeed;
        //get jump input
        if (Input.GetButtonDown(jumpInput))
            jumpPressed = true;
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
        if (jumpPressed&&isGrounded)
            Jump();
        //adjust up and down jump feel
        if(rigid.velocity.y>0)
        {
            rigid.velocity -=new Vector2(0, jumpSpeedUp * Time.deltaTime) ;
        }
        else if(rigid.velocity.y<0)
        {
            rigid.velocity -= new Vector2(0, jumpSpeedDown * Time.deltaTime);
        }
    }

    private void Jump()
    {
        rigid.velocity = new Vector2(rigid.velocity.x, jumpSpeed * Time.deltaTime);
        jumpPressed = false;
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
}
