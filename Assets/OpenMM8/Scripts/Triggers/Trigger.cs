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
    private TriggerType TriggerType = TriggerType.None;

    [SerializeField]
    private PrimitiveType ColliderShape = PrimitiveType.Sphere;

    [SerializeField]
    private float SideLength = 0.0f;

    [SerializeField]
    private float Height = 30.0f;

    [SerializeField]
    private string LayerMask;

    private GameObject TriggerRef;

    void Awake()
    {
        UnityEngine.Assertions.Assert.AreNotEqual(TriggerType.None, TriggerType, "Valid Trigger Type must be set");
        UnityEngine.Assertions.Assert.AreNotEqual(SideLength, 0.0f, "Trigger has to have a valid size");

        GameObject trigger = new GameObject();
        trigger.name = "Trigger_" + TriggerType.ToString();

        switch (ColliderShape)
        {
            case PrimitiveType.Sphere:
                var sphere = trigger.AddComponent(typeof(SphereCollider)) as SphereCollider;
                sphere.radius = SideLength;
                break;

            case PrimitiveType.Capsule:
                var capsusle = trigger.AddComponent(typeof(CapsuleCollider)) as CapsuleCollider;
                capsusle.radius = SideLength;
                capsusle.height = Height;
                break;

            case PrimitiveType.Cube:
                var cube = trigger.AddComponent(typeof(BoxCollider)) as BoxCollider;
                cube.size = new Vector3(SideLength, Height, SideLength);
                break;

            default:
                Debug.LogError("Unsupported Collider Shape: " + ColliderShape);
                break;
        }
        trigger.GetComponent<Collider>().isTrigger = true;


        TriggerDispatcher td = trigger.AddComponent(typeof(TriggerDispatcher)) as TriggerDispatcher;
        td.TriggerType = TriggerType;

        trigger.transform.parent = this.transform;

        trigger.gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        trigger.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);    

        trigger.layer = UnityEngine.LayerMask.NameToLayer(LayerMask);
    }

    // Use this for initialization
    void Start()
    {
        
    }
}