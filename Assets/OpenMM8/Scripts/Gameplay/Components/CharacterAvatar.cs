using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Assets.OpenMM8.Scripts.Gameplay;

public delegate void CharacterAvatarClicked(Character chr);

public class CharacterAvatar : MonoBehaviour, IPointerClickHandler
{
    static public event CharacterAvatarClicked OnCharacterAvatarClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        foreach (Character chr in GameMgr.Instance.PlayerParty.Characters)
        {
            if (chr.UI.Holder == this.transform.parent.gameObject)
            {
                GameMgr.Instance.PlayerParty.SelectCharacter(chr.GetPartyIndex());
                 
                if (OnCharacterAvatarClicked != null)
                {
                    OnCharacterAvatarClicked(chr);
                }
            }
        }
    }
}
