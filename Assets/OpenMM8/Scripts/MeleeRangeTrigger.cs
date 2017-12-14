using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(SphereCollider))]
public class MeleeRangeTrigger : MonoBehaviour
{
    public float m_MeleeRangeRadius = 5.0f;

	// Use this for initialization
	void Start ()
    {
        GetComponent<SphereCollider>().radius = m_MeleeRangeRadius;
	}

    void OnTriggerEnter(Collider other)
    {
        OpenMM8_IObjectRangeListener listener = GetComponentInParent<OpenMM8_IObjectRangeListener>();
        if (listener != null)
        {
            listener.OnObjectEnteredMeleeRange(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        OpenMM8_IObjectRangeListener listener = GetComponentInParent<OpenMM8_IObjectRangeListener>();
        if (listener != null)
        {
            listener.OnObjectLeftMeleeRange(other.gameObject);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MeleeRangeTrigger))]
public class MeleeRangeTrigger_Editor : Editor
{
    MeleeRangeTrigger m_TargetObject;

    public void OnSceneGUI()
    {
        m_TargetObject = this.target as MeleeRangeTrigger;

        Handles.color = new Color(1.0f, 0.0f, 0, 0.15f);
        Handles.DrawSolidDisc(m_TargetObject.transform.position, Vector3.up, m_TargetObject.m_MeleeRangeRadius);
    }
}
#endif