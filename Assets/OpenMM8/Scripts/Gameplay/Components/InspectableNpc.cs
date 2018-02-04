using UnityEngine;
using System.Collections;

using Assets.OpenMM8.Scripts.Gameplay;

[RequireComponent(typeof(BaseNpc))]
public class InspectableNpc : Inspectable
{
    private NpcData NpcData = null;
    private BaseNpc Npc = null;

    void Start()
    {
        Npc = GetComponent<BaseNpc>();
    }

    public override Canvas SetupInspectCanvas()
    {
        if (NpcData == null)
        {
            NpcData = Npc.NpcData;
        }

        InspectNpcUI ui = GameMgr.Instance.InspectNpcUI;
        ui.NpcNameText.text = NpcData.Name;
        ui.HitPointsText.text = Npc.CurrentHitPoints.ToString() + "/" + NpcData.HitPoints;
        ui.ArmorClassText.text = NpcData.ArmorClass.ToString();
        ui.AttackText.text = NpcData.Attack1.DamageType.ToString();
        ui.DamageText.text = NpcData.AttackAmountText;
        ui.SpellText.text = NpcData.SpellAttack1.SpellName == "" ? "None" : NpcData.SpellAttack1.SpellName;
        ui.FireResistanceText.text = NpcData.Resistances[SpellElement.Fire].ToString();
        ui.AirResistanceText.text = NpcData.Resistances[SpellElement.Air].ToString();
        ui.WaterResistanceText.text = NpcData.Resistances[SpellElement.Water].ToString();
        ui.EarthResistanceText.text = NpcData.Resistances[SpellElement.Earth].ToString();
        ui.MindResistanceText.text = NpcData.Resistances[SpellElement.Mind].ToString();
        ui.SpiritResistanceText.text = NpcData.Resistances[SpellElement.Spirit].ToString();
        ui.BodyResistanceText.text = NpcData.Resistances[SpellElement.Body].ToString();
        ui.LightResistanceText.text = NpcData.Resistances[SpellElement.Light].ToString();
        ui.DarkResistanceText.text = NpcData.Resistances[SpellElement.Dark].ToString();
        ui.PhysicalResistanceText.text = NpcData.Resistances[SpellElement.Physical].ToString();

        ui.PreviewImage.sprite = GetComponent<BaseNpc>().PreviewImage;

        ui.Healthbar.fillAmount = (float)Npc.CurrentHitPoints / (float)NpcData.HitPoints;
        if (ui.Healthbar.fillAmount > 0.66f)
        {
            ui.Healthbar.sprite = GameMgr.Instance.GreenInspectNpcHealthbar;
        }
        else if (ui.Healthbar.fillAmount > 0.33f)
        {
            ui.Healthbar.sprite = GameMgr.Instance.YellowInspectNpcHealthbar;
        }
        else
        {
            ui.Healthbar.sprite = GameMgr.Instance.RedInspectNpcHealthbar;
        }

        return GameMgr.Instance.InspectNpcUI.Canvas;
    }
}
