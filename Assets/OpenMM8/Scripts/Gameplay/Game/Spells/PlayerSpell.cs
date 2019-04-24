using Assets.OpenMM8.Scripts.Gameplay;
using Assets.OpenMM8.Scripts.Gameplay.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerSpell
{
    public SpellType SpellType;
    public SkillMastery SkillMastery = SkillMastery.None;
    public int SkillLevel;
    public int RequiredMana;
    public int RecoveryTime;
    public Character Caster;
    public object Target; // Could really be anything...
    public CastSpellFlags Flags;
    public bool DisplayCrosshair;

    static public PlayerSpell CreateFromSpellbook(SpellType spellType, Character caster)
    {
        int spellSchoolIdx = (int)(spellType - 1) / 11;
        if (!Enum.IsDefined(typeof(SpellSchool), spellSchoolIdx))
        {
            Debug.LogError("Invalid dervied spell school for spell: " + spellType);
            return null;
        }

        SpellSchool spellSchool = (SpellSchool)spellSchoolIdx;
        SkillType skillType = GameMechanics.SpellSchoolToSkillType(spellSchool);
        if (skillType == SkillType.None)
        {
            Debug.LogError("Invalid skill type derived from SpellSchool: " + spellSchool);
            return null;
        }

        int skillLevel = caster.GetActualSkillLevel(skillType);
        SkillMastery skillMastery = caster.GetSkillMastery(skillType);
        if (skillMastery == SkillMastery.None)
        {
            Debug.LogError("No skill mastery for: " + skillType);
            return null;
        }

        SpellData spellData = DbMgr.Instance.SpellDataDb.Get(spellType);
        if (spellData == null)
        {
            Debug.LogError("No spell data for: " + spellType);
            return null;
        }

        int recoveryTime = int.MaxValue;
        int requiredMana = int.MaxValue;
        switch (skillMastery)
        {
            case SkillMastery.Normal:
                requiredMana = spellData.ManaCostNormal;
                recoveryTime = spellData.RecoveryRateNormal;
                break;
            case SkillMastery.Expert:
                requiredMana = spellData.ManaCostExpert;
                recoveryTime = spellData.RecoveryRateExpert;
                break;
            case SkillMastery.Master:
                requiredMana = spellData.ManaCostMaster;
                recoveryTime = spellData.RecoveryRateMaster;
                break;
            case SkillMastery.Grandmaster:
                requiredMana = spellData.ManaCostGrandmaster;
                recoveryTime = spellData.RecoveryRateGrandmaster;
                break;

            default:
                Debug.LogError("Unknown skill mastery: " + skillMastery);
                return null;
        }

        PlayerSpell newSpell = new PlayerSpell();
        newSpell.SpellType = spellType;
        newSpell.Caster = caster;
        newSpell.SkillLevel = skillLevel;
        newSpell.SkillMastery = skillMastery;
        newSpell.RequiredMana = requiredMana;
        newSpell.RecoveryTime = recoveryTime;

        return newSpell;
    }
}

[Flags]
public enum CastSpellFlags
{
    TargetCharacter =   0x2,
    TargetOutdoorItem = 0x4,
    TargetNpc =         0x8,
    TargetMesh =        0x10,
    ItemEnchantment =   0x20,
    TargetCorpse =      0x40,
}