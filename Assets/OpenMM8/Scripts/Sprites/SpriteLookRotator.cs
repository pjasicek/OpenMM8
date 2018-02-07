using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class SpriteLookRotator : MonoBehaviour
{
    // Relative to camera
    public enum LookDirection { Front, FrontRight, Right, BackRight, Back, BackLeft, Left, FrontLeft };

    Camera Camera;
    Transform CameraTransform;
    LookDirection LookDir = LookDirection.Front;
    bool IsNpc = false;

    SpriteRenderer Renderer;
    Animator Animator;

    public float RefreshRate = 50.0f;
    public bool LookLocked = false;

    // Use this for initialization
    void Start()
    {
        Camera = Camera.main;
        CameraTransform = Camera.main.transform;

        Renderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();
        IsNpc = GetComponent<BaseNpc>() != null;

        InvokeRepeating("AlignRotation", 0.0f, RefreshRate / 1000.0f);
    }

    public void OnLookDirectionChanged(LookDirection current)
    {
        LookDir = current;

        //Debug.Log("Look direction changed to: " + current);

        Renderer.flipX = false;

        if (LookDir == LookDirection.Front)
        {
            Animator.SetInteger("LookDirection", 0);
        }
        else if (LookDir == LookDirection.FrontRight)
        {
            //Debug.Log("FrontRight");
            Animator.SetInteger("LookDirection", 1);
            Renderer.flipX = true;
        }
        else if (LookDir == LookDirection.Right)
        {
            Animator.SetInteger("LookDirection", 2);
            Renderer.flipX = true;
        }
        else if (LookDir == LookDirection.BackRight)
        {
            Animator.SetInteger("LookDirection", 3);
            Renderer.flipX = true;
        }
        else if (LookDir == LookDirection.Back)
        {
            Animator.SetInteger("LookDirection", 4);
        }
        else if (LookDir == LookDirection.BackLeft)
        {
            Animator.SetInteger("LookDirection", 3);
            //m_Renderer.flipX = true;
        }
        else if (LookDir == LookDirection.Left)
        {
            Animator.SetInteger("LookDirection", 2);
            //m_Renderer.flipX = true;
        }
        else if (LookDir == LookDirection.FrontLeft)
        {
            //Debug.Log("FrontLeft");
            Animator.SetInteger("LookDirection", 1);
            //m_Renderer.flipX = true;
        }
        else
        {
            UnityEngine.Assertions.Assert.IsTrue(false, "Invalid LookDirection: " + LookDir);
        }
    }

    // Instead of Update to not drain CPU
    public void AlignRotation()
    {
        //Debug.Log("Object rotation: " + transform.rotation.eulerAngles);
        //Debug.Log("Camera rotation: " + m_CameraTransform.rotation.eulerAngles);

        if (LookLocked)
        {
            return;
        }

        float cameraY = CameraTransform.rotation.eulerAngles.y;
        float thisY = transform.rotation.eulerAngles.y;

        float diffAngle = cameraY - thisY;
        diffAngle = (diffAngle + 180) % 360.0f - 180;

        LookDirection currLook = LookDirection.Front;
        if (diffAngle < 22.5f && diffAngle > -22.5f)
        {
            currLook = LookDirection.Back;
        }
        else if (diffAngle < -22.5f && diffAngle > -67.5f)
        {
            currLook = LookDirection.BackRight;
        }
        else if (diffAngle < -67.5f && diffAngle > -112.5f)
        {
            currLook = LookDirection.Right;
        }
        else if (diffAngle < -112.5f && diffAngle > -157.5f)
        {
            currLook = LookDirection.FrontRight;
        }
        else if (diffAngle < -157.5f && diffAngle > -180.0f)
        {
            currLook = LookDirection.Front;
        }
        else if (diffAngle > 157.5f && diffAngle < 180.0f)
        {
            currLook = LookDirection.Front;
        }
        else if (diffAngle < 157.5f && diffAngle > 112.5f)
        {
            currLook = LookDirection.FrontLeft;
        }
        else if (diffAngle < 112.5f && diffAngle > 67.5f)
        {
            currLook = LookDirection.Left;
        }
        else if (diffAngle < 67.5f && diffAngle > 22.5f)
        {
            currLook = LookDirection.BackLeft;
        }
        else
        {
            /*Debug.Log("SHOULD NOT EVER HAPPEN ! ANGLE: " + diffAngle);
            Debug.Log("CameraY: " + cameraY + ", ThisY: " + thisY);*/
            return;
        }

        if (currLook != LookDir)
        {
            if (IsNpc)
            {
                // Hack
                BaseNpc.NpcState state = (BaseNpc.NpcState)Animator.GetInteger("State");
                if (state == BaseNpc.NpcState.Attacking)
                {
                    return;
                    //currLook = LookDirection.Front;
                }
            }

            OnLookDirectionChanged(currLook);
        }

        /*if (diffAngle < 0)
        {
            diffAngle *= -1;
        }
        diffAngle = (diffAngle + 180.0f) % 360.0f;*/

        /*Quaternion a = m_CameraTransform.rotation;
        a.x = 0;
        a.z = 0;

        Quaternion b = transform.rotation;
        b.x = 0;
        b.z = 0;
        diffAngle = Quaternion.Angle(a, b);

        var lookPos = m_CameraTransform.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);*/

        //Debug.Log("Angle diff: " + (diffAngle));

        /*LineRenderer lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetVertexCount(2);
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.forward * 20 + transform.position);

        LineRenderer lineRendere2r = m_CameraTransform.parent.GetComponent<LineRenderer>();
        lineRendere2r.SetVertexCount(2);
        lineRendere2r.SetColors(Color.red, Color.red);
        lineRendere2r.SetPosition(0, m_CameraTransform.position);
        lineRendere2r.SetPosition(1, m_CameraTransform.forward * 20 + m_CameraTransform.position);*/
    }
}
