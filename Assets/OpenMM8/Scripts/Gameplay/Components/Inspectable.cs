using UnityEngine;
using System.Collections;
using Assets.OpenMM8.Scripts.Gameplay;

abstract public class Inspectable : MonoBehaviour
{
    abstract public void StartInspect(Character inspector);
    abstract public void EndInspect(Character inspector);
}
