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
        Evently.Instance.Subscribe<HitEvent>(Hit);
    }
    private void OnDisable()
    {
        Evently.Instance.Unsubscribe<NormalAttackEvent>(NormalAttack);
        Evently.Instance.Unsubscribe<HitEvent>(Hit);
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
    //the hit feel when player hit enemy
    private void Hit(HitEvent evt)
    {
        Debug.Log("hit enemy");
        evt.enemy.state = EnemyState.BeHit;
        evt.enemy.isHit = true;
        FlashRed(evt.enemy.sprite);       
    }

    #region FUNCTION
    private void Instantiate()
    {
        playerAnim = player.GetComponent<Animator>();
    }
    private void FlashRed(SpriteRenderer enemySprite)
    {
        enemySprite.color = Color.red;
    }

    #endregion
}
