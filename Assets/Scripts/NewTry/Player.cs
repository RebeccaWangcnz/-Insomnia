using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Player_State[] states;
    [Header("Current Status")]
    public Player_State currentState;
    public float currentWalkSpeed;

    [Header("Params Settings")]
    public float maxWalkSpeed;

    private Rigidbody2D rigid;

    private void Awake()
    {
        states =new Player_State[]{ new IDLE(this), new WALK(this)};
        currentState = states[0];
        rigid = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        //Debug.Log(currentState.state);
        HandleInput(PlayerInput.lastKey);
    }
    private void FixedUpdate()
    {
        rigid.velocity = new Vector2(currentWalkSpeed * Time.deltaTime, rigid.velocity.y);
    }
    private void HandleInput(string input)
    {
        currentState.handleInput(input);
    }
    public void SetState(int state)
    {
        currentState = states[state];
        currentState.enter();
    }

}
