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
    public LineRenderer line;

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
        Evently.Instance.Subscribe<EnterOrExitHookModeEvent>(EnterOrExitHookMode);
        Evently.Instance.Subscribe<FindingHookEvent>(FindingHook);
        Evently.Instance.Subscribe<ExecuteHookEvent>(ExecuteHook);
        Evently.Instance.Subscribe<AfterHookEvent>(AfterHook);
        Evently.Instance.Subscribe<ResetHookParamsEvent>(ResetHookParams);
    }
    private void OnDisable()
    {
        Evently.Instance.Unsubscribe<EnterOrExitHookModeEvent>(EnterOrExitHookMode);
        Evently.Instance.Unsubscribe<FindingHookEvent>(FindingHook);
        Evently.Instance.Unsubscribe<ExecuteHookEvent>(ExecuteHook);
        Evently.Instance.Unsubscribe<AfterHookEvent>(AfterHook);
        Evently.Instance.Unsubscribe<ResetHookParamsEvent>(ResetHookParams);
    }
    //detect sphere
    private void OnDrawGizmos()
    {
        //draw sphere
        Gizmos.DrawWireSphere(player.transform.position, findingRadius);
    }
    private void FindingHook(FindingHookEvent evt)
    {
        if (hooks == null||hooks.Length==0)
            return;
        nearestHook = hooks[0];
        foreach (var hook in hooks)
        {
            //Debug.Log("hook:"+hook.name +Vector2.Distance(evt.mousePos, hook.transform.position)+","+"nearest:"+nearestHook.name + Vector2.Distance(evt.mousePos, nearestHook.transform.position));
            //Debug.Log(evt.mousePos);
            //change player facing
            var direction = evt.mousePos.x- player.transform.position.x;
            if (direction!= 0)
                player.transform.localScale = new Vector3(Mathf.Sign(direction), 1, 1);
            //find the nearest hook to mouse pos
            if (Vector2.Distance(evt.mousePos, hook.transform.position) < Vector2.Distance(evt.mousePos, nearestHook.transform.position))
            {
                nearestHook = hook;
            }
        }
        //check if it is occluded
        var layermask = ~(1 << 6) & ~(1 << 8);
        var dir = nearestHook.transform.position - player.transform.position;
        //draw line on the screen
        Debug.DrawRay(player.transform.position, dir, Color.blue);
        var raycast = Physics2D.Raycast(player.transform.position, dir, findingRadius, layermask);
        //something between player and hook
        if (raycast&&!raycast.collider.GetComponent<HookComponent>())
        {
            //Debug.Log("has occlusion");
            nearestHook = null;
            player.hook_test = null;
            DrawLine(raycast.point,Color.red);
        }
        else
        {
            player.hook_test = nearestHook.GetComponent<HookComponent>();
            DrawLine(nearestHook.transform.position,Color.green);
        }
    }

    private void ExecuteHook(ExecuteHookEvent evt)
    {
        
        Time.timeScale = 1f;
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
            else
                DrawLine(evt.hook.transform.position, Color.blue);

        }
        else
        {
            //add velocity to hook
            evt.hookRigid.velocity += hookFlySpeed * Time.deltaTime * -new Vector2(direction.x,direction.y);
            //stop hook flying
            if(distance<hookStopThreshold)
                AfterHook(new AfterHookEvent(evt.hook));
            else
                DrawLine(evt.hook.transform.position, Color.blue);
        }        
    }
    private void AfterHook(AfterHookEvent evt)
    {
        if (evt.hookType == HookType.big)
        {
            player.rigid.bodyType = RigidbodyType2D.Dynamic;
            //����ˮƽ�ʹ�ֱ���ٶȵ������ƿ����ָл��һ��
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
        ResetLine();
    }
    private void EnterOrExitHookMode(EnterOrExitHookModeEvent evt)
    {
        if(player.playerState == PlayerState.Normal|| player.playerState == PlayerState.AfterBigHook)
        {
            //if player is not in the normal state, he cannot enter hook mode(Ҳ��afterhook״̬Ҳ����ֱ���л���hookģʽ)
            Time.timeScale = 0f;
            player.playerState = PlayerState.PrepareHook;
            PrepareHook();
        }
        else if(player.playerState == PlayerState.PrepareHook)
        {
            Time.timeScale = 1f;
            //if player hasn't hooked,but enter the hook mode, change state back to normal
            player.playerState = PlayerState.Normal;
            ResetLine();
            ResetHookParams(new ResetHookParamsEvent());
            player.hook_test = null;
        }
    }

    private void ResetHookParams(ResetHookParamsEvent evt)
    {
        hooks = null;
        nearestHook = null;
    }

    #region FUNCTION
    private void PrepareHook()
    {
        hooks = Physics2D.OverlapCircleAll(player.transform.position, findingRadius, 1 << 7);
    }
    private void DrawLine(Vector2 endPos,Color lineCol)
    {
        line.enabled = true;
        line.SetPosition(0, player.transform.position);
        line.SetPosition(1, endPos);
        line.startColor = line.endColor =lineCol;
    }
    private void ResetLine()
    {
        line.enabled = false;
        line.SetPosition(0, Vector3.zero);
        line.SetPosition(1, Vector3.zero);
    }
    #endregion
}
