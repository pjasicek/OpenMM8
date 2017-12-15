using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class OpenMM8_NPC_Rotator : MonoBehaviour
{
    // Relative to camera
    enum LookDirection { Front, FrontRight, Right, BackRight, Back, BackLeft, Left, FrontLeft };

    Camera m_Camera;
    Transform m_CameraTransform;
    LookDirection m_LookDirection = LookDirection.Front;

    SpriteRenderer m_Renderer;
    Animator m_Animator;

    public float m_RefreshRate = 50.0f;

    // Use this for initialization
    void Start()
    {
        m_Camera = Camera.main;
        m_CameraTransform = Camera.main.transform;

        m_Renderer = GetComponent<SpriteRenderer>();
        m_Animator = GetComponent<Animator>();

        InvokeRepeating("AlignRotation", 0.0f, m_RefreshRate / 1000.0f);
    }

    void OnLookDirectionChanged(LookDirection previous, LookDirection current)
    {
        m_LookDirection = current;

        Debug.Log("Look direction changed to: " + current);

        m_Renderer.flipX = false;

        if (m_LookDirection == LookDirection.Front)
        {
            m_Animator.SetInteger("LookDirection", 0);
        }
        else if (m_LookDirection == LookDirection.FrontRight)
        {
            m_Animator.SetInteger("LookDirection", 1);
            m_Renderer.flipX = true;
        }
        else if (m_LookDirection == LookDirection.Right)
        {
            m_Animator.SetInteger("LookDirection", 2);
            m_Renderer.flipX = true;
        }
        else if (m_LookDirection == LookDirection.BackRight)
        {
            m_Animator.SetInteger("LookDirection", 3);
            m_Renderer.flipX = true;
        }
        else if (m_LookDirection == LookDirection.Back)
        {
            m_Animator.SetInteger("LookDirection", 4);
        }
        else if (m_LookDirection == LookDirection.BackLeft)
        {
            m_Animator.SetInteger("LookDirection", 3);
            //m_Renderer.flipX = true;
        }
        else if (m_LookDirection == LookDirection.Left)
        {
            m_Animator.SetInteger("LookDirection", 2);
            //m_Renderer.flipX = true;
        }
        else if (m_LookDirection == LookDirection.FrontLeft)
        {
            m_Animator.SetInteger("LookDirection", 1);
            //m_Renderer.flipX = true;
        }
        else
        {
            UnityEngine.Assertions.Assert.IsTrue(false, "Invalid LookDirection: " + m_LookDirection);
        }


    }

    // Instead of Update to not drain CPU
    void AlignRotation()
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

        //Debug.Log("Object rotation: " + transform.rotation.eulerAngles);
        //Debug.Log("Camera rotation: " + m_CameraTransform.rotation.eulerAngles);

        float cameraY = m_CameraTransform.rotation.eulerAngles.y;
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

        if (currLook != m_LookDirection)
        {
            OnLookDirectionChanged(m_LookDirection, currLook);
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
