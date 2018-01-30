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

        public Sprite GreenHealthBarSprite;
        public Sprite YellowHealthBarSprite;
        public Sprite RedHealthBarSprite;

        public void SetHealth(float percentage)
        {
            Debug.Assert(EmptySlot.enabled == false);

            if (percentage > 66.6f)
            {
                HealthBar.sprite = GreenHealthBarSprite;
            }
            else if (percentage > 33.3f)
            {
                HealthBar.sprite = YellowHealthBarSprite;
            }
            else
            {
                HealthBar.sprite = RedHealthBarSprite;
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
                AgroStatus.sprite = GameMgr.Instance.GreenAgroStatusSprite;
            }
            else if (state == AgroState.HostileNearby)
            {
                AgroStatus.sprite = GameMgr.Instance.YelloqAgroStatusSprite;
            }
            else
            {
                AgroStatus.sprite = GameMgr.Instance.RedAgroStatusSprite;
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
