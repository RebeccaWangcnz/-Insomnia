using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsAttackSystem : MonoBehaviour
{
    public PlayerController player;
    public Animator playerAnim;
    private void OnEnable()
    {
        //Instantiate();
        Evently.Instance.Subscribe<NormalAttackEvent>(NormalAttack);
        Evently.Instance.Subscribe<HitEvent>(Hit);
    }
    private void OnDisable()
    {
        Evently.Instance.Unsubscribe<NormalAttackEvent>(NormalAttack);
        Evently.Instance.Unsubscribe<HitEvent>(Hit);
    }
    private void NormalAttack(NormalAttackEvent evt)
    {
        if (evt.isUpAttack)
            playerAnim.SetBool("upattack", true);
        else
            playerAnim.SetBool("upattack", false);
        //play attack animation according to combos

        switch (evt.currentAttackTimes)
        {
            case 1:
                if (player.playerState != PlayerState.Attack)
                {
                    Debug.Log(evt.currentAttackTimes);
                    player.playerState = PlayerState.Attack;
                    playerAnim.SetTrigger("isattack");
                    playerAnim.SetBool("combo1", true);
                    playerAnim.SetBool("combo2", false);
                }
                else
                {
                    Debug.Log("11");
                    player.isTriggerCombo = true;
                }
                break;
            case 0:
                Debug.Log("11");
                player.isTriggerCombo = true;
                //playerAnim.SetBool("combo2", true);
                //playerAnim.SetBool("combo1", false);
                break;
            default:
                break;
        }
    }
    //the hit feel when player hit enemy
    private void Hit(HitEvent evt)
    {
        Debug.Log("hit enemy");
        ////change enemy state
        //evt.enemy.state = EnemyState.BeHit;
        //evt.enemy.isHit = true;
        ////turn red
        //FlashRed(evt.enemy.sprite);       
    }

    #region FUNCTION
    private void Instantiate()
    {
        playerAnim = player.GetComponentInParent<Animator>();
    }
    private void FlashRed(SpriteRenderer enemySprite)
    {
        enemySprite.color = Color.red;
    }

    #endregion
}
