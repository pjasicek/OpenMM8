using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(OpenMM8_NPC_AI))]
public class OpenMM8_NPC_AI_Editor : Editor
{
    OpenMM8_NPC_AI m_TargetObject;

	public void OnSceneGUI()
    {
        m_TargetObject = this.target as OpenMM8_NPC_AI;

        Handles.color = new Color(0, 1.0f, 0, 0.1f);
        if (EditorApplication.isPlaying)
        {
            Handles.DrawSolidDisc(m_TargetObject.m_SpawnPosition, Vector3.up, m_TargetObject.m_WanderRadius);
        }
        else
        {
            Handles.DrawSolidDisc(m_TargetObject.transform.position, Vector3.up, m_TargetObject.m_WanderRadius);
        }
    }
}
