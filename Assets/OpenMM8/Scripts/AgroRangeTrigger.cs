using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(CapsuleCollider))]
public class AgroRangeTrigger : MonoBehaviour
{
    public float m_AgroRangeRadius = 30.0f;
    public float m_AgroHeight = 20.0f;

    // Use this for initialization
    void Start()
    {
        GetComponent<CapsuleCollider>().radius = m_AgroRangeRadius;
        GetComponent<CapsuleCollider>().height = m_AgroHeight;
    }

    void OnTriggerEnter(Collider other)
    {
        OpenMM8_IObjectRangeListener listener = GetComponentInParent<OpenMM8_IObjectRangeListener>();
        if (listener != null)
        {
            listener.OnObjectEnteredAgroRange(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        OpenMM8_IObjectRangeListener listener = GetComponentInParent<OpenMM8_IObjectRangeListener>();
        if (listener != null)
        {
            listener.OnObjectLeftAgroRange(other.gameObject);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AgroRangeTrigger))]
public class AgroRangeTrigger_Editor : Editor
{
    AgroRangeTrigger m_TargetObject;

    public void OnSceneGUI()
    {
        m_TargetObject = this.target as AgroRangeTrigger;

        Handles.color = new Color(1.0f, 1.0f, 0, 0.15f);
        Handles.DrawSolidDisc(m_TargetObject.transform.position, Vector3.up, m_TargetObject.m_AgroRangeRadius);
    }
}
#endif