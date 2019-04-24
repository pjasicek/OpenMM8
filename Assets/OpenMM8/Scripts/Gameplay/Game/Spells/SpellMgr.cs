using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class SpellMgr
    {
        // Global state variable - crosshair targeted buffered spell cast waiting for target selection
        static public PlayerSpell PendingPlayerSpell = null;

        // Passed @playerSpell argument at this point needs to have filled:
        // @SpellType
        // @SkillLevel
        // @SkillMastery
        // @Caster
        // @RecoveryTime
        // @RequiredMana (can be 0 - e.g. spells from wands / scrolls)
        //
        // @Flags are filled HERE
        // @Target is either determined right here (whole party) or through the target crosshair
        static public void CastPlayerSpell(PlayerSpell playerSpell)
        {
            if (PendingPlayerSpell != null)
            {
                Debug.LogError("There is existing pending player spell: " + PendingPlayerSpell.SpellType);
                return;
            }

            if (playerSpell.SkillMastery == SkillMastery.None)
            {
                Debug.LogError("PlayerSpell object was not properly filled !");
                return;
            }

            switch (playerSpell.SpellType)
            {
                case SpellType.Spirit_Fate:
                case SpellType.Body_FirstAid:
                    playerSpell.Flags |= CastSpellFlags.TargetCharacter | CastSpellFlags.TargetNpc;
                    break;

                case SpellType.Fire_FireAura:
                case SpellType.Water_RechargeItem:
                case SpellType.Water_EnchantItem:
                case SpellType.Dark_VampiricWeapon:
                    playerSpell.Flags |= CastSpellFlags.ItemEnchantment;
                    break;

                case SpellType.Dark_Reanimate:
                case SpellType.Fire_FireBolt:
                case SpellType.Fire_Fireball:
                case SpellType.Fire_Incinerate:
                case SpellType.Air_LightningBolt:
                case SpellType.Air_Implosion:
                case SpellType.Water_PoisonSpray:
                case SpellType.Water_IceBolt:
                case SpellType.Water_IceBlast:
                case SpellType.Earth_Stun:
                case SpellType.Earth_Slow:
                case SpellType.Earth_DeadlySwarm:
                case SpellType.Earth_Blades:
                case SpellType.Earth_MassDistortion:
                case SpellType.Spirit_SpiritLash:
                case SpellType.Mind_MindBlast:
                case SpellType.Mind_Charm:
                case SpellType.Mind_PsychicShock:
                case SpellType.Body_Harm:
                case SpellType.Body_FlyingFist:
                case SpellType.Light_LightBold:
                case SpellType.Light_DestroyUndead:
                case SpellType.Light_Sunray:
                case SpellType.Dark_ToxicCloud:
                case SpellType.Dark_ShrinkingRay:
                case SpellType.Dark_Sharpmetal:
                case SpellType.Dark_DragonBreath:
                case SpellType.Vampire_Lifedrain:
                case SpellType.DarkElf_DarkfireBolt:
                case SpellType.DarkElf_Blind:
                case SpellType.Dragon_FlameBlast:
                    playerSpell.Flags |= CastSpellFlags.TargetNpc;
                    break;

                case SpellType.Mind_Telepathy:
                case SpellType.Mind_Berserk:
                case SpellType.Mind_Enslave:
                case SpellType.Light_Paralyze:
                case SpellType.Dark_ControlUndead:
                    playerSpell.Flags |= CastSpellFlags.TargetNpc;
                    break;

                case SpellType.Earth_Telekinesis:
                    playerSpell.Flags |= CastSpellFlags.TargetOutdoorItem | 
                        CastSpellFlags.TargetCorpse | 
                        CastSpellFlags.TargetMesh;
                    break;

                case SpellType.Earth_StoneToFlesh:
                case SpellType.Spirit_RemoveCurse:
                case SpellType.Spirit_RaiseDead:
                case SpellType.Spirit_Ressurection:
                case SpellType.Mind_RemoveFear:
                case SpellType.Mind_CureParalysis:
                case SpellType.Mind_CureInsanity:
                case SpellType.Body_CureWeakness:
                case SpellType.Body_Regeneration:
                case SpellType.Body_CurePoison:
                case SpellType.Body_CureDisease:
                    playerSpell.Flags |= CastSpellFlags.TargetCharacter;
                    break;

                case SpellType.Spirit_Bless:
                    if (playerSpell.SkillMastery < SkillMastery.Expert)
                    {
                        playerSpell.Flags |= CastSpellFlags.TargetCharacter;
                    }
                    break;

                case SpellType.Spirit_Preservation:
                    if (playerSpell.SkillMastery < SkillMastery.Master)
                    {
                        playerSpell.Flags |= CastSpellFlags.TargetCharacter;
                    }
                    break;

                case SpellType.Dark_PainReflection:
                    if (playerSpell.SkillMastery < SkillMastery.Master)
                    {
                        playerSpell.Flags |= CastSpellFlags.TargetCharacter;
                    }
                    break;

                case SpellType.Body_Hammerhands:
                    if (playerSpell.SkillMastery < SkillMastery.Grandmaster)
                    {
                        playerSpell.Flags |= CastSpellFlags.TargetCharacter;
                    }
                    break;
            }


            if (playerSpell.Flags.HasFlag(CastSpellFlags.TargetCharacter) ||
                playerSpell.Flags.HasFlag(CastSpellFlags.TargetMesh) ||
                playerSpell.Flags.HasFlag(CastSpellFlags.TargetNpc) ||
                playerSpell.Flags.HasFlag(CastSpellFlags.TargetOutdoorItem) ||
                playerSpell.Flags.HasFlag(CastSpellFlags.TargetCorpse) ||
                playerSpell.Flags.HasFlag(CastSpellFlags.ItemEnchantment))
            {
                playerSpell.DisplayCrosshair = true;
            }

            if (playerSpell.DisplayCrosshair)
            {
                if (playerSpell.Flags.HasFlag(CastSpellFlags.TargetCharacter))
                {
                    GameMgr.Instance.PlayerParty.Characters.ForEach(chr =>
                    {
                        chr.UI.PortraitOverlayButton.image.enabled = true;
                        chr.UI.PortraitOverlayButton.onClick.AddListener(delegate
                        {
                            OnCrosshairClickedOnCharacter(chr);
                        });
                    });
                }
                if (playerSpell.Flags.HasFlag(CastSpellFlags.ItemEnchantment))
                {
                    // Open inventory
                    UiMgr.Instance.HandleButtonDown("Inventory");
                }

                // Set cursor to target crosshair
                Texture2D cursorImage = UiMgr.Instance.TargetCrosshairTexture;
                Vector2 cursorHotspot = new Vector2(cursorImage.width / 2, cursorImage.height / 2);
                Cursor.SetCursor(cursorImage, cursorHotspot, CursorMode.Auto);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                GameMgr.Instance.PauseGame();
                PendingPlayerSpell = playerSpell;
            }
            else
            {
                // Cast the spell immidiately - party buffs / heals, AoE spells
                playerSpell.Caster.CastSpell(playerSpell);
            }
        }

        static public void ClearPlayerSpell()
        {
            if (PendingPlayerSpell != null)
            {
                if (PendingPlayerSpell.DisplayCrosshair)
                {
                    // Restore cursor
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;

                    GameMgr.Instance.PlayerParty.Characters.ForEach(chr =>
                    {
                        chr.UI.PortraitOverlayButton.image.enabled = false;
                        chr.UI.PortraitOverlayButton.onClick.RemoveAllListeners();
                    });

                    UiMgr.Instance.ReturnToGame();
                }

                PendingPlayerSpell = null;
            }
        }

        static public void OnCrosshairClickedOnCharacter(Character clickedCharacter)
        {
            if (PendingPlayerSpell == null)
            {
                Debug.LogError("No pending player spell ???");
                return;
            }

            PendingPlayerSpell.Target = clickedCharacter;
            PendingPlayerSpell.Caster.CastSpell(PendingPlayerSpell);

            ClearPlayerSpell();
        }

        static public void OnCrosshairClickedOnInventoryItem(InventoryItem inventoryItem)
        {
            if (PendingPlayerSpell == null)
            {
                Debug.LogError("No pending player spell ???");
                return;
            }

            PendingPlayerSpell.Target = inventoryItem;
            PendingPlayerSpell.Caster.CastSpell(PendingPlayerSpell);

            ClearPlayerSpell();
        }
    }
}
