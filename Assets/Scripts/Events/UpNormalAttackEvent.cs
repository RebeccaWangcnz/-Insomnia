using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpNormalAttackEvent
{
    public int currentAttackTimes;
    public UpNormalAttackEvent(int attackTimes)//���캯��
    {
        //get the attack times for player
        currentAttackTimes = attackTimes;
    }
}   
