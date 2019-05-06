using Assets.OpenMM8.Scripts.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SpriteBillboardAnimator))]
public class SpriteRotator : MonoBehaviour
{
    Camera Camera;
    Transform CameraTransform;
    LookDirection LookDir = LookDirection.Front;
    bool IsNpc = false;
    public float facingAngle;

    SpriteRenderer Renderer;
    Animator Animator;

    public float RefreshRate = 50.0f;
    public bool LookLocked = false;

    SpriteBillboardAnimator SpriteBillboardAnimator;

    // Use this for initialization
    void Start()
    {
        Camera = Camera.main;
        CameraTransform = Camera.main.transform;

        Renderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();
        IsNpc = GetComponent<BaseNpc>() != null;
        SpriteBillboardAnimator = GetComponent<SpriteBillboardAnimator>();

        InvokeRepeating("AlignRotation", 0.0f, RefreshRate / 1000.0f);
    }

    public void OnLookDirectionChanged(LookDirection current)
    {
        LookDir = current;

        SpriteBillboardAnimator.SetLookDirection(LookDir);
    }

    // Instead of Update to not drain CPU
    public void AlignRotation()
    {
        if (LookLocked)
        {
            return;
        }

        // Inspipred by (credit goes to): https://github.com/Interkarma/daggerfall-unity

        Transform parent = transform;
        if (parent == null)
            return;

        Vector3 cameraPosition = Camera.transform.position;
        Vector3 viewDirection = -new Vector3(Camera.transform.forward.x, 0, Camera.transform.forward.z);

        // Get direction normal to camera, ignore y axis
        Vector3 dir = Vector3.Normalize(
            new Vector3(cameraPosition.x, 0, cameraPosition.z) -
            new Vector3(transform.position.x, 0, transform.position.z));

        // Get parent forward normal, ignore y axis
        Vector3 parentForward = transform.forward;
        parentForward.y = 0;

        // Get angle and cross product for left/right angle
        facingAngle = Vector3.Angle(dir, parentForward);
        facingAngle = facingAngle * -Mathf.Sign(Vector3.Cross(dir, parentForward).y);

        LookDirection currLook = LookDirection.Front;
        // Right-hand side
        if (facingAngle > 0.0f && facingAngle < 22.5f)
        {
            // orientation = 0;
            currLook = LookDirection.Front;
        }
        if (facingAngle > 22.5f && facingAngle < 67.5f)
        {
            // orientation = 7;
            currLook = LookDirection.FrontRight;
        }
        if (facingAngle > 67.5f && facingAngle < 112.5f)
        {
            // orientation = 6;
            currLook = LookDirection.Right;
        }   
        if (facingAngle > 112.5f && facingAngle < 157.5f)
        {
            // orientation = 5;
            currLook = LookDirection.BackRight;
        }   
        if (facingAngle > 157.5f && facingAngle <= 180.0f)
        {
            // orientation = 4;
            currLook = LookDirection.Back;
        }
            

        // Left-hand side
        if (facingAngle < 0.0f && facingAngle > -22.5f)
        {
            currLook = LookDirection.Front;
        }
        if (facingAngle < -22.5f && facingAngle > -67.5f)
        {
            currLook = LookDirection.FrontLeft;
        }   
        if (facingAngle < -67.5f && facingAngle > -112.5f)
        {
            currLook = LookDirection.Left;
        }   
        if (facingAngle < -112.5f && facingAngle > -157.5f)
        {
            currLook = LookDirection.BackLeft;
        }   
        if (facingAngle < -157.5f && facingAngle >= -180.0f)
        {
            currLook = LookDirection.Back;
        }
            

        // Change person to this orientation
        if (currLook != LookDir)
        {
            if (IsNpc)
            {
                // Hack
                BaseNpc.NpcState state = (BaseNpc.NpcState)Animator.GetInteger("State");
                if (state == BaseNpc.NpcState.Attacking)
                {
                    return;
                }
            }

            OnLookDirectionChanged(currLook);
        }
    }
}
