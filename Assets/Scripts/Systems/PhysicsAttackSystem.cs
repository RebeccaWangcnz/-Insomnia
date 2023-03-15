using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsAttackSystem : MonoBehaviour
{
    public GameObject player;
    private Animator playerAnim;
    private void OnEnable()
    {
        Instantiate();
        Evently.Instance.Subscribe<NormalAttackEvent>(NormalAttack);
    }
    private void OnDisable()
    {
        Evently.Instance.Unsubscribe<NormalAttackEvent>(NormalAttack);
    }
    private void NormalAttack(NormalAttackEvent evt)
    {
        //play attack animation according to combos
        switch (evt.currentAttackTimes)
        {
            case 0:
                playerAnim.SetBool("combo1", true);
                playerAnim.SetBool("combo2", false);
                break;
            case 1:
                playerAnim.SetBool("combo2", true);
                playerAnim.SetBool("combo1", false);
                break;
            default:
                break;
        }
    }

    private void Instantiate()
    {
        playerAnim = player.GetComponent<Animator>();
    }
}
