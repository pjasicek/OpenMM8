using Assets.OpenMM8.Scripts.Gameplay.Data;
using Assets.OpenMM8.Scripts.Gameplay.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class SpellCastHelper
    {
        // Global state variable - crosshair targeted buffered spell cast waiting for target selection
        static public PlayerSpell PendingPlayerSpell = null;

        // This method handles picking the right spell target for player's spell - e.g. displays Crosshair if applicable
        //     Actual spell cast is done in @ExecutePlayerSpellCast
        //
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
        static public void PushPlayerSpellCast(PlayerSpell playerSpell)
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
                case SpellType.Light_LightBolt:
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
                    GameCore.Instance.PlayerParty.Characters.ForEach(chr =>
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
                UiMgr.Instance.CrosshairImage.enabled = false;

                GameCore.Instance.PauseGame();
                PendingPlayerSpell = playerSpell;
            }
            else
            {
                // Cast the spell immidiately - party buffs / heals, AoE spells
                ExecutePlayerSpellCast(playerSpell);
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
                    UiMgr.Instance.CrosshairImage.enabled = true;

                    GameCore.Instance.PlayerParty.Characters.ForEach(chr =>
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
            ExecutePlayerSpellCast(PendingPlayerSpell);

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
            ExecutePlayerSpellCast(PendingPlayerSpell);

            ClearPlayerSpell();
        }

        static public void OnCrosshairClickedOnMonster(Monster monster)
        {
            if (PendingPlayerSpell == null)
            {
                Debug.LogError("No pending player spell ???");
                return;
            }

            PendingPlayerSpell.Target = monster;
            ExecutePlayerSpellCast(PendingPlayerSpell);

            ClearPlayerSpell();
        }

        // This one is generic
        static public void OnCrosshairTargetClicked(object target)
        {
            PendingPlayerSpell.Target = target;
            ExecutePlayerSpellCast(PendingPlayerSpell);

            ClearPlayerSpell();
        }

        static public void ExecutePlayerSpellCast(PlayerSpell spell)
        {
            /*if (spell.Caster.GetMaxSpellPoints() == 0)
            {
                GameCore.SetStatusBarText(spell.Caster.Class.ToString() + " cannot cast spells !");
                SoundMgr.PlaySoundById(SoundType.SpellFail);
                return;
            }*/

            if (spell.Caster.CurrSpellPoints < spell.RequiredMana)
            {
                Debug.Log("Not enough mana: " + spell.Caster.CurrSpellPoints + " (Required " + spell.RequiredMana + ")");
                GameCore.SetStatusBarText("Not enough mana !");
                SoundMgr.PlaySoundById(SoundType.SpellFail);
                return;
            }

            // Handle curse - chance to fail the spell

            SpellType spellType = spell.SpellType;
            SkillMastery skillMastery = spell.SkillMastery;
            int skillLevel = spell.SkillLevel;
            SpellData spellData = DbMgr.Instance.SpellDataDb.Get(spellType);
            PlayerParty Party = GameCore.GetParty();

            Character targetCharacter = spell.Target as Character;
            InventoryItem targetInventoryItem = spell.Target as InventoryItem;
            Monster targetMonster = spell.Target as Monster;

            // Prepare projectile, even though it is not used all the time, it saves a lot of boilerplate code
            ProjectileInfo projectileInfo = new ProjectileInfo();
            projectileInfo.Shooter = spell.Caster;
            projectileInfo.ShooterTransform = Party.transform;
            if (targetMonster != null)
            {
                projectileInfo.TargetPosition = targetMonster.transform.position;
            }
            else
            {
                projectileInfo.TargetPosition = UiMgr.GetCrosshairRay().GetPoint(100.0f);
            }
            projectileInfo.SpellType = spellType;
            projectileInfo.SkillMastery = skillMastery;
            projectileInfo.SkillLevel = skillLevel;

            if (DbMgr.Instance.ObjectDisplayDb.Data.ContainsKey(spellData.DisplayObjectId))
            {
                projectileInfo.DisplayData = DbMgr.Instance.ObjectDisplayDb.Get(spellData.DisplayObjectId);
                //projectileInfo.ImpactObject = DbMgr.Instance.ObjectDisplayDb.Get(spellData.ImpactDisplayObjectId);
            }

            int duration = 0;
            int power = 0;

            switch (spellType)
            {
                case SpellType.None:
                    Debug.LogError("Spell not implemented: " + spellType);
                    return;

                case SpellType.Fire_TorchLight:
                    duration = skillLevel * 60;
                    switch (skillMastery)
                    {
                        case SkillMastery.Normal:
                            power = 2;
                            break;
                        case SkillMastery.Expert:
                            power = 3;
                            break;
                        case SkillMastery.Master:
                        case SkillMastery.Grandmaster:
                            power = 4;
                            break;
                    }

                    Party.PartyBuffMap[PartyEffectType.Torchlight].Apply(
                        skillMastery,
                        power,
                        GameTime.FromCurrentTime(60 * duration),
                        spell.Caster);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Fire_FireBolt:
                    // TODO: Special projectile ?
                    break;

                case SpellType.Fire_FireAura:
                    duration = 60 * skillLevel;
                    SpecialEnchantType enchantType = SpecialEnchantType.None;
                    switch (skillMastery)
                    {
                        case SkillMastery.Normal:
                            enchantType = SpecialEnchantType.OfFire;
                            break;
                        case SkillMastery.Expert:
                            enchantType = SpecialEnchantType.OfFlame;
                            break;
                        case SkillMastery.Master:
                            enchantType = SpecialEnchantType.OfInfernos;
                            break;
                        case SkillMastery.Grandmaster:
                            enchantType = SpecialEnchantType.OfInfernos;
                            duration = 0; // Permanent
                            break;
                    }

                    Assert.IsTrue(targetInventoryItem != null);

                    Item targetItem = targetInventoryItem.Item;
                    bool canApplyFireAura = targetItem.Enchant == null &&
                        (targetItem.Data.ItemType == ItemType.WeaponOneHanded ||
                         targetItem.Data.ItemType == ItemType.WeaponTwoHanded ||
                         targetItem.Data.ItemType == ItemType.Missile);
                    if (canApplyFireAura)
                    {
                        // TODO: Effect should be permanent only with Grandmaster
                        ItemGenerator.Instance.ApplySpecialEnchant(enchantType, targetItem);
                        SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    }
                    else
                    {
                        //UiMgr.Instance.SetPartyInfoText("Spell failed");
                        SoundMgr.PlaySoundById((int)SoundType.SpellFail);
                    }

                    break;

                case SpellType.Fire_Haste:
                    switch (skillMastery)
                    {
                        case SkillMastery.Normal:
                        case SkillMastery.Expert:
                            duration = 60 + (skillLevel * 1);
                            break;
                        case SkillMastery.Master:
                            duration = 60 + (skillLevel * 3);
                            break;
                        case SkillMastery.Grandmaster:
                            duration = 60 + (skillLevel * 4);
                            break;
                    }

                    bool wasAppliedAtleastOnce = false;
                    Party.Characters.ForEach(chr =>
                    {
                        // TODO: Check if this character has Weak condition
                        wasAppliedAtleastOnce = true;
                        SpellFxRenderer.SetPlayerBuffAnim(spellType, chr);
                    });

                    if (wasAppliedAtleastOnce)
                    {
                        // In original game if it was applied atleast once it affected the entire party
                        // But that does make 0 sense to me
                        Party.PartyBuffMap[PartyEffectType.Haste].Apply(
                            skillMastery,
                            skillLevel,
                            GameTime.FromCurrentTime(60 * duration),
                            spell.Caster);

                        SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    }

                    break;

                case SpellType.Fire_Fireball:
                    Projectile.Spawn(projectileInfo);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Fire_FireSpike:
                    // TODO: Should just drop on ground like landmine
                    break;

                case SpellType.Fire_Immolation:
                    switch (skillMastery)
                    {
                        case SkillMastery.Normal:
                        case SkillMastery.Expert:
                        case SkillMastery.Master:
                            duration = skillLevel;
                            break;

                        case SkillMastery.Grandmaster:
                            duration = 10 * skillLevel;
                            break;
                    }

                    Party.Characters.ForEach(chr =>
                    {
                        SpellFxRenderer.SetPlayerBuffAnim(spellType, chr);
                    });
                    Party.PartyBuffMap[PartyEffectType.Immolation].Apply(skillMastery,
                        skillLevel,
                        GameTime.FromCurrentTime(60 * duration),
                        spell.Caster);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Fire_MeteorShower:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Fire_Inferno:
                    /*Ray ray = UiMgr.GetCrosshairRay();
                    Transform partyTransform = GameCore.GetParty().transform;
                    GameObject arrow = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/PlaceholderProjectile"),
                        ray.origin + (ray.direction * 1), partyTransform.rotation);
                    Projectile projectile = arrow.GetComponent<Projectile>();
                    projectile.AttackInfo = new AttackInfo();
                    projectile.IsTargetPlayer = false;

                    Debug.Log("Origin: " + ray.origin + ", Direction: " + ray.direction);*/
                    //projectile.Shoot(arrow.transform.position, arrow.transform.position + ray.direction * 100.0f);
                    break;

                case SpellType.Fire_Incinerate:
                    Projectile.Spawn(projectileInfo);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Air_WizardEye:
                    duration = 60 * skillLevel;
                    Party.PartyBuffMap[PartyEffectType.WizardEye].Apply(skillMastery,
                        skillLevel,
                        GameTime.FromCurrentTime(60 * duration),
                        spell.Caster);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Air_FeatherFall:
                    switch (skillMastery)
                    {
                        case SkillMastery.Normal:
                            duration = 5 * skillLevel;
                            break;
                        case SkillMastery.Expert:
                            duration = 10 * skillLevel;
                            break;
                        case SkillMastery.Master:
                        case SkillMastery.Grandmaster:
                            duration = 60 * duration;
                            break;
                    }

                    Party.PartyBuffMap[PartyEffectType.FeatherFall].Apply(skillMastery,
                        0,
                        GameTime.FromCurrentTime(60 * duration),
                        spell.Caster);
                    Party.Characters.ForEach(chr =>
                    {
                        SpellFxRenderer.SetPlayerBuffAnim(spellType, chr);
                    });
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Air_Sparks:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Air_Jump:
                    if (Party.IsGrounded())
                    {
                        Party.Controller.IsSpellJumpQueued = true;
                        Party.Controller.SpellJumpVelocity = 25.0f;
                        SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    }
                    else
                    {
                        GameCore.SetStatusBarText("Can't cast Jump while airborne !");
                        SoundMgr.PlaySoundById(SoundType.SpellFail);
                        return;
                    }

                    break;

                case SpellType.Air_Shield:
                    switch (skillMastery)
                    {
                        case SkillMastery.Normal:
                        case SkillMastery.Expert:
                            duration = 60 + 5 * skillLevel;
                            break;
                        case SkillMastery.Master:
                            duration = 60 + 15 * skillLevel;
                            break;
                        case SkillMastery.Grandmaster:
                            duration = 60 + 60 * skillLevel;
                            break;
                    }

                    Party.Characters.ForEach(chr =>
                    {
                        SpellFxRenderer.SetPlayerBuffAnim(spellType, chr);
                    });
                    Party.PartyBuffMap[PartyEffectType.Shield].Apply(skillMastery,
                        skillLevel,
                        GameTime.FromCurrentTime(60 * duration),
                        spell.Caster);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Air_LightningBolt:
                    Projectile.Spawn(projectileInfo);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Air_Invisibility:
                    if (GameCore.Instance.NearbyMonsterList.Count > 0)
                    {
                        GameCore.SetStatusBarText("There are hostile creatures nearby !");
                        return;
                    }

                    switch (skillMastery)
                    {
                        case SkillMastery.Normal:
                            duration = 10 * skillLevel;
                            power = skillLevel;
                            break;
                        case SkillMastery.Expert:
                            duration = 10 * skillLevel;
                            power = 2 * skillLevel;
                            break;
                        case SkillMastery.Master:
                            duration = 10 * skillLevel;
                            power = 3 * skillLevel;
                            break;
                        case SkillMastery.Grandmaster:
                            duration = 60 * skillLevel;
                            power = 4 * skillLevel;
                            break;
                    }

                    Party.Characters.ForEach(chr =>
                    {
                        SpellFxRenderer.SetPlayerBuffAnim(spellType, chr);
                    });
                    Party.PartyBuffMap[PartyEffectType.Invisibility].Apply(skillMastery,
                        power,
                        GameTime.FromCurrentTime(60 * duration),
                        spell.Caster);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Air_Implosion:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Air_Fly:
                    if (GameCore.Instance.MapType == MapType.Indoor)
                    {
                        GameCore.SetStatusBarText("Can't cast Fly indoors !");
                        SoundMgr.PlaySoundById(SoundType.SpellFail);
                        return;
                    }

                    duration = 60 * skillLevel;
                    if (skillMastery == SkillMastery.Grandmaster)
                    {
                        power = 0;
                    }
                    else
                    {
                        power = 1;
                    }

                    Party.PartyBuffMap[PartyEffectType.Fly].Apply(skillMastery,
                        power,
                        GameTime.FromCurrentTime(60 * duration),
                        spell.Caster);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Air_Startburst:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Water_Awaken:
                    switch (skillMastery)
                    {
                        case SkillMastery.Normal:
                            duration = 3 * skillLevel;
                            break;
                        case SkillMastery.Expert:
                            duration = 60 * skillLevel;
                            break;
                        case SkillMastery.Master:
                            duration = 1440 * skillLevel;
                            break;
                        case SkillMastery.Grandmaster:
                            duration = 0;
                            break;
                    }

                    Party.Characters.ForEach(chr =>
                    {
                        if (duration == 0)
                        {
                            if (chr.Conditions[Condition.Sleep].IsValid())
                            {
                                chr.Conditions[Condition.Sleep].Reset();
                                chr.PlayEventReaction(CharacterReaction.WokeUp);
                            }
                        }
                        else
                        {
                            if (chr.DiscardConditionIfLastsLessThan(Condition.Sleep, GameTime.FromSeconds(duration)))
                            {
                                chr.PlayEventReaction(CharacterReaction.WokeUp);
                            }
                        }
                    });

                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Water_PoisonSpray:
                    Projectile.Spawn(projectileInfo);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Water_IceBolt:
                    Projectile.Spawn(projectileInfo);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Water_WaterWalk:
                    switch (skillMastery)
                    {
                        case SkillMastery.Normal:
                        case SkillMastery.Expert:
                            duration = 10 * skillLevel;
                            break;
                        case SkillMastery.Master:
                        case SkillMastery.Grandmaster:
                            duration = 60 * skillLevel;
                            break;
                    }

                    Party.Characters.ForEach(chr =>
                    {
                        SpellFxRenderer.SetPlayerBuffAnim(spellType, chr);
                    });
                    Party.PartyBuffMap[PartyEffectType.WaterWalk].Apply(skillMastery,
                        skillLevel,
                        GameTime.FromCurrentTime(60 * duration),
                        spell.Caster);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Water_RechargeItem:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Water_AcidBurst:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Water_EnchantItem:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Water_TownPortal:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Water_IceBlast:
                    Projectile.Spawn(projectileInfo);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Water_LloydsBeacon:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Earth_Stun:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Earth_Slow:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Earth_DeadlySwarm:
                    Projectile.Spawn(projectileInfo);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Earth_Stoneskin:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Earth_Blades:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Earth_StoneToFlesh:
                    switch (skillMastery)
                    {
                        case SkillMastery.Normal:
                        case SkillMastery.Expert:
                            duration = 60 * skillLevel;
                            break;
                        case SkillMastery.Master:
                            duration = 1440 * skillLevel;
                            break;
                        case SkillMastery.Grandmaster:
                            duration = 0;
                            break;
                    }

                    if (duration == 0)
                    {
                        if (targetCharacter.Conditions[Condition.Petrified].IsValid())
                        {
                            targetCharacter.Conditions[Condition.Petrified].Reset();
                        }
                    }
                    else
                    {
                        targetCharacter.DiscardConditionIfLastsLessThan(Condition.Sleep, GameTime.FromSeconds(duration));
                    }

                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Earth_RockBlast:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Earth_Telekinesis:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Earth_DeathBlossom:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Earth_MassDistortion:
                    Projectile.Spawn(projectileInfo);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Spirit_DetectLife:
                    switch (skillMastery)
                    {
                        case SkillMastery.Normal:
                            duration = 10 * skillLevel;
                            break;
                        case SkillMastery.Expert:
                            duration = 30 * skillLevel;
                            break;
                        case SkillMastery.Master:
                        case SkillMastery.Grandmaster:
                            duration = 60 * skillLevel;
                            break;
                        default:
                            break;
                    }

                    Party.Characters.ForEach(chr =>
                    {
                        SpellFxRenderer.SetPlayerBuffAnim(spellType, chr);
                    });
                    Party.PartyBuffMap[PartyEffectType.DetectLife].Apply(skillMastery,
                        skillLevel,
                        GameTime.FromCurrentTime(60 * duration),
                        spell.Caster);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Spirit_Bless:
                    switch (skillMastery)
                    {
                        case SkillMastery.Normal:
                        case SkillMastery.Expert:
                            duration = 60 + 5 * skillLevel;
                            break;
                        case SkillMastery.Master:
                            duration = 60 + 15 * skillLevel;
                            break;
                        case SkillMastery.Grandmaster:
                            duration = 60 + 60 * skillLevel;
                            break;
                    }

                    bool castOnEntireParty = skillMastery >= SkillMastery.Expert;
                    if (castOnEntireParty)
                    {
                        Party.Characters.ForEach(chr =>
                        {
                            chr.PlayerBuffMap[PlayerEffectType.Bless].Apply(
                                skillMastery,
                                skillLevel,
                                GameTime.FromCurrentTime(60 * duration),
                                spell.Caster);
                            SpellFxRenderer.SetPlayerBuffAnim(spellType, chr);
                        });
                    }
                    else
                    {
                        Assert.IsTrue(targetCharacter != null);

                        targetCharacter.PlayerBuffMap[PlayerEffectType.Bless].Apply(
                            skillMastery,
                            skillLevel,
                            GameTime.FromCurrentTime(60 * duration),
                            spell.Caster);
                        SpellFxRenderer.SetPlayerBuffAnim(spellType, targetCharacter);
                    }

                    SoundMgr.PlaySoundById(spellData.EffectSoundId);

                    break;

                case SpellType.Spirit_Fate:
                    switch (skillMastery)
                    {
                        case SkillMastery.Normal:
                            power = skillLevel;
                            break;
                        case SkillMastery.Expert:
                            power = skillLevel * 2;
                            break;
                        case SkillMastery.Master:
                            power = skillLevel * 4;
                            break;
                        case SkillMastery.Grandmaster:
                            power = skillLevel * 6;
                            break;
                    }
                    duration = 5; // 5 minutes or until character / npc attacks

                    Assert.IsTrue(targetMonster != null || targetCharacter != null);
                    if (targetCharacter != null)
                    {
                        targetCharacter.PlayerBuffMap[PlayerEffectType.Bless].Apply(
                                skillMastery,
                                power,
                                GameTime.FromCurrentTime(60 * duration),
                                spell.Caster);
                        SpellFxRenderer.SetPlayerBuffAnim(spellType, targetCharacter);
                    }
                    else if (targetMonster != null)
                    {
                        targetMonster.BuffMap[MonsterBuffType.Fate].Apply(skillMastery,
                            power,
                            GameTime.FromCurrentTime(60 * duration),
                            spell.Caster);

                        // WHY WOULD THIS TURN THE MONSTER TO AGGRSSION ?!?!?!?
                        //targetMonster.Flags |= MonsterFlags.Aggressor;
                    }

                    SoundMgr.PlaySoundById(spellData.EffectSoundId);

                    break;

                case SpellType.Spirit_TurnUndead:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Spirit_RemoveCurse:
                    switch (skillMastery)
                    {
                        case SkillMastery.Normal:
                        case SkillMastery.Expert:
                            duration = 60 * skillLevel;
                            break;
                        case SkillMastery.Master:
                            duration = 1440 * skillLevel;
                            break;
                        case SkillMastery.Grandmaster:
                            duration = 0;
                            break;
                    }

                    bool wasCurseRemoved = false;
                    if (skillMastery == SkillMastery.Grandmaster)
                    {
                        if (targetCharacter.Conditions[Condition.Cursed].IsValid())
                        {
                            targetCharacter.Conditions[Condition.Cursed].Reset();
                            wasCurseRemoved = true;
                        }
                    }
                    else
                    {
                        if (targetCharacter.DiscardConditionIfLastsLessThan(Condition.Cursed, GameTime.FromSeconds(duration)))
                        {
                            wasCurseRemoved = true;
                        }
                    }

                    if (wasCurseRemoved)
                    {
                        SpellFxRenderer.SetPlayerBuffAnim(spellType, targetCharacter);
                    }
                    
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Spirit_Preservation:
                    switch (skillMastery)
                    {
                        case SkillMastery.Normal:
                        case SkillMastery.Expert:
                        case SkillMastery.Master:
                            duration = 60 + 5 * skillLevel;
                            break;
                        case SkillMastery.Grandmaster:
                            duration = 60 + 15 * skillLevel;
                            break;
                    }

                    castOnEntireParty = skillMastery >= SkillMastery.Master;
                    if (castOnEntireParty)
                    {
                        Party.Characters.ForEach(chr =>
                        {
                            chr.PlayerBuffMap[PlayerEffectType.Preservation].Apply(
                                skillMastery,
                                skillLevel,
                                GameTime.FromCurrentTime(60 * duration),
                                spell.Caster);
                            SpellFxRenderer.SetPlayerBuffAnim(spellType, chr);
                        });
                    }
                    else
                    {
                        Assert.IsTrue(targetCharacter != null);

                        targetCharacter.PlayerBuffMap[PlayerEffectType.Preservation].Apply(
                            skillMastery,
                            skillLevel,
                            GameTime.FromCurrentTime(60 * duration),
                            spell.Caster);
                        SpellFxRenderer.SetPlayerBuffAnim(spellType, targetCharacter);
                    }

                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Spirit_Heroism:
                    power = 5 + skillLevel;
                    switch (skillMastery)
                    {
                        case SkillMastery.Normal:
                            duration = 60 + (skillLevel * 5);
                            break;

                        case SkillMastery.Expert:
                            duration = 60 + (skillLevel * 5);
                            break;

                        case SkillMastery.Master:
                            duration = 60 + (skillLevel * 15);
                            break;

                        case SkillMastery.Grandmaster:
                            duration = 60 + (skillLevel * 60);
                            break;
                    }

                    Party.PartyBuffMap[PartyEffectType.Heroism].Apply(skillMastery, power, GameTime.FromCurrentTime(60 * duration), spell.Caster);
                    Party.Characters.ForEach(chr => SpellFxRenderer.SetPlayerBuffAnim(spellType, chr));
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    //TimeUntilRecovery = recoveryTime / 100.0f;
                    break;

                case SpellType.Spirit_SpiritLash:
                    Projectile.Spawn(projectileInfo);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;
                    
                case SpellType.Spirit_Ressurection:
                    switch (skillMastery)
                    {
                        case SkillMastery.Normal:
                            duration = 3 * skillLevel;
                            break;
                        case SkillMastery.Expert:
                            duration = 180 * skillLevel;
                            break;
                        case SkillMastery.Master:
                            duration = 3 * 1440 * skillLevel;
                            break;
                    }

                    if (targetCharacter.IsDead())
                    {
                        if (skillMastery == SkillMastery.Grandmaster ||
                            targetCharacter.DiscardConditionIfLastsLessThan(Condition.Dead, 
                                GameTime.FromSeconds(60 * duration)))
                        {
                            targetCharacter.CurrHitPoints = (int)(0.15f * targetCharacter.GetMaxHitPoints());
                            targetCharacter.Conditions[Condition.Dead].Reset();
                            targetCharacter.Conditions[Condition.Unconcious].Reset();
                            targetCharacter.Conditions[Condition.Weak] = GameTime.FromCurrentTime(0);
                        }
                    }
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Spirit_SharedLife:
                    if (skillMastery == SkillMastery.Grandmaster)
                    {
                        power = 4;
                    }
                    else
                    {
                        power = 3;
                    }

                    int sharedLifePool = power * skillLevel;

                    List<Character> eligibleCharacters = new List<Character>();
                    Party.Characters.ForEach(chr =>
                    {
                        if (!chr.IsDead() && chr.IsEradicated() && !chr.IsPetrified())
                        {
                            eligibleCharacters.Add(chr);
                            sharedLifePool += chr.CurrHitPoints;
                        }
                    });

                    int meanLife = sharedLifePool / eligibleCharacters.Count;
                    eligibleCharacters.ForEach(chr =>
                    {
                        chr.CurrHitPoints = meanLife;
                        if (chr.CurrHitPoints > chr.GetMaxHitPoints())
                        {
                            chr.CurrHitPoints = chr.GetMaxHitPoints();
                        }
                        if (chr.CurrHitPoints > 0)
                        {
                            chr.RemoveCondition(Condition.Unconcious);
                        }
                        SpellFxRenderer.SetPlayerBuffAnim(spellType, chr);
                    });

                    SoundMgr.PlaySoundById(spellData.EffectSoundId);

                    break;
                    
                case SpellType.Spirit_RaiseDead:
                    if (skillMastery < SkillMastery.Grandmaster)
                    {
                        duration = 1440 * skillLevel;
                    }

                    if (targetCharacter.IsEradicated() || targetCharacter.IsDead())
                    {
                        if (skillMastery == SkillMastery.Grandmaster ||
                            targetCharacter.DiscardConditionIfLastsLessThan(Condition.Eradicated,
                                GameTime.FromSeconds(60 * duration)) ||
                            targetCharacter.DiscardConditionIfLastsLessThan(Condition.Dead,
                                GameTime.FromSeconds(60 * duration)))
                        {
                            targetCharacter.CurrHitPoints = (int)(0.15f * targetCharacter.GetMaxHitPoints());
                            targetCharacter.Conditions[Condition.Eradicated].Reset();
                            targetCharacter.Conditions[Condition.Dead].Reset();
                            targetCharacter.Conditions[Condition.Unconcious].Reset();
                            targetCharacter.Conditions[Condition.Weak] = GameTime.FromCurrentTime(0);
                        }
                    }

                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Mind_Telepathy:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Mind_RemoveFear:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Mind_MindBlast:
                    Projectile.Spawn(projectileInfo);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Mind_Charm:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Mind_CureParalysis:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Mind_Berserk:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Mind_MassFear:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Mind_CureInsanity:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Mind_PsychicShock:
                    Projectile.Spawn(projectileInfo);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Mind_Enslave:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Body_CureWeakness:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Body_FirstAid:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Body_Harm:
                    Projectile.Spawn(projectileInfo);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Body_Regeneration:
                    switch (skillMastery)
                    {
                        case SkillMastery.Normal:
                            power = 1;
                            break;
                        case SkillMastery.Expert:
                            power = 1;
                            break;
                        case SkillMastery.Master:
                            power = 3;
                            break;
                        case SkillMastery.Grandmaster:
                            power = 10;
                            break;
                    }
                    duration = 60 * skillLevel;

                    Assert.IsTrue(targetCharacter != null);

                    targetCharacter.PlayerBuffMap[PlayerEffectType.Regeneration].Apply(
                        skillMastery,
                        power,
                        GameTime.FromCurrentTime(60 * duration),
                        spell.Caster);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    SpellFxRenderer.SetPlayerBuffAnim(spellType, targetCharacter);

                    break;

                case SpellType.Fire_ProtectionFromFire:
                case SpellType.Air_ProtectionFromAir:
                case SpellType.Water_ProtectionFromWater:
                case SpellType.Earth_ProtectionFromEarth:
                case SpellType.Mind_ProtectionFromMind:
                case SpellType.Body_ProtectionFromBody:
                    PartyEffectType partyEffectType = PartyEffectType.Torchlight;
                    switch (spell.SpellType)
                    {
                        case SpellType.Fire_ProtectionFromFire:
                            partyEffectType = PartyEffectType.ResistFire;
                            break;
                        case SpellType.Air_ProtectionFromAir:
                            partyEffectType = PartyEffectType.ResistAir;
                            break;
                        case SpellType.Water_ProtectionFromWater:
                            partyEffectType = PartyEffectType.ResistWater;
                            break;
                        case SpellType.Earth_ProtectionFromEarth:
                            partyEffectType = PartyEffectType.ResistEarth;
                            break;
                        case SpellType.Mind_ProtectionFromMind:
                            partyEffectType = PartyEffectType.ResistMind;
                            break;
                        case SpellType.Body_ProtectionFromBody:
                            partyEffectType = PartyEffectType.ResistBody;
                            break;
                    }

                    switch (skillMastery)
                    {
                        case SkillMastery.Normal:
                        case SkillMastery.Expert:
                        case SkillMastery.Master:
                        case SkillMastery.Grandmaster:
                            power = (int)skillMastery * skillLevel;
                            break;
                    }
                    duration = 60 * skillLevel;

                    Party.PartyBuffMap[partyEffectType].Apply(
                        skillMastery,
                        power,
                        GameTime.FromCurrentTime(60 * duration),
                        spell.Caster);

                    Party.Characters.ForEach(chr =>
                    {
                        SpellFxRenderer.SetPlayerBuffAnim(spellType, chr);
                    });
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);

                    break;

                case SpellType.Body_CurePoison:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Body_Hammerhands:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Body_CureDisease:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Body_ProtectionFromMagic:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Body_FlyingFist:
                    Projectile.Spawn(projectileInfo);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Body_PowerCure:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Light_LightBolt:
                    Projectile.Spawn(projectileInfo);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Light_DestroyUndead:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Light_DispelMagic:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Light_Paralyze:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Light_SummonElemental:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Light_DayOfTheGods:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Light_PrismaticLight:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Light_DayOfProtection:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Light_HourOfPower:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Light_Sunray:
                    Projectile.Spawn(projectileInfo);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Light_DivineIntervention:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dark_Reanimate:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dark_ToxicCloud:
                    Projectile.Spawn(projectileInfo);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Dark_VampiricWeapon:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dark_ShrinkingRay:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dark_Sharpmetal:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dark_ControlUndead:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dark_PainReflection:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dark_DarkGrasp:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dark_DragonBreath:
                    Projectile.Spawn(projectileInfo);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Dark_Armageddon:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dark_Souldrinker:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.DarkElf_Glamour:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.DarkElf_TravelersBoon:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.DarkElf_Blind:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.DarkElf_DarkfireBolt:
                    Projectile.Spawn(projectileInfo);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Vampire_Lifedrain:
                    Projectile.Spawn(projectileInfo);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Vampire_Levitate:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Vampire_Charm:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Vampire_Mistform:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dragon_Fear:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dragon_FlameBlast:
                    Projectile.Spawn(projectileInfo);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Dragon_Flight:
                    if (GameCore.Instance.MapType == MapType.Indoor)
                    {
                        GameCore.SetStatusBarText("Can't cast Fly indoors !");
                        SoundMgr.PlaySoundById(SoundType.SpellFail);
                        return;
                    }

                    duration = 60 * skillLevel;
                    if (skillMastery == SkillMastery.Grandmaster)
                    {
                        power = 0;
                    }
                    else
                    {
                        power = 1;
                    }

                    Party.PartyBuffMap[PartyEffectType.Fly].Apply(skillMastery,
                        power,
                        GameTime.FromCurrentTime(60 * duration),
                        spell.Caster);
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Dragon_WingBuffer:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                default:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

            }

            spell.Caster.PlayEventReaction(CharacterReaction.CastedSpell);
        }
    }

}
