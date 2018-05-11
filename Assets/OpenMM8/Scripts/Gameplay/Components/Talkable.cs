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
    }

    public void OnInitComplete()
    {
        InitMgr.OnInitComplete -= OnInitComplete;

        /*
         * Check TalkProperties are custom or used from MM8's original data
         *    - If by data, load all corresponding fields by data
         */
        foreach (TalkProperties talkProp in TalkProperties)
        {
            if (talkProp.NpcId > 0)
            {
                // Load TalkProperties here
                NpcTalkData talkData = DbMgr.Instance.NpcTalkDb.Get(talkProp.NpcId);
                if (talkData == null)
                {
                    Debug.LogError("Talk data for id: " + talkProp.NpcId + " is  null");
                    continue;
                }

                talkProp.Name = talkData.Name;
                talkProp.GreetId = talkData.GreetId;
                talkProp.Avatar = UiMgr.Instance.GetNpcAvatarSprite(talkData.PictureId);
                talkProp.TopicIds.Clear();
                foreach (int topicId in talkData.TopicList)
                {
                    talkProp.TopicIds.Add(topicId);
                }
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
}
