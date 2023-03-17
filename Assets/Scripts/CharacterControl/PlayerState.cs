using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    //player can move,jump,attack
    Normal,
    //player cannot move,jump,attack
    Hook,
    //the state after big hook, when player flying according to momentum
    AfterBigHook
}
