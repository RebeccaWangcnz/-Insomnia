using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private Collider2D weaponCollider;
    private PlayerController playerController;
    private Animator anim;
    private void Awake()
    {
        // get weapon collider
        if(GetComponentInChildren<PlayerWeaponComponent>())
        weaponCollider = GetComponentInChildren<PlayerWeaponComponent>().GetComponent<Collider2D>();
        //check if can attack
        playerController = GetComponentInParent<PlayerController>();
        anim = GetComponent<Animator>();
    }
    public void EnableWeaponCollider()
    {
        //enble weapon collider
        weaponCollider.enabled = true;
    }
    public void AllowAttackAndTriggerCombo()
    {
        //allow attack
        playerController.allowAttackInput = true;
        //allow trigger combo
        //playerController.isTriggerCombo = true;
    }
    public void CheckCombo()
    {
        if(playerController.isTriggerCombo)
        {
            if (playerController.currentAttackTimes==1)
            {
                anim.SetBool("combo1", true);
                anim.SetBool("combo2", false);
            }
            else
            {
                anim.SetBool("combo2", true);
                anim.SetBool("combo1", false);
            }
            playerController.isTriggerCombo = false;
        }
    }
    public void DisableWeaponCollider()
    {
        //disable weapon collider
        weaponCollider.enabled = false;
    }
    public void stopTriggerCombo()
    {
        //player cannot trigger combo anymore
        //playerController.isTriggerCombo = false;
    }
}
