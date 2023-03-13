using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [Header("BasicMove")]
    public string xInput;

    public float walkSpeed;
    public float stopDeadzone;
    private Rigidbody2D rigid;
    private float rigidVelocityx;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        //get horizontal speed
        rigidVelocityx = Input.GetAxis(xInput) * walkSpeed;
        //change the face
        if(rigidVelocityx!=0)
            transform.localScale =new Vector3( Mathf.Sign(rigidVelocityx), 1, 1);
    }
    private void FixedUpdate()
    {
        //set horizontal speed(in order to stop directly, give it a stopDeadzone)
        if (Mathf.Abs(rigidVelocityx * Time.deltaTime) > stopDeadzone)
            rigid.velocity = new Vector2(rigidVelocityx*Time.deltaTime, 0);
        else
            rigid.velocity = new Vector2(0, rigid.velocity.y);
    }
}
