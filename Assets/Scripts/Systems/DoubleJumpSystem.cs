using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpSystem : MonoBehaviour
{
    public PlayerController player;
    
    private void OnEnable()
    {
        //change the jump chance for player
        player.jumpChances = 2;
    }
    private void OnDisable()
    {
        player.jumpChances = 1;
    }
}
