using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxComponent : MonoBehaviour
{
    public float pushSpeed;//represent the speed of pushing
    public float pushStopDeadZone;
    [HideInInspector]
    public Rigidbody2D rigid;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.constraints|=RigidbodyConstraints2D.FreezePositionX;
    }

} 
