using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Test : MonoBehaviour
{

    Animator animator;

    float tillNextChange = 0.0f;
    float interval = 1.0f;

    public float angle;

    public void PrintEvent(string s)
    {
        Debug.Log("PrintEvent: " + s + " called at: " + Time.time);
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        
    }

    void Update()
    {
        //Debug.Log("Time.time: " + Time.time + ", tillNext: " + tillNextChange);
        /*if (Time.time > tillNextChange)
        {
            int frontId = Animator.StringToHash("Anim_Walk_Front");
            Debug.Log("Change, front ID: " + frontId);
            if (animator.GetCurrentAnimatorStateInfo(0).nameHash == frontId)
            {
                animator.SetInteger("State", 1);
            }
            else
            {
                animator.SetInteger("State", 0);
            }

            tillNextChange = Time.time + interval;
        }*/

        if (Input.GetKey(KeyCode.Q))
        {
            animator.SetInteger("State", 0);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            animator.SetInteger("State", 1);
        }

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            float currAngle = Vector3.Angle(agent.velocity.normalized, this.transform.forward);
            if (agent.velocity.normalized.x < this.transform.forward.x)
            {
                currAngle *= -1;
            }
            currAngle = (currAngle + 180.0f) % 360.0f;

            
            if (!(Mathf.Abs(currAngle - 270.0f) < float.Epsilon || Mathf.Abs(currAngle - 90.0f) < float.Epsilon))
            {
                angle = currAngle;
            }
        }
        

        //Debug.Log("Camera parent name: " + Camera.main.)
        /*Debug.Log("Camera Rot Y: " + Camera.main.transform.parent.transform.localEulerAngles.y);
        Debug.Log("This Rot Y: " + transform.parent.transform.localEulerAngles.y);
        Debug.Log("This parent's name: " + transform.parent.name);*/
    }
}
