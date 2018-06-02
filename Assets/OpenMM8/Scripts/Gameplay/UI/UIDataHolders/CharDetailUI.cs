﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class CharDetailUI
    {
        public Canvas CanvasHolder;

        public CharStatsUI StatsUI;
        public CharSkillsUI SkillsUI;
        public CharInventoryUI InventoryUI;
        public CharAwardsUI AwardsUI;

        public GameObject DollHolder;
        public Image DollBackgroundImage;
        public Image DollRightHandImage;
        public Image DollLeftHandImage;
        public Image DollBodyImage;
        public Image JewelryViewImage;

        static public CharDetailUI Load()
        {
            CharDetailUI ui = new CharDetailUI();

            GameObject holder = OpenMM8Util.GetGameObjAtScenePath("/PartyCanvas/CharDetailCanvas");
            if (holder == null)
            {
                Debug.Log("Holder is null");
            }
            ui.CanvasHolder = holder.GetComponent<Canvas>();

            ui.DollHolder = OpenMM8Util.GetGameObjAtScenePath("Doll", holder);
            ui.DollBackgroundImage = OpenMM8Util.GetComponentAtScenePath<Image>("Doll_Background", ui.DollHolder);
            ui.DollRightHandImage = OpenMM8Util.GetComponentAtScenePath<Image>("Doll_RightHand", ui.DollHolder);
            ui.DollLeftHandImage = OpenMM8Util.GetComponentAtScenePath<Image>("Doll_LeftHand", ui.DollHolder);
            ui.DollBodyImage = OpenMM8Util.GetComponentAtScenePath<Image>("Doll_Body", ui.DollHolder);
            ui.JewelryViewImage = OpenMM8Util.GetComponentAtScenePath<Image>("JewelryView", ui.DollHolder);

            ui.StatsUI = CharStatsUI.Load(holder);
            ui.SkillsUI = CharSkillsUI.Load(holder);
            ui.InventoryUI = CharInventoryUI.Load(holder);
            ui.AwardsUI = CharAwardsUI.Load(holder);

            OpenMM8Util.GetComponentAtScenePath<Button>("StatsButton", holder).onClick.AddListener(
                delegate { UiMgr.Instance.OnCharDetailButtonPressed("Stats"); });

            OpenMM8Util.GetComponentAtScenePath<Button>("SkillsButton", holder).onClick.AddListener(
                delegate { UiMgr.Instance.OnCharDetailButtonPressed("Skills"); });

            OpenMM8Util.GetComponentAtScenePath<Button>("InventoryButton", holder).onClick.AddListener(
                delegate { UiMgr.Instance.OnCharDetailButtonPressed("Inventory"); });

            OpenMM8Util.GetComponentAtScenePath<Button>("AwardsButton", holder).onClick.AddListener(
                delegate { UiMgr.Instance.OnCharDetailButtonPressed("Awards"); });

            OpenMM8Util.GetComponentAtScenePath<Button>("ExitButton", holder).onClick.AddListener(
                delegate { UiMgr.Instance.OnCharDetailButtonPressed("Escape"); });

            return ui;
        }
    }

    public class CharStatsUI
    {
        public GameObject Holder;

        static public CharStatsUI Load(GameObject origin)
        {
            CharStatsUI ui = new CharStatsUI();

            ui.Holder = OpenMM8Util.GetGameObjAtScenePath("Stats", origin);

            return ui;
        }
    }

    public class CharSkillsUI
    {
        public GameObject Holder;

        static public CharSkillsUI Load(GameObject origin)
        {
            CharSkillsUI ui = new CharSkillsUI();

            ui.Holder = OpenMM8Util.GetGameObjAtScenePath("Skills", origin);

            return ui;
        }
    }

    public class CharInventoryUI
    {
        public GameObject Holder;

        static public CharInventoryUI Load(GameObject origin)
        {
            CharInventoryUI ui = new CharInventoryUI();

            ui.Holder = OpenMM8Util.GetGameObjAtScenePath("Inventory", origin);

            return ui;
        }
    }

    public class CharAwardsUI
    {
        public GameObject Holder;

        static public CharAwardsUI Load(GameObject origin)
        {
            CharAwardsUI ui = new CharAwardsUI();

            ui.Holder = OpenMM8Util.GetGameObjAtScenePath("Awards", origin);

            return ui;
        }
    }
}