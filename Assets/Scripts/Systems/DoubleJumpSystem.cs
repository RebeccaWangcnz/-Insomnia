using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpSystem : MonoBehaviour
{
    public CharacterController player;
    private void OnEnable()
    {
        player.jumpCount = 2;
    }
    private void OnDisable()
    {
        player.jumpCount = 1;
    }
}
