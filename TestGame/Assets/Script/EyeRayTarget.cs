using System;
using UnityEngine;

public class EyeRayTarget : MonoBehaviour
{
    private bool isBrainControl;


    public void OnBrainControl()
    {
        isBrainControl = true;
    }
}
