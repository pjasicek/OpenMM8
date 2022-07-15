using UnityEngine;

public interface ITriggerListener
{
    void OnObjectEnteredMyTrigger(GameObject other, TriggerType triggerType);
    void OnObjectLeftMyTrigger(GameObject other, TriggerType triggerType);
}
