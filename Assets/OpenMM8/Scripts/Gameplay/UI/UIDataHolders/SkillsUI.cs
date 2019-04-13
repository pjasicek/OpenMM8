using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.OpenMM8.Scripts;
using System;

public class SkillsUI
{
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

    static public StatsUI Create()
    {
        GameObject parent = OpenMM8Util.GetGameObjAtScenePath("/PartyCanvas/CharDetailCanvas/Stats");
        if (parent == null)
        {
            throw new Exception("Could not find inventory's parent");
        }

        StatsUI ui = new StatsUI();
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
