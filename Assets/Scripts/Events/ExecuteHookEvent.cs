using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecuteHookEvent
{
    public HookComponent hook;
    public HookType hookType;
    public ExecuteHookEvent(HookComponent _hook)//���캯��
    {
        hook = _hook;
        //get the hook type
        hookType = hook.hookType;
    }
}
