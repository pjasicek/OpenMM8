using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface OpenMM8_IObjectRangeListener
{
    void OnObjectEnteredMeleeRange(GameObject other);
    void OnObjectLeftMeleeRange(GameObject other);

    void OnObjectEnteredAgroRange(GameObject other);
    void OnObjectLeftAgroRange(GameObject other);
}