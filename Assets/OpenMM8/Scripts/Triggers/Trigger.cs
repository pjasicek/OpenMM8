using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

public class Trigger : MonoBehaviour
{
    [SerializeField]
    private TriggerType m_TriggerType = TriggerType.None;

    [SerializeField]
    private PrimitiveType m_ColliderShape = PrimitiveType.Sphere;

    [SerializeField]
    private float m_SideLength = 0.0f;

    [SerializeField]
    private float m_Height = 30.0f;

    void Awake()
    {
        UnityEngine.Assertions.Assert.AreNotEqual(TriggerType.None, m_TriggerType, "Valid Trigger Type must be set");
        UnityEngine.Assertions.Assert.AreNotEqual(m_SideLength, 0.0f, "Trigger has to have a valid size");

        GameObject trigger = new GameObject();
        trigger.name = "Trigger_" + m_TriggerType.ToString();

        switch (m_ColliderShape)
        {
            case PrimitiveType.Sphere:
                var sphere = trigger.AddComponent(typeof(SphereCollider)) as SphereCollider;
                sphere.radius = m_SideLength;
                break;

            case PrimitiveType.Capsule:
                var capsusle = trigger.AddComponent(typeof(CapsuleCollider)) as CapsuleCollider;
                capsusle.radius = m_SideLength;
                capsusle.height = m_Height;
                break;

            case PrimitiveType.Cube:
                var cube = trigger.AddComponent(typeof(BoxCollider)) as BoxCollider;
                cube.size = new Vector3(m_SideLength, m_Height, m_SideLength);
                break;

            default:
                Debug.LogError("Unsupported Collider Shape: " + m_ColliderShape);
                break;
        }
        trigger.GetComponent<Collider>().isTrigger = true;


        TriggerDispatcher td = trigger.AddComponent(typeof(TriggerDispatcher)) as TriggerDispatcher;
        td.m_TriggerType = m_TriggerType;

        trigger.transform.parent = this.transform;

        trigger.gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        trigger.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
    }

    // Use this for initialization
    void Start()
    {
        
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Trigger))]
public class TriggerEditor: Editor
{
    Trigger m_TargetObject;

    public void OnSceneGUI()
    {
        m_TargetObject = this.target as Trigger;

        var type = typeof(Trigger).GetField("m_TriggerType", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(m_TargetObject);
        var radius = typeof(Trigger).GetField("m_SideLength", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(m_TargetObject);
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
#endif