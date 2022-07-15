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
            GameObject.Destroy(StatsUI.Holder);
            GameObject.Destroy(SkillsUI.Holder);
            GameObject.Destroy(Holder);
        }

        // Should only be called when game is running (not paused)
        public void Refresh()
        {
            //Debug.Log("Refresh");

            SetHealth(Owner.GetHealthPercentage());
            SetMana(Owner.GetManaPercentage());

            bool hasBuff = false;
            foreach (SpellEffect playerBuff in Owner.PlayerBuffMap.Values)
            {
                if (playerBuff.IsActive())
                {
                    hasBuff = true;
                    break;
                }
            }
            BlessBuff.enabled = hasBuff;

            // Selection ring
            if (!GameCore.Instance.IsGamePaused())
            {
                if (Owner.IsRecovered() && Owner.CanAct() && Owner.Party.ActiveCharacter == Owner)
                {
                    SelectionRing.enabled = true;
                }
                else
                {
                    SelectionRing.enabled = false;
                }
            }

            if (Owner.IsRecovered() && Owner.CanAct())
            {
                AgroStatus.enabled = true;
            }
            else
            {
                AgroStatus.enabled = false;
            }
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

            //=====================================================================================
            // CharacterUI
            //=====================================================================================
            CharacterUI ui = new CharacterUI();
            ui.Owner = owner;
            OpenMM8Util.AppendResourcesToMap(ui.AvatarSpriteMap,
                "Player/PlayerCharacters/Sprites/PC_" + ((int)owner.CharacterId).ToString());
            OpenMM8Util.AppendResourcesToMap(ui.AvatarSpriteMap, "Player/PlayerCharacters/Sprites/PC_Common");

            GameObject pc = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/UI/PC"), UiMgr.Instance.PartyCanvasHolder.transform);
            pc.transform.localPosition = new Vector3(Constants.PC_WidthDelta, 0.0f, 0.0f) * owner.GetPartyIndex();
            //pc.transform.position += new Vector3(Constants.PC_WidthDelta, 0.0f, 0.0f) * owner.Data.PartyIndex;
            pc.name = "PC_" + owner.Name;

            ui.Holder = pc;
            ui.CharacterAvatarImage = pc.transform.Find("PC_Avatar").GetComponent<Image>();
            ui.SelectionRing = pc.transform.Find("PC_SelectRing").GetComponent<Image>();
            ui.AgroStatus = pc.transform.Find("PC_AgroStatus").GetComponent<Image>();
            ui.HealthBar = pc.transform.Find("PC_HealthBar").GetComponent<Image>();
            ui.ManaBar = pc.transform.Find("PC_ManaBar").GetComponent<Image>();
            ui.BlessBuff = pc.transform.Find("PC_BlessBuff").GetComponent<Image>();
            ui.FaceOverlayAnimation = pc.transform.Find("PC_AvatarAnim").GetComponent<SpriteAnimation>();
            ui.PortraitOverlayButton = pc.transform.Find("PC_PortraitOverlay").GetComponent<Button>();

            owner.UI = ui;

            owner.CharFaceUpdater = new CharFaceUpdater(owner);

            // Doll for char detail UI (Inventory/Stats/Awards/Skills UI, Adventurer's Inn, Character creation page)
            //string dollPrefabName = "DOLL_PC_" + ((int)owner.CharacterId).ToString();

            //=====================================================================================
            // DollUI
            //=====================================================================================
            owner.UI.DollUI = new DollUI();

            string dollPrefabName = "GenericDoll";
            owner.UI.DollUI.Holder = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/UI/Dolls/" + dollPrefabName), UiMgr.Instance.CharDetailUI.CanvasHolder.transform);
            owner.UI.DollUI.Holder.transform.SetSiblingIndex(0);
            owner.UI.DollUI.BackgroundImage = OpenMM8Util.GetComponentAtScenePath<Image>("Background", owner.UI.DollUI.Holder);
            owner.UI.DollUI.BodyImage = OpenMM8Util.GetComponentAtScenePath<Image>("Body", owner.UI.DollUI.Holder);
            owner.UI.DollUI.LH_OpenImage = OpenMM8Util.GetComponentAtScenePath<Image>("LeftHand_Open", owner.UI.DollUI.Holder);
            owner.UI.DollUI.LH_ClosedImage = OpenMM8Util.GetComponentAtScenePath<Image>("LeftHand_Closed", owner.UI.DollUI.Holder);
            owner.UI.DollUI.LH_HoldImage = OpenMM8Util.GetComponentAtScenePath<Image>("LeftHand_Hold", owner.UI.DollUI.Holder);
            owner.UI.DollUI.RH_OpenImage = OpenMM8Util.GetComponentAtScenePath<Image>("RightHand_Open", owner.UI.DollUI.Holder);
            owner.UI.DollUI.RH_HoldImage = OpenMM8Util.GetComponentAtScenePath<Image>("RightHand_Hold", owner.UI.DollUI.Holder);
            owner.UI.DollUI.RH_WeaponAnchorHolder = OpenMM8Util.GetGameObjAtScenePath("RightHand_WeaponHoldAnchor", owner.UI.DollUI.Holder);
            //owner.UI.DollUI.RH_HoldFingersImage = OpenMM8Util.GetComponentAtScenePath<Image>("RightHand_HoldFingers", owner.UI.DollUI.Holder);
            owner.UI.DollUI.Cloak = OpenMM8Util.GetComponentAtScenePath<InventoryItem>("CloakSlot", owner.UI.DollUI.Holder);
            owner.UI.DollUI.Bow = OpenMM8Util.GetComponentAtScenePath<InventoryItem>("BowSlot", owner.UI.DollUI.Holder);
            owner.UI.DollUI.Armor = OpenMM8Util.GetComponentAtScenePath<InventoryItem>("ArmorSlot", owner.UI.DollUI.Holder);
            owner.UI.DollUI.Boots = OpenMM8Util.GetComponentAtScenePath<InventoryItem>("BootsSlot", owner.UI.DollUI.Holder);
            owner.UI.DollUI.Helmet = OpenMM8Util.GetComponentAtScenePath<InventoryItem>("HelmetSlot", owner.UI.DollUI.Holder);
            owner.UI.DollUI.Belt = OpenMM8Util.GetComponentAtScenePath<InventoryItem>("BeltSlot", owner.UI.DollUI.Holder);
            owner.UI.DollUI.RH_Weapon =
                OpenMM8Util.GetComponentAtScenePath<InventoryItem>("RightHand_WeaponHoldAnchor/RightHand_WeaponSlot", owner.UI.DollUI.Holder);

            // Background
            owner.UI.DollUI.BackgroundImage.sprite = UiMgr.Instance.SpriteMap[owner.CharacterData.Background];
            owner.UI.DollUI.BackgroundImage.SetNativeSize();
            // Body
            owner.UI.DollUI.BodyImage.sprite = UiMgr.Instance.SpriteMap[owner.CharacterData.Body];
            owner.UI.DollUI.BodyImage.SetNativeSize();
            owner.UI.DollUI.BodyImage.rectTransform.anchoredPosition = owner.CharacterData.DollBodyPos;
            // LHo (Left Hand Open)
            if (owner.CharacterData.LHo != "none")
            {
                owner.UI.DollUI.LH_OpenImage.sprite = UiMgr.Instance.SpriteMap[owner.CharacterData.LHo];
                owner.UI.DollUI.LH_OpenImage.SetNativeSize();
                owner.UI.DollUI.LH_OpenImage.rectTransform.anchoredPosition = owner.DollTypeData.LH_FingersPos;
            }
            // LHd (Left Hand Closed)
            if (owner.CharacterData.LHd != "none")
            {
                owner.UI.DollUI.LH_ClosedImage.sprite = UiMgr.Instance.SpriteMap[owner.CharacterData.LHd];
                owner.UI.DollUI.LH_ClosedImage.SetNativeSize();
                owner.UI.DollUI.LH_ClosedImage.rectTransform.anchoredPosition = owner.DollTypeData.LH_ClosedPos;
            }
            // LHu (Left Hand Hold)
            if (owner.CharacterData.LHu != "none")
            {
                owner.UI.DollUI.LH_HoldImage.sprite = UiMgr.Instance.SpriteMap[owner.CharacterData.LHu];
                owner.UI.DollUI.LH_HoldImage.SetNativeSize();
                owner.UI.DollUI.LH_HoldImage.rectTransform.anchoredPosition = owner.DollTypeData.LH_OpenPos;
            }
            // RHd (Right Hand Open)
            if (owner.CharacterData.RHd != "none")
            {
                owner.UI.DollUI.RH_OpenImage.sprite = UiMgr.Instance.SpriteMap[owner.CharacterData.RHd];
                owner.UI.DollUI.RH_OpenImage.SetNativeSize();
                owner.UI.DollUI.RH_OpenImage.rectTransform.anchoredPosition = owner.DollTypeData.RH_OpenPos;
            }
            // RHu (Right Hand Closed)
            if (owner.CharacterData.RHu != "none")
            {
                owner.UI.DollUI.RH_HoldImage.sprite = UiMgr.Instance.SpriteMap[owner.CharacterData.RHu];
                owner.UI.DollUI.RH_HoldImage.SetNativeSize();
                owner.UI.DollUI.RH_HoldImage.rectTransform.anchoredPosition = owner.DollTypeData.RH_ClosedPos;
            }

            // Right hand fingers
            if (owner.CharacterData.RHb != "none")
            {
                // Right hand fingers + weapon holder
                Image weaponAnchorImage = owner.UI.DollUI.RH_WeaponAnchorHolder.GetComponent<Image>();
                weaponAnchorImage.sprite = UiMgr.Instance.SpriteMap[owner.CharacterData.RHb];
                weaponAnchorImage.SetNativeSize();
                owner.UI.DollUI.RH_WeaponAnchorHolder.GetComponent<RectTransform>().anchoredPosition =
                    owner.DollTypeData.RH_FingersPos;

                Image holdFingersImage = OpenMM8Util.GetComponentAtScenePath<Image>("RightHand_HoldFingers", owner.UI.DollUI.RH_WeaponAnchorHolder);
                holdFingersImage.sprite = UiMgr.Instance.SpriteMap[owner.CharacterData.RHb];
                holdFingersImage.SetNativeSize();
                holdFingersImage.rectTransform.anchoredPosition.Set(0.0f, 0.0f);
            }

            owner.UI.DollUI.AccessoryBackgroundHolder = OpenMM8Util.GetGameObjAtScenePath("AccessoryBackground", owner.UI.DollUI.Holder);
            owner.UI.DollUI.Ring_1 = OpenMM8Util.GetComponentAtScenePath<InventoryItem>("RingSlot_1", owner.UI.DollUI.AccessoryBackgroundHolder);
            owner.UI.DollUI.Ring_2 = OpenMM8Util.GetComponentAtScenePath<InventoryItem>("RingSlot_2", owner.UI.DollUI.AccessoryBackgroundHolder);
            owner.UI.DollUI.Ring_3 = OpenMM8Util.GetComponentAtScenePath<InventoryItem>("RingSlot_3", owner.UI.DollUI.AccessoryBackgroundHolder);
            owner.UI.DollUI.Ring_4 = OpenMM8Util.GetComponentAtScenePath<InventoryItem>("RingSlot_4", owner.UI.DollUI.AccessoryBackgroundHolder);
            owner.UI.DollUI.Ring_5 = OpenMM8Util.GetComponentAtScenePath<InventoryItem>("RingSlot_5", owner.UI.DollUI.AccessoryBackgroundHolder);
            owner.UI.DollUI.Ring_6 = OpenMM8Util.GetComponentAtScenePath<InventoryItem>("RingSlot_6", owner.UI.DollUI.AccessoryBackgroundHolder);
            owner.UI.DollUI.Gauntlets = OpenMM8Util.GetComponentAtScenePath<InventoryItem>("GauntletSlot", owner.UI.DollUI.AccessoryBackgroundHolder);
            owner.UI.DollUI.Necklace = OpenMM8Util.GetComponentAtScenePath<InventoryItem>("NecklaceSlot", owner.UI.DollUI.AccessoryBackgroundHolder);

            // Fill inventory slots
            owner.EquipSlots.Add(owner.UI.DollUI.Armor);
            owner.EquipSlots.Add(owner.UI.DollUI.Boots);
            owner.EquipSlots.Add(owner.UI.DollUI.Cloak);
            owner.EquipSlots.Add(owner.UI.DollUI.Belt);
            owner.EquipSlots.Add(owner.UI.DollUI.Helmet);
            owner.EquipSlots.Add(owner.UI.DollUI.Bow);
            owner.EquipSlots.Add(owner.UI.DollUI.RH_Weapon);
            //owner.EquipSlots.Add(owner.UI.DollUI.LH_Weapon);
            //owner.EquipSlots.Add(owner.UI.DollUI.Shield);

            owner.EquipSlots.Add(owner.UI.DollUI.Ring_1);
            owner.EquipSlots.Add(owner.UI.DollUI.Ring_2);
            owner.EquipSlots.Add(owner.UI.DollUI.Ring_3);
            owner.EquipSlots.Add(owner.UI.DollUI.Ring_4);
            owner.EquipSlots.Add(owner.UI.DollUI.Ring_5);
            owner.EquipSlots.Add(owner.UI.DollUI.Ring_6);
            owner.EquipSlots.Add(owner.UI.DollUI.Gauntlets);
            owner.EquipSlots.Add(owner.UI.DollUI.Necklace);

            owner.UI.DollUI.Holder.SetActive(false);

            OpenMM8Util.GetComponentAtScenePath<Button>("MagnifyGlass", owner.UI.DollUI.Holder).onClick.AddListener(
                delegate { owner.UI.DollUI.AccessoryBackgroundHolder.SetActive(!owner.UI.DollUI.AccessoryBackgroundHolder.active); });

            // To raycast only non-transparent areas
            // Settings for the texture-import for dolls was also changed because of this
            // Read-Write - enabled
            // Mesh Type - Full Rect
            owner.UI.DollUI.BodyImage.alphaHitTestMinimumThreshold = 0.4f;
            owner.UI.DollUI.LH_OpenImage.alphaHitTestMinimumThreshold = 0.4f;
            owner.UI.DollUI.LH_ClosedImage.alphaHitTestMinimumThreshold = 0.4f;
            owner.UI.DollUI.LH_HoldImage.alphaHitTestMinimumThreshold = 0.4f;
            owner.UI.DollUI.RH_OpenImage.alphaHitTestMinimumThreshold = 0.4f;
            owner.UI.DollUI.RH_HoldImage.alphaHitTestMinimumThreshold = 0.4f;

            //=====================================================================================
            // InventoryUI
            //=====================================================================================
            owner.UI.InventoryUI = InventoryUI.Create(owner);

            //=====================================================================================
            // StatsUI
            //=====================================================================================
            owner.UI.StatsUI = StatsUI.Create(owner);

            //=====================================================================================
            // SkillsUI
            //=====================================================================================
            owner.UI.SkillsUI = SkillsUI.Create(owner);

            return ui;
        }
    }
}
