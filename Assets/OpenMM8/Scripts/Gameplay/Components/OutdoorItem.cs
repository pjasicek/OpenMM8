using UnityEngine;
using System.Collections;

using Assets.OpenMM8.Scripts.Gameplay;

public class OutdoorItem : MonoBehaviour
{
    public bool IsMoving = true;
    Rigidbody m_RigidBody = null;
    float m_Elapsed = 0.0f;

    private void Start()
    {
        /*Collider[] cols = GameObject.FindGameObjectWithTag("Player").GetComponents<Collider>();
        foreach (var C in cols)
        {
            Physics.IgnoreCollision(C, GetComponent<Collider>());
        }*/
        m_RigidBody = GetComponent<Rigidbody>();

        GetComponent<MinimapMarker>().Color = Color.blue;
    }

    private void Update()
    {
        if (!IsMoving)
        {
            return;
        }

        m_Elapsed += Time.deltaTime;
        if (m_Elapsed < 5.0f)
        {
            
            return;
        }

        float speed = m_RigidBody.velocity.magnitude;
        if (speed < 0.1)
        {
            m_RigidBody.velocity = new Vector3(0, 0, 0);
            Destroy(m_RigidBody);
            GetComponent<Collider>().isTrigger = true;
            IsMoving = false;
        }
    }
}
