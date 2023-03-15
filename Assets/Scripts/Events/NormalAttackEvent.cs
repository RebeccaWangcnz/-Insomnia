using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttackEvent
{
    public int currentAttackTimes;
    public NormalAttackEvent(int attackTimes)//¹¹Ôìº¯Êý
    {
        //get the attack times for player
        currentAttackTimes = attackTimes;
    }
}
