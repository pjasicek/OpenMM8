using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using Assets.OpenMM8.Scripts.Gameplay;
using UnityEngine.Experimental.UIElements;
using System.Collections.Generic;
using System;
using Assets.OpenMM8.Scripts.Data;

public delegate void Talk(Character talkerChr, Talkable talkedToObj);
public delegate void EndTalk(Talkable talkedToObj);

public class Talkable : Interactable
{
    static public event Talk OnTalkStart;
    static public event EndTalk OnTalkEnd;

    [Header("Talkable")]
    public string Location;
    
    public bool IsBuilding = false;
    public GameObject VideoSceneHolder = null;

    [Header("Properties")]

    [SerializeField]
    private int[] NpcIdList = new int[3] { 0, 0, 0 };
    public List<TalkProperties> TalkProperties = new List<TalkProperties>();

    [HideInInspector]
    public AudioSource AudioSource;

    private void Awake()
    {
        InitMgr.OnInitComplete += OnInitComplete;

        if (VideoSceneHolder != null)
        {
            if (VideoSceneHolder.GetComponent<VideoScene>() == null)
            {
                Debug.LogError(name + ": Has VideoSceneHolder but it does not have VideoScene !");
                VideoSceneHolder = null;
            }
        }

        AudioSource = gameObject.AddComponent<AudioSource>();

        TalkEventMgr.OnNpcLeavingLocation += OnNpcLeavingLocation;
    }

    private void OnDestroy()
    {
        TalkEventMgr.OnNpcLeavingLocation -= OnNpcLeavingLocation;
    }

    public void OnInitComplete()
    {
        InitMgr.OnInitComplete -= OnInitComplete;

        // Populate with original game's NPCs
        int idx = 0;
        foreach (int npcId in NpcIdList)
        {
            if (npcId < 1)
            {
                continue;
            }

            TalkProperties npcTalker = TalkEventMgr.Instance.GetNpcTalkProperties(npcId);
            if (npcTalker != null)
            {
                TalkProperties.Insert(idx++, npcTalker);
            }
        }
    }

    public override bool CanInteract(GameObject interacter, RaycastHit interactRay)
    {
        if (!enabled)
        {
            return false;
        }

        HostilityChecker ownerHostilityChecker = GetComponent<HostilityChecker>();
        HostilityChecker interacterHostilityChecker = interacter.GetComponent<HostilityChecker>();
        if ((ownerHostilityChecker != null) && (interacterHostilityChecker != null))
        {
            if (ownerHostilityChecker.IsHostileTo(interacter))
            {
                return false;
            }
        }

        return true;
    }

    public override bool Interact(GameObject interacter, RaycastHit interactRay)
    {
        if (!CanInteract(interacter, interactRay))
        {
            return false;
        }

        PlayerParty playerParty = interacter.GetComponent<PlayerParty>();
        Character talker = playerParty.GetMostRecoveredCharacter();

        if (OnTalkStart != null)
        {
            OnTalkStart(talker, this);
        }

        return true;
    }

    public void OnEndInteract()
    {
        if (OnTalkEnd != null)
        {
            OnTalkEnd(this);
        }
    }

    // =============================== Events ===============================


    private void OnNpcLeavingLocation(TalkProperties talkProp)
    {
        if (TalkProperties.Contains(talkProp))
        {
            TalkProperties.Remove(talkProp);
        }
    }
}
