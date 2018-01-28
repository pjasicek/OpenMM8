using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(Trigger))]
[CanEditMultipleObjects]
public class TriggerEditor : Editor
{
    Trigger m_TargetObject;

    public void OnSceneGUI()
    {
        m_TargetObject = this.target as Trigger;

        var type = typeof(Trigger).GetField("TriggerType", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(m_TargetObject);
        var radius = typeof(Trigger).GetField("SideLength", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(m_TargetObject);
        if ((TriggerType)type == TriggerType.MeleeRange)
        {
            Handles.color = new Color(1.0f, 0.0f, 0, 0.15f);
            Handles.DrawSolidDisc(m_TargetObject.transform.position, Vector3.up, (float)radius);
        }
        else if ((TriggerType)type == TriggerType.AgroRange)
        {
            Handles.color = new Color(1.0f, 1.0f, 0, 0.15f);
            Handles.DrawSolidDisc(m_TargetObject.transform.position, Vector3.up, (float)radius);
        }
    }
}