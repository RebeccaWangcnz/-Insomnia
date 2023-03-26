using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorComponent : MonoBehaviour
{
    public bool canOpen;
    [HideInInspector]
    public Collider2D doorCollider;
    private void Awake()
    {
        doorCollider = GetComponent<BoxCollider2D>();
    }
}
