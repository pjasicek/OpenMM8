using UnityEngine;
using System.Collections;

abstract public class Interactable : MonoBehaviour
{
    abstract public bool Interact(GameObject interacter, RaycastHit interactRay);
    abstract public bool CanInteract(GameObject interacter, RaycastHit interactRay);
}
