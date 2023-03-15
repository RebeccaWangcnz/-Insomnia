using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Collider2D>().enabled = false;
    }
}
