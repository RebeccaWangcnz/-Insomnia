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
    private void OnEnable()
    {
        Evently.Instance.Subscribe<ExecuteHookEvent>(ExecuteHook);
    }
    private void OnDisable()
    {
        Evently.Instance.Unsubscribe<ExecuteHookEvent>(ExecuteHook);
    }
    private void ExecuteHook(ExecuteHookEvent evt)
    {
        if(evt.hookType==HookType.big)
        {
            player.rigid.bodyType = RigidbodyType2D.Kinematic;
            //distance between player and hook
            float distance = Vector2.Distance(evt.hook.transform.position, player.transform.position);
            //the flying direction of player
            Vector3 direction = Vector3.Normalize(evt.hook.transform.position - player.transform.position);
            //add velocity to player
            player.rigid.velocity = direction * playerFlySpeed * Time.deltaTime;
            //stop flying
            if(distance<playerStopThreshold)
            {
                player.rigid.bodyType = RigidbodyType2D.Dynamic;
                player.rigid.AddForce(player.transform.forward * playerStopForce);
                player.playerState = PlayerState.Normal;
                player.hookPressed = false;
            }
        }
        else
        {
            
        }
    }
}
