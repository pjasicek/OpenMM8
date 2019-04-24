using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.OpenMM8.Scripts.Gameplay;
using Assets.OpenMM8.Scripts;

public class PartyBuffUI
{
    public PlayerParty Party;

    public Image FireResistanceImage;
    public Image WaterResistanceImage;
    public Image AirResistanceImage;
    public Image EarthResistanceImage;
    public Image BodyResistanceImage;
    public Image MindResistanceImage;
    public Image ShieldImage;
    public Image HeroismImage;
    public Image HasteImage;
    public Image ImmolationImage;

    public Image FeatherFallImage;
    public Image StoneskinImage;
    public Image DayOfGodsImage;
    public Image TorchlightImage;
    public Image WizardEyeImage;
    public Image ProtectionFromMagicImage;

    public void Refresh()
    {
        FireResistanceImage.enabled = Party.PartyBuffMap[PartyEffectType.ResistFire].IsActive();
        WaterResistanceImage.enabled = Party.PartyBuffMap[PartyEffectType.ResistWater].IsActive();
        AirResistanceImage.enabled = Party.PartyBuffMap[PartyEffectType.ResistAir].IsActive();
        EarthResistanceImage.enabled = Party.PartyBuffMap[PartyEffectType.ResistEarth].IsActive();
        BodyResistanceImage.enabled = Party.PartyBuffMap[PartyEffectType.ResistBody].IsActive();
        MindResistanceImage.enabled = Party.PartyBuffMap[PartyEffectType.ResistMind].IsActive();
        ShieldImage.enabled = Party.PartyBuffMap[PartyEffectType.Shield].IsActive();
        HeroismImage.enabled = Party.PartyBuffMap[PartyEffectType.Heroism].IsActive();
        HasteImage.enabled = Party.PartyBuffMap[PartyEffectType.Haste].IsActive();
        ImmolationImage.enabled = Party.PartyBuffMap[PartyEffectType.Immolation].IsActive();

        FeatherFallImage.enabled = Party.PartyBuffMap[PartyEffectType.FeatherFall].IsActive();
        StoneskinImage.enabled = Party.PartyBuffMap[PartyEffectType.StoneSkin].IsActive();
        DayOfGodsImage.enabled = Party.PartyBuffMap[PartyEffectType.DayOfTheGods].IsActive();
        TorchlightImage.enabled = Party.PartyBuffMap[PartyEffectType.Torchlight].IsActive();
        WizardEyeImage.enabled = Party.PartyBuffMap[PartyEffectType.WizardEye].IsActive();
        ProtectionFromMagicImage.enabled = Party.PartyBuffMap[PartyEffectType.ProtectionFromMagic].IsActive();
    }

    static public PartyBuffUI Create(PlayerParty playerParty)
    {
        PartyBuffUI ui = new PartyBuffUI();
        ui.Party = playerParty;

        GameObject holderLeft = OpenMM8Util.GetGameObjAtScenePath("/PartyCanvas/BuffsAndButtonsCanvas/PartyBuffsLeft");
        ui.FireResistanceImage = OpenMM8Util.GetComponentAtScenePath<Image>("Buff_FireResistance", holderLeft);
        ui.WaterResistanceImage = OpenMM8Util.GetComponentAtScenePath<Image>("Buff_WaterResistance", holderLeft);
        ui.AirResistanceImage = OpenMM8Util.GetComponentAtScenePath<Image>("Buff_AirResistance", holderLeft);
        ui.EarthResistanceImage = OpenMM8Util.GetComponentAtScenePath<Image>("Buff_EarthResistance", holderLeft);
        ui.MindResistanceImage = OpenMM8Util.GetComponentAtScenePath<Image>("Buff_MindResistance", holderLeft);
        ui.BodyResistanceImage = OpenMM8Util.GetComponentAtScenePath<Image>("Buff_BodyResistance", holderLeft);
        ui.ShieldImage = OpenMM8Util.GetComponentAtScenePath<Image>("Buff_Shield", holderLeft);
        ui.HeroismImage = OpenMM8Util.GetComponentAtScenePath<Image>("Buff_Heroism", holderLeft);
        ui.HasteImage = OpenMM8Util.GetComponentAtScenePath<Image>("Buff_Haste", holderLeft);
        ui.ImmolationImage = OpenMM8Util.GetComponentAtScenePath<Image>("Buff_Immolation", holderLeft);

        GameObject holderRight = OpenMM8Util.GetGameObjAtScenePath("/PartyCanvas/BuffsAndButtonsCanvas/PartyBuffsRight");
        ui.FeatherFallImage = OpenMM8Util.GetComponentAtScenePath<Image>("Buff_FeatherFall", holderRight);
        ui.StoneskinImage = OpenMM8Util.GetComponentAtScenePath<Image>("Buff_Stoneskin", holderRight);
        ui.DayOfGodsImage = OpenMM8Util.GetComponentAtScenePath<Image>("Buff_DayOfGods", holderRight);
        ui.TorchlightImage = OpenMM8Util.GetComponentAtScenePath<Image>("Buff_Torchlight", holderRight);
        ui.WizardEyeImage = OpenMM8Util.GetComponentAtScenePath<Image>("Buff_WizardEye", holderRight);
        ui.ProtectionFromMagicImage = OpenMM8Util.GetComponentAtScenePath<Image>("Buff_ProtectionFromMagic", holderRight);

        return ui;
    }
}
