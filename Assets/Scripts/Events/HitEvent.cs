using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEvent : MonoBehaviour
{
    public EnemyController enemy;
    public HitEvent(EnemyController _enemy)//���캯��
    {
        enemy = _enemy;
    }
}
