using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookSystem : MonoBehaviour
{
    public CharacterController player;
    [Header("Big Hook Params")]
    [Tooltip("the speed for player flying to the hook")]
    public float playerFlySpeed;
    [Tooltip("the nearest distance player stop hook")]
    public float playerStopThreshold;
    [Tooltip("fly force when stop hooking")]
    public float playerStopForce;

    [Header("Small Hook Params")]
    [Tooltip("the speed for hook fly to player")]
    public float hookFlySpeed;
    [Tooltip("the nearest distance hook stop")]
    public float hookStopThreshold;

    [Header("Finding Hook Params")]
    public float findingRadius;

    //private parameters
    //distance between player and hook
    private float distance;
    //the flying direction of player(- is the direction of hook)
    private Vector3 direction;
    //the nearest hook
    private Collider2D nearestHook=null;
    //all the hooks
    private Collider2D[] hooks=null;
    private void OnEnable()
    {
        Evently.Instance.Subscribe<ExecuteHookEvent>(ExecuteHook);
        Evently.Instance.Subscribe<AfterHookEvent>(AfterHook);
        Evently.Instance.Subscribe<FindingHookEvent>(FindingHook);
        Evently.Instance.Subscribe<PrepareHookEvent>(PrepareHook);
        Evently.Instance.Subscribe<ResetHookParamsEvent>(ResetHookParams);
    }
    private void OnDisable()
    {
        Evently.Instance.Unsubscribe<ExecuteHookEvent>(ExecuteHook);
        Evently.Instance.Unsubscribe<AfterHookEvent>(AfterHook);
        Evently.Instance.Unsubscribe<FindingHookEvent>(FindingHook);
        Evently.Instance.Unsubscribe<PrepareHookEvent>(PrepareHook);
        Evently.Instance.Unsubscribe<ResetHookParamsEvent>(ResetHookParams);
    }
    //detect sphere
    private void OnDrawGizmos()
    {
        //draw sphere
        Gizmos.DrawWireSphere(player.transform.position, findingRadius);
    }
    private void PrepareHook(PrepareHookEvent evt)
    {
        hooks = Physics2D.OverlapCircleAll(player.transform.position, findingRadius, 1 << 7);
    }
    private void FindingHook(FindingHookEvent evt)
    {
        if (hooks == null||hooks.Length==0)
            return;
        nearestHook = hooks[0];
        foreach (var hook in hooks)
        {
            //Debug.Log("hook:"+hook.name +Vector2.Distance(evt.mousePos, hook.transform.position)+","+"nearest:"+nearestHook.name + Vector2.Distance(evt.mousePos, nearestHook.transform.position));
            Debug.Log(evt.mousePos);
            if (Vector2.Distance(evt.mousePos, hook.transform.position) < Vector2.Distance(evt.mousePos, nearestHook.transform.position))
            {
                nearestHook = hook;
            }
        }
        player.hook_test = nearestHook.GetComponent<HookComponent>();
    }
    private void ResetHookParams(ResetHookParamsEvent evt)
    {
        hooks = null;
        nearestHook = null;
    }
    private void ExecuteHook(ExecuteHookEvent evt)
    {
        
        distance = Vector2.Distance(evt.hook.transform.position, player.transform.position);
        direction = Vector3.Normalize(evt.hook.transform.position - player.transform.position);
        if (evt.hookType==HookType.big)
        {
            player.rigid.bodyType = RigidbodyType2D.Kinematic;
            //add velocity to player
            player.rigid.velocity = direction * playerFlySpeed * Time.deltaTime;
            //stop flying
            if (distance < playerStopThreshold)
                AfterHook(new AfterHookEvent(evt.hook));
        }
        else
        {
            //add velocity to hook
            evt.hookRigid.velocity += hookFlySpeed * Time.deltaTime * -new Vector2(direction.x,direction.y);
            //stop hook flying
            if(distance<hookStopThreshold)
                AfterHook(new AfterHookEvent(evt.hook));
        }
    }
    private void AfterHook(AfterHookEvent evt)
    {
        if (evt.hookType == HookType.big)
        {
            player.rigid.bodyType = RigidbodyType2D.Dynamic;
            //这里水平和垂直的速度单独控制可能手感会好一点
            player.rigid.velocity = direction * playerStopForce;
            player.initialSpeedAfterHook = player.rigid.velocity.x;
            player.playerState = PlayerState.AfterBigHook;
        }
        else
        {
            evt.hookRigid.velocity = Vector2.zero;
            player.playerState = PlayerState.Normal;            
        }
        player.hookPressed = false;
        player.hook_test = null;
    }
}
