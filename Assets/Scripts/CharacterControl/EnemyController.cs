using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyState state;
    //the color before flash red
    private Color normalCol;
    //the sprite of the enemy
    [HideInInspector]
    public SpriteRenderer sprite;
    //make sure the behit effect only play once
    [HideInInspector]
    public bool isHit;
    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        normalCol = sprite.color;
    }
    private void Update()
    {
        
    }
    private void FixedUpdate()
    {
        //turn back
        if(state==EnemyState.BeHit&&isHit)
        {
            Invoke("FlashBack",0.1f);
            isHit = false;
        }
    }

    public void FlashBack()
    {
        sprite.color = normalCol;
        //change state to normal
        state = EnemyState.Normal;
    }
}
