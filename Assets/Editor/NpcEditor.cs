using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BaseNpc))]
[CanEditMultipleObjects]
public class BaseNpcEditor : Editor
{
    BaseNpc m_TargetObject;

    public void OnSceneGUI()
    {
        m_TargetObject = this.target as BaseNpc;

        Handles.color = new Color(0, 1.0f, 0, 0.1f);
        if (EditorApplication.isPlaying)
        {
            Handles.DrawSolidDisc(m_TargetObject.SpawnPosition, Vector3.up, m_TargetObject.WanderRadius);
        }
        else
        {
            Handles.DrawSolidDisc(m_TargetObject.transform.position, Vector3.up, m_TargetObject.WanderRadius);
        }
    }
}

[CustomEditor(typeof(CombatNpc))]
[CanEditMultipleObjects]
public class CombatNpcEditor : BaseNpcEditor
{

}

[CustomEditor(typeof(VillagerNpc))]
[CanEditMultipleObjects]
public class VillagerNpcEditor : BaseNpcEditor
{

}