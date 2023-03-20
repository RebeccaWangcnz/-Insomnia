using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponComponent : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Collider2D>().enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //hit the enemy
        if(collision.GetComponent<EnemyController>())
        {
            Evently.Instance.Publish(new HitEvent(collision.GetComponent<EnemyController>()));
        }
    }
}
