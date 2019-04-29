using Assets.OpenMM8.Scripts.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(SpriteBillboardAnimator))]
public class TestRotator : MonoBehaviour
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

        //Debug.Log("Look direction changed to: " + current);

        SpriteBillboardAnimator.SetLookDirection(LookDir);
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

        // Inspipred by (credit goes to): https://github.com/Interkarma/daggerfall-unity

        Transform parent = transform;
        if (parent == null)
            return;


        Vector3 cameraPosition = Camera.transform.position;
        Vector3 viewDirection = -new Vector3(Camera.transform.forward.x, 0, Camera.transform.forward.z);
        //transform.LookAt(transform.position + viewDirection);


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

        // Hand-tune facing index
        int orientation = 0;

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
            // orientation = 0;
            currLook = LookDirection.Front;
        }
        if (facingAngle < -22.5f && facingAngle > -67.5f)
        {
            // orientation = 1;
            currLook = LookDirection.FrontLeft;
        }   
        if (facingAngle < -67.5f && facingAngle > -112.5f)
        {
            // orientation = 2;
            currLook = LookDirection.Left;
        }   
        if (facingAngle < -112.5f && facingAngle > -157.5f)
        {
            // orientation = 3;
            currLook = LookDirection.BackLeft;
        }   
        if (facingAngle < -157.5f && facingAngle >= -180.0f)
        {
            // orientation = 4;
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
                    //currLook = LookDirection.Front;
                }
            }

            OnLookDirectionChanged(currLook);
        }

        return;




        // This one (mine) was buggy

        /*float cameraY = CameraTransform.rotation.eulerAngles.y;
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
        

       /* if (currLook != LookDir)
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
        }*/

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
