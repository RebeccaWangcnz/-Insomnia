using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterHookEvent
{
    public HookComponent hook;
    public HookType hookType;
    public Rigidbody2D hookRigid;
    public AfterHookEvent(HookComponent _hook)//���캯��
    {
        hook = _hook;
        //get the hook type
        hookType = hook.hookType;
        //get hook rigid
        hookRigid = hook.GetComponent<Rigidbody2D>();
    }
}
