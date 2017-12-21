using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDispatcher : MonoBehaviour 
{
    public TriggerType m_TriggerType = TriggerType.None;

    private List<ITriggerListener> m_Listeners = new List<ITriggerListener>();

    void Start()
    {
        UnityEngine.Assertions.Assert.AreNotEqual(TriggerType.None, m_TriggerType, 
            "Valid Trigger Type must be set");

        foreach (MonoBehaviour mb in GetComponentsInParent<MonoBehaviour>())
        {
            if (mb is ITriggerListener)
            {
                m_Listeners.Add((ITriggerListener)mb);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        foreach (ITriggerListener listener in m_Listeners)
        {
            listener.OnObjectEnteredMyTrigger(other.gameObject, m_TriggerType);
        }
    }

    void OnTriggerExit(Collider other)
    {
        foreach (ITriggerListener listener in m_Listeners)
        {
            listener.OnObjectLeftMyTrigger(other.gameObject, m_TriggerType);
        }
    }
}
