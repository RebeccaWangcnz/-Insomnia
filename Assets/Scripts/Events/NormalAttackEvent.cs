using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttackEvent
{
    public int currentAttackTimes;
    public bool isUpAttack;
    public NormalAttackEvent(int attackTimes,bool _isUpAttack)//���캯��
    {
        //get the attack times for player
        currentAttackTimes = attackTimes;
        isUpAttack = _isUpAttack;
    }
}
