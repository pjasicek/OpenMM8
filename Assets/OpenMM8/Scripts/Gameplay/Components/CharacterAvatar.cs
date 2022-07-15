using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Assets.OpenMM8.Scripts.Gameplay;



public class CharacterAvatar : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        foreach (Character chr in GameCore.Instance.PlayerParty.Characters)
        {
            if (chr.UI.Holder == this.transform.parent.gameObject)
            {
                GameCore.Instance.PlayerParty.SelectCharacter(chr.GetPartyIndex());
                 
                GameEvents.InvokeEvent_OnCharacterAvatarClicked(chr);
            }
        }
    }
}
