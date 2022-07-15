/*using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using Assets.OpenMM8.Scripts.Gameplay;
using UnityEngine.Experimental.UIElements;
using System.Collections.Generic;

public class TalkableBuilding : Talkable
{
    [Header("Building")]
    public float SoundVolume = 0.6f;
    public AudioClip EnterSound;
    public AudioClip LeaveSound;
    public AudioClip GreetSound;

    private new void Start()
    {
        base.Start();

        IsBuilding = true;
    }

    protected override bool CanInteract(GameObject interacter, RaycastHit interactRay)
    {
        // Has to be a player
        if (interacter.GetComponent<PlayerParty>() == null)
        {
            return false;
        }

        return true;
    }
}
*/