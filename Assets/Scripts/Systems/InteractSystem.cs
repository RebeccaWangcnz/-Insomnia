using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSystem : MonoBehaviour
{
    public PlayerController player;
    private BoxComponent boxBePushed;
    private void OnEnable()
    {
        Evently.Instance.Subscribe<PushBoxEvent>(PushBox);
    }
    private void OnDisable()
    {
        Evently.Instance.Unsubscribe<PushBoxEvent>(PushBox);
    }
    private void PushBox(PushBoxEvent evt)
    {
        //detect box && the player is pushing
        if (evt.hit.collider != null && evt.hit.collider.GetComponent<BoxComponent>() && evt.isPushing)
        {
            //change animation
            player.upperAnimator.SetBool("ispushing", true);
            boxBePushed = evt.hit.collider.GetComponent<BoxComponent>();
            player.playerState = PlayerState.PushBox;
            boxBePushed.rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        //end pushing
        else if (player.playerState == PlayerState.PushBox)
        {
            player.upperAnimator.SetBool("ispushing", false);
            player.playerState = PlayerState.Normal;
            boxBePushed.rigid.constraints |= RigidbodyConstraints2D.FreezePositionX;
            boxBePushed = null;
        }
    }
}
