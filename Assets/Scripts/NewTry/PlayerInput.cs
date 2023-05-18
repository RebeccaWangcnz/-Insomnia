using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Tooltip("the input for jumping")]
    public string xInput;
    public static string lastKey;
    private void Update()
    {
        if(Input.GetAxis(xInput)!=0)
        {
            lastKey = "xInput";
        }
        if (Input.GetAxis(xInput) == 0)
        {
            lastKey = "xInputRelease";
        }
    }
}
