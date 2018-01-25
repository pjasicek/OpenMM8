using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    class CharacterUI
    {
        public Image PlayerCharacter;
        public Image HealthBar;
        public Image ManaBar;
        public Image AgroStatus;
        public Image SelectionRing;
        public Image EmptySlot;

        /*PartyMemberUI(int partyMemberIndex)
        {
            switch (partyMemberIndex)
            {
                case 1:
                    break;

                case 2:
                    break;

                case 3:
                    break;

                case 4:
                    break;

                case 5:
                    break;
            }
        }*/

        public void SetHealth(float percentage)
        {
            Debug.Assert(EmptySlot.enabled == true);
        }

        public void SetMana(float percentage)
        {
            Debug.Assert(EmptySlot.enabled == true);
        }

        public void SetAvatarState(PlayerState state)
        {
            Debug.Assert(EmptySlot.enabled == true);
        }

        public void SetAgroStatus(AgroState state)
        {
            Debug.Assert(EmptySlot.enabled == true);
        }

        public void SetSelected(bool isSelected)
        {
            Debug.Assert(EmptySlot.enabled == true);

            SelectionRing.enabled = isSelected;
        }

        public void SetIsOccuppied(bool isOccupied)
        {
            if (isOccupied)
            {
                PlayerCharacter.enabled = false;
                HealthBar.enabled = false;
                ManaBar.enabled = false;
                AgroStatus.enabled = false;
                SelectionRing.enabled = false;
                EmptySlot.enabled = true;
            }
            else
            {
                PlayerCharacter.enabled = true;
                HealthBar.enabled = true;
                ManaBar.enabled = true;
                AgroStatus.enabled = true;
                SelectionRing.enabled = true;
                EmptySlot.enabled = false;
            }
        }

        public bool IsOccuppied()
        {
            return EmptySlot.enabled;
        }
    }
}
