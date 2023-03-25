using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private Collider2D weaponCollider;
    private PlayerController playerController;
    private void Awake()
    {
        // get weapon collider
        if(GetComponentInChildren<PlayerWeaponComponent>())
        weaponCollider = GetComponentInChildren<PlayerWeaponComponent>().GetComponent<Collider2D>();
        //check if can attack
        playerController = GetComponent<PlayerController>();
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
        playerController.canTriggerCombo = true;
    }
    public void DisableWeaponCollider()
    {
        //disable weapon collider
        weaponCollider.enabled = false;
    }
    public void stopTriggerCombo()
    {
        //player cannot trigger combo anymore
        playerController.canTriggerCombo = false;
    }
}
