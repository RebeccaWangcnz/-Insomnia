using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum State
{
    IDLE=0,
    WALK=1,
    JUMP=2,
};
[Serializable]
public class Player_State
{
    [SerializeField]
    private State state;
    public Player_State(State _state)
    {
        state = _state;
    }
    public virtual void enter() { }
    public virtual void handleInput(string input) { }
}

public class IDLE : Player_State
{
    private Player player;
    public IDLE(Player _player) : base(State.IDLE)
    {
        player = _player;
    }
    public override void enter()
    {
        //animation
        player.currentWalkSpeed = 0;
    }
    public override void handleInput(string input)
    {
        //input to change state
        if(input=="xInput")
        {
            player.SetState((int)State.WALK);
        }
    }
}

public class WALK : Player_State
{
    private Player player;
    public WALK(Player _player) : base(State.WALK)
    {
        player = _player;
    }
    public override void enter()
    {
        //animation
        player.currentWalkSpeed = player.maxWalkSpeed;
    }
    public override void handleInput(string input)
    {
        //input to change state
        if (input == "xInputRelease")
        {
            player.SetState((int)State.IDLE);
        }
    }
}

