using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenMM8_NPC_Rotator : MonoBehaviour
{
    // Relative to camera
    enum LookDirection { Front, FrontRight, Right, BackRight, Back, BackLeft, Left, FrontLeft };

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Get Main Camera angle
        // Get this NPC's Angle
        // Substract
        // Get LookDirection from gotten angle between Main Camera and NPC
        // Check if LookDirection changed
        // If it did change, set current to new one
        //     and get current animation's state - 0.0f - 1.0f scaled time which
        //     elapsed from animation's start
        //   Set new animation with respect to current LookDirection
        //   Set new animation's scaled time from last animation to keep it consistent
    }
}
