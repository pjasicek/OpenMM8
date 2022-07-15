using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDispatcher : MonoBehaviour 
{
    public TriggerType TriggerType = TriggerType.None;

    private List<ITriggerListener> Listeners = new List<ITriggerListener>();

    void Awake()
    {
        
    }

    void Start()
    {
        UnityEngine.Assertions.Assert.AreNotEqual(TriggerType.None, TriggerType,
            "Valid Trigger Type must be set");

        foreach (MonoBehaviour mb in GetComponentsInParent<MonoBehaviour>())
        {
            if (mb is ITriggerListener)
            {
                Listeners.Add((ITriggerListener)mb);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        foreach (ITriggerListener listener in Listeners)
        {
            listener.OnObjectEnteredMyTrigger(other.gameObject, TriggerType);
        }
    }

    void OnTriggerExit(Collider other)
    {
        foreach (ITriggerListener listener in Listeners)
        {
            listener.OnObjectLeftMyTrigger(other.gameObject, TriggerType);
        }
    }
}
