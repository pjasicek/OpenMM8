using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.OpenMM8.Scripts;
using System;
using Assets.OpenMM8.Scripts.Gameplay;

public class StatsUI
{
    public Character Owner;

    public GameObject Holder;
    public Canvas Canvas;

    public Text NameText;
    public Text SkillPointsText;
    public Text MightText;
    public Text IntellectText;
    public Text PersonalityText;
    public Text EnduranceText;
    public Text AccuracyText;
    public Text SpeedText;
    public Text LuckText;
    public Text HitPointsText;
    public Text SpellPointsText;
    public Text ArmorClassText;
    public Text ConditionText;
    public Text QuickSpellText;
    public Text AgeText;
    public Text LevelText;
    public Text ExperienceText;
    public Text AttackText;
    public Text AttackDamageText;
    public Text ShootText;
    public Text ShootDamageText;
    public Text FireResistText;
    public Text AirResistText;
    public Text WaterResistText;
    public Text EarthResistText;
    public Text MindResistText;
    public Text BodyResistText;

    public void Refresh()
    {
        NameText.text = Owner.Name;

        string skillPointsStr = Owner.SkillPoints.ToString();
        if (Owner.SkillPoints > 0)
        {
            skillPointsStr = "<color=#00ff00ff>" + Owner.SkillPoints + "</color>";
        }
        SkillPointsText.text = "Skill Points: " + skillPointsStr;

        MightText.text = GenStatTextPair(Owner.GetActualMight(), Owner.GetBaseMight());
        IntellectText.text = GenStatTextPair(Owner.GetActualIntellect(), Owner.GetBaseIntellect());
        PersonalityText.text = GenStatTextPair(Owner.GetActualPersonality(), Owner.GetBasePersonality());
        EnduranceText.text = GenStatTextPair(Owner.GetActualEndurance(), Owner.GetBaseEndurance());
        AccuracyText.text = GenStatTextPair(Owner.GetActualAccuracy(), Owner.GetBaseAccuracy());
        SpeedText.text = GenStatTextPair(Owner.GetActualSpeed(), Owner.GetBaseSpeed());
        LuckText.text = GenStatTextPair(Owner.GetActualLuck(), Owner.GetBaseLuck());

        // hp/sp:
        // < 25% = red
        // < 100% = yellow
        if (Owner.GetHealthPercentage() < 25.0f)
        {
            HitPointsText.text = "<color=red>" + Owner.CurrHitPoints + "</color> / " + Owner.GetMaxHitPoints();
        }
        else if (Owner.GetHealthPercentage() < 100.0f)
        {
            HitPointsText.text = "<color=yellow>" + Owner.CurrHitPoints + "</color> / " + Owner.GetMaxHitPoints();
        }
        else
        {
            HitPointsText.text = Owner.CurrHitPoints + " / " + Owner.GetMaxHitPoints();
        }

        if (Owner.GetManaPercentage() < 25.0f)
        {
            SpellPointsText.text = "<color=red>" + Owner.CurrSpellPoints + "</color> / " + Owner.GetMaxSpellPoints();
        }
        else if (Owner.GetManaPercentage() < 100.0f)
        {
            SpellPointsText.text = "<color=yellow>" + Owner.CurrSpellPoints + "</color> / " + Owner.GetMaxSpellPoints();
        }
        else
        {
            SpellPointsText.text = Owner.CurrSpellPoints + " / " + Owner.GetMaxSpellPoints();
        }

        ArmorClassText.text = GenStatTextPair(Owner.GetActualArmorClass(), Owner.GetBaseArmorClass());

        ConditionText.text = Owner.Condition.ToString(); // TODO
        QuickSpellText.text = Owner.QuickSpellName;

        AgeText.text = GenStatTextPair(Owner.GetActualAge(), Owner.GetBaseAge());
        LevelText.text = GenStatTextPair(Owner.GetActualLevel(), Owner.Level);

        // Check if can reach next level
        if (Owner.CanTrainToNextLevel())
        {
            ExperienceText.text = "<color=#00ff00ff>" + Owner.Experience + "</color>";
        }
        else
        {
            ExperienceText.text = Owner.Experience.ToString();
        }

        int meleeAttack = Owner.GetMeleeAttack();
        string attackBonusStr = meleeAttack.ToString();
        if (meleeAttack >= 0)
        {
            attackBonusStr = "+" + meleeAttack;
        }
        string attackDamageStr = Owner.GetMeleeDamageMin() + " - " + Owner.GetMeleeDamageMax();

        int shootAttack = Owner.GetRangedAttack();
        string shootBonusStr = shootAttack.ToString();
        if (shootAttack >= 0)
        {
            shootBonusStr = "+" + shootAttack;
        }
        string shootDamageStr = Owner.GetRangedDamageMin() + " - " + Owner.GetRangedDamageMax();
        if (Owner.UI.DollUI.Bow.Item == null)
        {
            shootDamageStr = "N/A";
        }


        AttackText.text = attackBonusStr;
        AttackDamageText.text = attackDamageStr;
        ShootText.text = shootBonusStr;
        ShootDamageText.text = shootDamageStr;

        FireResistText.text = GenResistTextPair(Owner.GetActualResistance(SpellElement.Fire), Owner.GetBaseResistance(SpellElement.Fire));
        WaterResistText.text = GenResistTextPair(Owner.GetActualResistance(SpellElement.Water), Owner.GetBaseResistance(SpellElement.Water));
        AirResistText.text = GenResistTextPair(Owner.GetActualResistance(SpellElement.Air), Owner.GetBaseResistance(SpellElement.Air));
        EarthResistText.text = GenResistTextPair(Owner.GetActualResistance(SpellElement.Earth), Owner.GetBaseResistance(SpellElement.Earth));
        BodyResistText.text = GenResistTextPair(Owner.GetActualResistance(SpellElement.Body), Owner.GetBaseResistance(SpellElement.Body));
        MindResistText.text = GenResistTextPair(Owner.GetActualResistance(SpellElement.Mind), Owner.GetBaseResistance(SpellElement.Mind));
    }

    private string GenResistTextPair(int actualAmount, int baseAmount)
    {
        if (baseAmount == int.MaxValue)
        {
            return "Immune";
        }
        else
        {
            return GenStatTextPair(actualAmount, baseAmount);
        }
    }

    private string GenStatTextPair(int actualAmount, int baseAmount)
    {
        if (actualAmount > baseAmount)
        {
            return "<color=#00ff00ff>" + actualAmount + "</color> / " + baseAmount;
        }
        else if (actualAmount < baseAmount)
        {
            return "<color=red>" + actualAmount + "</color> / " + baseAmount;
        }
        else
        {
            return actualAmount + " / " + baseAmount;
        }
    }

    static public StatsUI Create(Character owner)
    {
        GameObject parent = OpenMM8Util.GetGameObjAtScenePath("/PartyCanvas/CharDetailCanvas/Stats");
        if (parent == null)
        {
            throw new Exception("Could not find inventory's parent");
        }

        StatsUI ui = new StatsUI();
        ui.Owner = owner;
        ui.Holder = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/UI/CharacterStats/CharStats"),
            parent.transform);

        ui.NameText = OpenMM8Util.GetComponentAtScenePath<Text>("Name", ui.Holder);
        ui.SkillPointsText = OpenMM8Util.GetComponentAtScenePath<Text>("SkillPoints", ui.Holder);
        ui.MightText = OpenMM8Util.GetComponentAtScenePath<Text>("Might", ui.Holder);
        ui.IntellectText = OpenMM8Util.GetComponentAtScenePath<Text>("Intellect", ui.Holder);
        ui.PersonalityText = OpenMM8Util.GetComponentAtScenePath<Text>("Personality", ui.Holder);
        ui.EnduranceText = OpenMM8Util.GetComponentAtScenePath<Text>("Endurance", ui.Holder);
        ui.AccuracyText = OpenMM8Util.GetComponentAtScenePath<Text>("Accuracy", ui.Holder);
        ui.SpeedText = OpenMM8Util.GetComponentAtScenePath<Text>("Speed", ui.Holder);
        ui.LuckText = OpenMM8Util.GetComponentAtScenePath<Text>("Luck", ui.Holder);
        ui.HitPointsText = OpenMM8Util.GetComponentAtScenePath<Text>("HitPoints", ui.Holder);
        ui.SpellPointsText = OpenMM8Util.GetComponentAtScenePath<Text>("SpellPoints", ui.Holder);
        ui.ArmorClassText = OpenMM8Util.GetComponentAtScenePath<Text>("ArmorClass", ui.Holder);
        ui.ConditionText = OpenMM8Util.GetComponentAtScenePath<Text>("Condition", ui.Holder);
        ui.QuickSpellText = OpenMM8Util.GetComponentAtScenePath<Text>("QuickSpell", ui.Holder);
        ui.AgeText = OpenMM8Util.GetComponentAtScenePath<Text>("Age", ui.Holder);
        ui.LevelText = OpenMM8Util.GetComponentAtScenePath<Text>("Level", ui.Holder);
        ui.ExperienceText = OpenMM8Util.GetComponentAtScenePath<Text>("Experience", ui.Holder);
        ui.AttackText = OpenMM8Util.GetComponentAtScenePath<Text>("Attack", ui.Holder);
        ui.AttackDamageText = OpenMM8Util.GetComponentAtScenePath<Text>("AttackDamage", ui.Holder);
        ui.ShootText = OpenMM8Util.GetComponentAtScenePath<Text>("Shoot", ui.Holder);
        ui.ShootDamageText = OpenMM8Util.GetComponentAtScenePath<Text>("ShootDamage", ui.Holder);
        ui.FireResistText = OpenMM8Util.GetComponentAtScenePath<Text>("FireResist", ui.Holder);
        ui.AirResistText = OpenMM8Util.GetComponentAtScenePath<Text>("AirResist", ui.Holder);
        ui.WaterResistText = OpenMM8Util.GetComponentAtScenePath<Text>("WaterResist", ui.Holder);
        ui.EarthResistText = OpenMM8Util.GetComponentAtScenePath<Text>("EarthResist", ui.Holder);
        ui.MindResistText = OpenMM8Util.GetComponentAtScenePath<Text>("MindResist", ui.Holder);
        ui.BodyResistText = OpenMM8Util.GetComponentAtScenePath<Text>("BodyResist", ui.Holder);

        return ui;
    }
}
