using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using Assets.OpenMM8.Scripts.Gameplay;
using UnityEngine.Experimental.UIElements;

public delegate void TalkWithNpcDlg(Character talkerChr, Talkable talkedToObj);

public class Talkable : Interactable
{
    static public event TalkWithNpcDlg OnTalkWithNpc;

    public string Name;
    public string Location;
    public string GreetText;
    public Sprite Avatar;

    public override bool Interact(GameObject interacter)
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

        PlayerParty playerParty = interacter.GetComponent<PlayerParty>();
        Character talker = playerParty.GetMostRecoveredCharacter();

        if (OnTalkWithNpc != null)
        {
            OnTalkWithNpc(talker, this);
        }

        return true;
    }
}
