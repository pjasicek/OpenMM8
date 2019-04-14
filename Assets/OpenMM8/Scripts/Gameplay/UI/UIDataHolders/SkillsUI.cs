using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.OpenMM8.Scripts;
using System;
using System.Collections.Generic;
using Assets.OpenMM8.Scripts.Gameplay;
using Assets.OpenMM8.Scripts.Data;
using System.Linq;

public class SkillUIRow
{
    public GameObject Holder;
    public RectTransform HolderRt;
    public Text SkillText;
    public Text LevelText;

    public string SkillName;
    public Skill Skill;
}

public class SkillsUI
{
    public Character Owner;

    public GameObject Holder;

    public Text NameText;
    public Text SkillPointsText;
    public RectTransform WeaponsHolderRt;
    public RectTransform MagicHolderRt;
    public RectTransform ArmorHolderRt;
    public RectTransform MiscHolderRt;

    public List<SkillUIRow> WeaponSkillRows = new List<SkillUIRow>();
    public List<SkillUIRow> MagicSkillRows = new List<SkillUIRow>();
    public List<SkillUIRow> ArmorSkillRows = new List<SkillUIRow>();
    public List<SkillUIRow> MiscSkillRows = new List<SkillUIRow>();

    public void AddSkillRow(Skill skill)
    {
        SkillType skillType = skill.Type;
        SkillDescriptionData skillDescData = DbMgr.Instance.SkillDescriptionDb.Get(skillType);
        if (skillDescData == null)
        {
            Debug.LogError("Cannot create skill row with invalid skill");
        }

        GameObject rowHolder = null;
        List<SkillUIRow> targetRowContainer = null;
        switch (skillDescData.SkillGroup)
        {
            case SkillGroupType.Weapon:
                if (WeaponSkillRows.FirstOrDefault(row => row.Skill.Type == skillType) != null)
                {
                    Debug.LogError("Skill UI already has row with skill: " + skillType);
                    return;
                }
                rowHolder = WeaponsHolderRt.gameObject;
                targetRowContainer = WeaponSkillRows;
                break;

            case SkillGroupType.Armor:
                if (ArmorSkillRows.FirstOrDefault(row => row.Skill.Type == skillType) != null)
                {
                    Debug.LogError("Skill UI already has row with skill: " + skillType);
                    return;
                }
                rowHolder = ArmorHolderRt.gameObject;
                targetRowContainer = ArmorSkillRows;
                break;

            case SkillGroupType.Magic:
                if (MagicSkillRows.FirstOrDefault(row => row.Skill.Type == skillType) != null)
                {
                    Debug.LogError("Skill UI already has row with skill: " + skillType);
                    return;
                }
                rowHolder = MagicHolderRt.gameObject;
                targetRowContainer = MagicSkillRows;
                break;

            case SkillGroupType.Misc:
                if (MiscSkillRows.FirstOrDefault(row => row.Skill.Type == skillType) != null)
                {
                    Debug.LogError("Skill UI already has row with skill: " + skillType);
                    return;
                }
                rowHolder = MiscHolderRt.gameObject;
                targetRowContainer = MiscSkillRows;
                break;

            default:
                Debug.LogError("Invalid skill group: " + skillDescData.SkillGroup);
                return;
        }

        // Check if already have

        SkillUIRow skillUIRow = new SkillUIRow();
        skillUIRow.Skill = skill;
        skillUIRow.Holder = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/UI/CharacterSkills/SkillRow"),
            rowHolder.transform);
        skillUIRow.HolderRt = skillUIRow.Holder.GetComponent<RectTransform>();
        skillUIRow.SkillText = skillUIRow.Holder.GetComponent<Text>();
        skillUIRow.LevelText = OpenMM8Util.GetComponentAtScenePath<Text>("SkillLevel", skillUIRow.Holder);

        skillUIRow.Holder.GetComponent<InspectableUiSkill>().SkillType = skillType;

        skillUIRow.SkillName = skillDescData.Name;
        skillUIRow.SkillText.text = skillDescData.Name;
        skillUIRow.LevelText.text = skillUIRow.Skill.Level.ToString();

        if (skillType == SkillType.None)
        {
            skillUIRow.SkillText.gameObject.GetComponent<InspectableUiSkill>().enabled = false;
            skillUIRow.SkillText.text = "None";
            skillUIRow.LevelText.text = "";
        }

        // Check if the 

        targetRowContainer.Add(skillUIRow);

        // Setup position

        // TODO: Should I do this really every time ?
        RepositionSkillRows();
    }

    // Skills that can be learnt (enough skill points) - blue
    // Rest - white
    public void Repaint(int availSkillPoints)
    {
        Action<List<SkillUIRow>> rowRepainter = (rows) =>
        {
            foreach (SkillUIRow row in rows)
            {
                if (row.Skill.Type == SkillType.None)
                {
                    continue;
                }

                if (row.Skill.Level < availSkillPoints)
                {
                    // Blue-ish
                    row.SkillText.color = new Color(0, 174 / 255.0f, 255 / 255.0f);
                    row.LevelText.color = new Color(0, 174 / 255.0f, 255 / 255.0f);
                }
                else
                {
                    row.SkillText.color = Color.white;
                    row.LevelText.color = Color.white;
                }

                row.LevelText.text = row.Skill.Level.ToString();
                if (row.Skill.Mastery == SkillMastery.Expert)
                {
                    row.SkillText.text = row.SkillName + " Expert";
                }
                else if (row.Skill.Mastery == SkillMastery.Master)
                {
                    row.SkillText.text = row.SkillName + " Master";
                }
                else if (row.Skill.Mastery == SkillMastery.Grandmaster)
                {
                    row.SkillText.text = row.SkillName + " Grandmaster";
                }
                else
                {
                    row.SkillText.text = row.SkillName;
                }
            }
        };

        rowRepainter(WeaponSkillRows);
        rowRepainter(ArmorSkillRows);
        rowRepainter(MagicSkillRows);
        rowRepainter(MiscSkillRows);

        if (Owner.SkillPoints > 0)
        {
            SkillPointsText.text = "Skill Points: <color=#00ff00ff>" + Owner.SkillPoints + "</color>";
        }
        else
        {
            SkillPointsText.text = "Skill Points: " + Owner.SkillPoints;
        }
    }
    
    public void RepositionSkillRows()
    {
        //float scale = WeaponsHolder.GetComponent<RectTransform>().localScale;
        float scale = 0.1f;

        // Left column - Weapons + Magic

        // Scaled pixels
        float rowY = -1.0f * WeaponsHolderRt.rect.height;
        // Unscaled pixels
        float totalRowsHeight = 0.0f;

        foreach (SkillUIRow row in WeaponSkillRows)
        {
            row.HolderRt.anchoredPosition = new Vector2(7.5f / scale, rowY);
            rowY -= row.HolderRt.rect.height;
            totalRowsHeight += row.HolderRt.rect.height * scale;
        }

        // Spacer between sections
        totalRowsHeight += 10.0f;

        float belowWeaponsHeaderY = WeaponsHolderRt.anchoredPosition.y -
            WeaponsHolderRt.rect.height * scale;
        MagicHolderRt.anchoredPosition = new Vector2(MagicHolderRt.anchoredPosition.x,
            belowWeaponsHeaderY - totalRowsHeight);
        rowY = -1.0f * MagicHolderRt.rect.height;

        foreach (SkillUIRow row in MagicSkillRows)
        {
            row.HolderRt.anchoredPosition = new Vector2(7.5f / scale, rowY);
            rowY -= row.HolderRt.rect.height * scale;
        }


        // Right column - Armors + Misc


        rowY = -1.0f * ArmorHolderRt.rect.height;
        totalRowsHeight = 0.0f;

        foreach (SkillUIRow row in ArmorSkillRows)
        {
            row.HolderRt.anchoredPosition = new Vector2(7.5f / scale, rowY);
            rowY -= row.HolderRt.rect.height;
            totalRowsHeight += row.HolderRt.rect.height * scale;
        }

        // Spacer between sections
        totalRowsHeight += 10.0f;

        float belowArmorsHeaderY = WeaponsHolderRt.anchoredPosition.y -
            WeaponsHolderRt.rect.height * scale;
        MiscHolderRt.anchoredPosition = new Vector2(MiscHolderRt.anchoredPosition.x,
            belowArmorsHeaderY - totalRowsHeight);
        rowY = -1.0f * MiscHolderRt.rect.height;

        foreach (SkillUIRow row in MiscSkillRows)
        {
            row.HolderRt.anchoredPosition = new Vector2(7.5f / scale, rowY);
            rowY -= row.HolderRt.rect.height * scale;
        }
    }

    public void Refresh()
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        Action<List<SkillUIRow>> removeNoneRows = (rows) =>
        {
            int nonNoneRowCount = rows.Count(row => row.Skill.Type != SkillType.None);
            if (nonNoneRowCount == 0)
            {
                return;
            }

            foreach (SkillUIRow row in rows)
            {
                if (row.Skill.Type == SkillType.None)
                {
                    GameObject.Destroy(row.Holder);
                }
            }
            rows.RemoveAll(row => row.Skill.Type == SkillType.None);
        };

        // Remove all "None" skills when there is atleast one skill in given skillgroup
        removeNoneRows(WeaponSkillRows);
        removeNoneRows(ArmorSkillRows);
        removeNoneRows(MagicSkillRows);
        removeNoneRows(MiscSkillRows);

        RepositionSkillRows();
        Repaint(Owner.SkillPoints);

        sw.Stop();
        Debug.Log("Skill UI refresh: " + sw.ElapsedMilliseconds + " ms");
    }

    static public SkillsUI Create(Character owner)
    {
        GameObject parent = OpenMM8Util.GetGameObjAtScenePath("/PartyCanvas/CharDetailCanvas/Skills");
        if (parent == null)
        {
            throw new Exception("Could not find inventory's parent");
        }

        SkillsUI ui = new SkillsUI();
        ui.Owner = owner;
        ui.Holder = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/UI/CharacterSkills/CharSkills"),
            parent.transform);

        ui.NameText = OpenMM8Util.GetComponentAtScenePath<Text>("NameText", ui.Holder);
        ui.SkillPointsText = OpenMM8Util.GetComponentAtScenePath<Text>("SkillPointsText", ui.Holder);
        ui.WeaponsHolderRt = OpenMM8Util.GetComponentAtScenePath<RectTransform>("WeaponsLabel", ui.Holder);
        ui.ArmorHolderRt = OpenMM8Util.GetComponentAtScenePath<RectTransform>("ArmorLabel", ui.Holder);
        ui.MiscHolderRt = OpenMM8Util.GetComponentAtScenePath<RectTransform>("MiscLabel", ui.Holder);
        ui.MagicHolderRt = OpenMM8Util.GetComponentAtScenePath<RectTransform>("MagicLabel", ui.Holder);

        ui.NameText.text = owner.Name;

        // Initialize with "None" skills
        ui.AddNoneRow(ui.WeaponSkillRows, ui.WeaponsHolderRt.gameObject);
        ui.AddNoneRow(ui.ArmorSkillRows, ui.ArmorHolderRt.gameObject);
        ui.AddNoneRow(ui.MagicSkillRows, ui.MagicHolderRt.gameObject);
        ui.AddNoneRow(ui.MiscSkillRows, ui.MiscHolderRt.gameObject);

        return ui;
    }

    private void AddNoneRow(List<SkillUIRow> rowList, GameObject holder)
    {
        SkillUIRow noneRow = new SkillUIRow();
        noneRow.Skill = new Skill();
        noneRow.Skill.Type = SkillType.None;
        noneRow.Holder = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/UI/CharacterSkills/NoneRow"),
            holder.transform);
        noneRow.HolderRt = noneRow.Holder.GetComponent<RectTransform>();

        rowList.Add(noneRow);
    }
}
