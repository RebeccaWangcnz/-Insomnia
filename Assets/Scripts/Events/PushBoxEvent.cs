using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBoxEvent 
{
    public RaycastHit2D hit;
    //whether player is pushing
    public bool isPushing;
    public PushBoxEvent(RaycastHit2D _hit,bool _isPushing)
    {
        hit = _hit;
        isPushing = _isPushing;
    }
}
