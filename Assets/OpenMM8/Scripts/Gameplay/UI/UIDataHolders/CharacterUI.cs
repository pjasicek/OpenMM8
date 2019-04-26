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
        public Character Owner;
        public GameObject Holder;

        public Dictionary<string, Sprite> AvatarSpriteMap = new Dictionary<string, Sprite>();

        public DollUI DollUI;
        public StatsUI StatsUI;
        public InventoryUI InventoryUI;
        public SkillsUI SkillsUI;

        public Image CharacterAvatarImage;
        public Image HealthBar;
        public Image ManaBar;
        public Image AgroStatus;
        public Image SelectionRing;
        public Image BlessBuff;
        public Image EmptySlot;
        public SpriteAnimation FaceOverlayAnimation;

        public Button PortraitOverlayButton;

        static public Sprite HealthBarSprite_Green;
        static public Sprite HealthBarSprite_Yellow;
        static public Sprite HealthBarSprite_Red;

        static public Sprite AgroStatusSprite_Green;
        static public Sprite AgroStatusSprite_Yellow;
        static public Sprite AgroStatusSprite_Red;
        static public Sprite AgroStatusSprite_Gray;

        public void Destroy()
        {
            GameObject.Destroy(DollUI.Holder);
            GameObject.Destroy(InventoryUI.Holder);
        }

        public void SetHealth(float percentage)
        {
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
            ManaBar.fillAmount = percentage / 100.0f;
        }

        public void SetAvatarState(PlayerState state)
        {
            Debug.Assert(EmptySlot.enabled == false);
        }

        public void SetAgroStatus(AgroState state)
        {
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
            SelectionRing.enabled = isSelected;
        }

        static public CharacterUI Create(Character owner)
        {
            // TODO: Move all initialization from UiMgr here

            CharacterUI ui = new CharacterUI();
            ui.Owner = owner;

            OpenMM8Util.AppendResourcesToMap(ui.AvatarSpriteMap,
                "Player/PlayerCharacters/Sprites/PC_" + ((int)owner.CharacterId).ToString());
            OpenMM8Util.AppendResourcesToMap(ui.AvatarSpriteMap, "Player/PlayerCharacters/Sprites/PC_Common");

            return ui;
        }
    }
}
