using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class CharacterUI
    {
        public Image PlayerCharacter;
        public Image HealthBar;
        public Image ManaBar;
        public Image AgroStatus;
        public Image SelectionRing;
        public Image EmptySlot;

        static public Sprite HealthBarSprite_Green;
        static public Sprite HealthBarSprite_Yellow;
        static public Sprite HealthBarSprite_Red;

        static public Sprite AgroStatusSprite_Green;
        static public Sprite AgroStatusSprite_Yellow;
        static public Sprite AgroStatusSprite_Red;
        static public Sprite AgroStatusSprite_Gray;

        public void SetHealth(float percentage)
        {
            Debug.Assert(EmptySlot.enabled == false);

            if (percentage > 50.0f)
            {
                HealthBar.sprite = HealthBarSprite_Green;
            }
            else if (percentage > 20.0f)
            {
                HealthBar.sprite = HealthBarSprite_Yellow;
            }
            else
            {
                HealthBar.sprite = HealthBarSprite_Red;
            }

            HealthBar.fillAmount = percentage / 100.0f;
        }

        public void SetMana(float percentage)
        {
            Debug.Assert(EmptySlot.enabled == false);

            ManaBar.fillAmount = percentage / 100.0f;
        }

        public void SetAvatarState(PlayerState state)
        {
            Debug.Assert(EmptySlot.enabled == false);
        }

        public void SetAgroStatus(AgroState state)
        {
            Debug.Assert(EmptySlot.enabled == false);

            if (state == AgroState.Safe)
            {
                AgroStatus.sprite = AgroStatusSprite_Green;
            }
            else if (state == AgroState.HostileNearby)
            {
                AgroStatus.sprite = AgroStatusSprite_Yellow;
            }
            else
            {
                AgroStatus.sprite = AgroStatusSprite_Red;
            }
        }

        public void SetSelected(bool isSelected)
        {
            Debug.Assert(EmptySlot.enabled == false);

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
