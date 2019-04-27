using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class SpellbookPageUI
    {
        public GameObject Holder;
        public SpellSchool SpellSchool;
        public Sprite PageButtonDefaultSprite;
        public Button PageButton;
        public Dictionary<SpellType, SpellbookSpellButton> SpellButtons = new Dictionary<SpellType, SpellbookSpellButton>();

        // Parent
        public SpellbookUI SpellbookUI;

        public void DeselectAllSpells()
        {
            foreach (SpellbookSpellButton spellBtn in SpellButtons.Values)
            {
                spellBtn.IsClicked = false;
            }
        }
    }

    public class SpellbookUI
    {
        public GameObject Holder;
        public Canvas Canvas;

        public Text PlayerNameText;
        public Text SpellNameText;
        public Button QuickcastButton;
        public Button CloseButton;
        public Dictionary<SpellSchool, SpellbookPageUI> SpellbookPages = new Dictionary<SpellSchool, SpellbookPageUI>();

        public Character CurrentSpellbookOwner = null;

        public void DisplayForCharacter(Character chr)
        {
            // Hide everything
            foreach (SpellbookPageUI spellbookPage in SpellbookPages.Values)
            {
                spellbookPage.Holder.SetActive(false);
                spellbookPage.PageButton.gameObject.SetActive(false);

                spellbookPage.PageButton.onClick.AddListener(delegate
                {
                    if (UnityEngine.Random.Range(0, 2) == 0) SoundMgr.PlaySoundById(SoundType.TurnPageUp);
                    else SoundMgr.PlaySoundById(SoundType.TurnPageDown);

                    DisplaySpellSchool(chr, spellbookPage.SpellSchool);
                });
            }

            // Display spellbook pages based on which magic school the character has learned
            List<SpellSchool> availableSpellSchools = new List<SpellSchool>();
            if (chr.HasSkill(SkillType.FireMagic)) availableSpellSchools.Add(SpellSchool.Fire);
            if (chr.HasSkill(SkillType.AirMagic)) availableSpellSchools.Add(SpellSchool.Air);
            if (chr.HasSkill(SkillType.EarthMagic)) availableSpellSchools.Add(SpellSchool.Earth);
            if (chr.HasSkill(SkillType.WaterMagic)) availableSpellSchools.Add(SpellSchool.Water);
            if (chr.HasSkill(SkillType.SpiritMagic)) availableSpellSchools.Add(SpellSchool.Spirit);
            if (chr.HasSkill(SkillType.MindMagic)) availableSpellSchools.Add(SpellSchool.Mind);
            if (chr.HasSkill(SkillType.BodyMagic)) availableSpellSchools.Add(SpellSchool.Body);
            if (chr.HasSkill(SkillType.LightMagic)) availableSpellSchools.Add(SpellSchool.Light);
            if (chr.HasSkill(SkillType.DarkMagic)) availableSpellSchools.Add(SpellSchool.Dark);
            if (chr.HasSkill(SkillType.DarkElfAbility)) availableSpellSchools.Add(SpellSchool.DarkElf);
            if (chr.HasSkill(SkillType.VampireAbility)) availableSpellSchools.Add(SpellSchool.Vampire);
            if (chr.HasSkill(SkillType.DragonAbility)) availableSpellSchools.Add(SpellSchool.Dragon);

            foreach (SpellSchool availSpellSchool in availableSpellSchools)
            {
                SpellbookPages[availSpellSchool].PageButton.gameObject.SetActive(true);
            }

            // Fire spellbook is default - it is displayed even character knows no magic
            if (chr.LastSpellbookPage == SpellSchool.Fire)
            {
                // Check if he learned some other magic in the meantime
                if (!chr.HasSkill(SkillType.FireMagic) && availableSpellSchools.Count > 0)
                {
                    chr.LastSpellbookPage = availableSpellSchools[0];
                }
            }

            DisplaySpellSchool(chr, chr.LastSpellbookPage);

            Canvas.enabled = true;
            CurrentSpellbookOwner = chr;

            SoundMgr.PlaySoundById(SoundType.OpenBook);
        }

        public void DisplaySpellSchool(Character chr, SpellSchool spellSchool)
        {
            // Hide all existing
            foreach (SpellbookPageUI spellbookPage in SpellbookPages.Values)
            {
                spellbookPage.Holder.SetActive(false);
                spellbookPage.PageButton.image.sprite = spellbookPage.PageButtonDefaultSprite;
            }

            SpellbookPageUI currSpellbookPage = SpellbookPages[spellSchool];
            currSpellbookPage.Holder.SetActive(true);

            // Show only spells which are learned
            foreach (SpellbookSpellButton spellButton in currSpellbookPage.SpellButtons.Values)
            {
                if (chr.HasSpell(spellButton.SpellType))
                {
                    Debug.Log("Has spell: " + spellButton.SpellType);
                    spellButton.gameObject.SetActive(true);
                }
                else
                {
                    spellButton.gameObject.SetActive(false);
                }

                spellButton.IsClicked = false;
            }

            currSpellbookPage.PageButton.image.sprite = currSpellbookPage.PageButton.spriteState.pressedSprite;

            PlayerNameText.text = chr.Name;
        }

        public void Hide(Character chr)
        {
            // Save last used spellbook
            foreach (SpellbookPageUI spellbookPage in SpellbookPages.Values)
            {
                if (spellbookPage.Holder.active)
                {
                    chr.LastSpellbookPage = spellbookPage.SpellSchool;
                    break;
                }

                spellbookPage.PageButton.onClick.RemoveAllListeners();
            }

            Canvas.enabled = false;
            CurrentSpellbookOwner = null;
        }

        public static SpellbookUI Create()
        {
            SpellbookUI ui = new SpellbookUI();

            // Could be more generic, but it would be hareder to read the intent
            ui.Holder = OpenMM8Util.GetGameObjAtScenePath("/Spellbook");
            ui.Canvas = ui.Holder.GetComponent<Canvas>();
            ui.QuickcastButton = OpenMM8Util.GetComponentAtScenePath<Button>("QuickCastButton", ui.Holder);
            ui.CloseButton = OpenMM8Util.GetComponentAtScenePath<Button>("CloseButton", ui.Holder);
            ui.PlayerNameText = OpenMM8Util.GetComponentAtScenePath<Text>("PlayerNameText", ui.Holder);
            ui.SpellNameText = OpenMM8Util.GetComponentAtScenePath<Text>("SpellNameText", ui.Holder);

            foreach (SpellSchool spellSchool in Enum.GetValues(typeof(SpellSchool)))
            {
                if (spellSchool == SpellSchool.None)
                {
                    continue;
                }

                SpellbookPageUI spellbookPage = new SpellbookPageUI();
                ui.SpellbookPages.Add(spellSchool, spellbookPage);
                spellbookPage.SpellSchool = spellSchool;
                spellbookPage.SpellbookUI = ui;

                switch (spellSchool)
                {
                    case SpellSchool.Fire:
                        spellbookPage.Holder = OpenMM8Util.GetGameObjAtScenePath("Fire", ui.Holder);
                        spellbookPage.PageButton = OpenMM8Util.GetComponentAtScenePath<Button>("FireMagicButton", ui.Holder);
                        break;

                    case SpellSchool.Air:
                        spellbookPage.Holder = OpenMM8Util.GetGameObjAtScenePath("Air", ui.Holder);
                        spellbookPage.PageButton = OpenMM8Util.GetComponentAtScenePath<Button>("AirMagicButton", ui.Holder);
                        break;

                    case SpellSchool.Water:
                        spellbookPage.Holder = OpenMM8Util.GetGameObjAtScenePath("Water", ui.Holder);
                        spellbookPage.PageButton = OpenMM8Util.GetComponentAtScenePath<Button>("WaterMagicButton", ui.Holder);
                        break;

                    case SpellSchool.Earth:
                        spellbookPage.Holder = OpenMM8Util.GetGameObjAtScenePath("Earth", ui.Holder);
                        spellbookPage.PageButton = OpenMM8Util.GetComponentAtScenePath<Button>("EarthMagicButton", ui.Holder);
                        break;

                    case SpellSchool.Spirit:
                        spellbookPage.Holder = OpenMM8Util.GetGameObjAtScenePath("Spirit", ui.Holder);
                        spellbookPage.PageButton = OpenMM8Util.GetComponentAtScenePath<Button>("SpiritMagicButton", ui.Holder);
                        break;

                    case SpellSchool.Mind:
                        spellbookPage.Holder = OpenMM8Util.GetGameObjAtScenePath("Mind", ui.Holder);
                        spellbookPage.PageButton = OpenMM8Util.GetComponentAtScenePath<Button>("MindMagicButton", ui.Holder);
                        break;

                    case SpellSchool.Body:
                        spellbookPage.Holder = OpenMM8Util.GetGameObjAtScenePath("Body", ui.Holder);
                        spellbookPage.PageButton = OpenMM8Util.GetComponentAtScenePath<Button>("BodyMagicButton", ui.Holder);
                        break;

                    case SpellSchool.Light:
                        spellbookPage.Holder = OpenMM8Util.GetGameObjAtScenePath("Light", ui.Holder);
                        spellbookPage.PageButton = OpenMM8Util.GetComponentAtScenePath<Button>("LightMagicButton", ui.Holder);
                        break;

                    case SpellSchool.Dark:
                        spellbookPage.Holder = OpenMM8Util.GetGameObjAtScenePath("Dark", ui.Holder);
                        spellbookPage.PageButton = OpenMM8Util.GetComponentAtScenePath<Button>("DarkMagicButton", ui.Holder);
                        break;

                    case SpellSchool.DarkElf:
                        spellbookPage.Holder = OpenMM8Util.GetGameObjAtScenePath("DarkElf", ui.Holder);
                        spellbookPage.PageButton = OpenMM8Util.GetComponentAtScenePath<Button>("DarkElfButton", ui.Holder);
                        break;

                    case SpellSchool.Vampire:
                        spellbookPage.Holder = OpenMM8Util.GetGameObjAtScenePath("Vampire", ui.Holder);
                        spellbookPage.PageButton = OpenMM8Util.GetComponentAtScenePath<Button>("VampireButton", ui.Holder);
                        break;

                    case SpellSchool.Dragon:
                        spellbookPage.Holder = OpenMM8Util.GetGameObjAtScenePath("Dragon", ui.Holder);
                        spellbookPage.PageButton = OpenMM8Util.GetComponentAtScenePath<Button>("DragonButton", ui.Holder);
                        break;

                    default:
                        Debug.LogError("Unknown spellschool: " + spellSchool);
                        break;
                }

                spellbookPage.PageButtonDefaultSprite = spellbookPage.PageButton.image.sprite;

                SpellbookSpellButton[] spellButtons = spellbookPage.Holder.GetComponentsInChildren<SpellbookSpellButton>();
                foreach (SpellbookSpellButton spellButton in spellButtons)
                {
                    spellbookPage.SpellButtons[spellButton.SpellType] = spellButton;
                    spellButton.Parent = spellbookPage;
                }
            }

            return ui;
        }
    }
}
