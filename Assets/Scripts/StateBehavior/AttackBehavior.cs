using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehavior : StateMachineBehaviour
{
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //reset animator bool
        //animator.SetBool("combo1", false);
        //animator.SetBool("combo2", false);
        //animator.gameObject.GetComponentInParent<PlayerController>().allowAttackInput = true;
        //animator.gameObject.GetComponentInParent<PlayerController>().currentAttackTimes = 0;
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("combo1", false);
        animator.SetBool("combo2", false);
        animator.gameObject.GetComponentInParent<PlayerController>().allowAttackInput = true;
        animator.gameObject.GetComponentInParent<PlayerController>().currentAttackTimes = 0;
        animator.gameObject.GetComponentInParent<PlayerController>().playerState = PlayerState.Normal;
    }

}
