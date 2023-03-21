using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpNormalAttackEvent
{
    public int currentAttackTimes;
    public UpNormalAttackEvent(int attackTimes)//¹¹Ôìº¯Êý
    {
        //get the attack times for player
        currentAttackTimes = attackTimes;
    }
}   
