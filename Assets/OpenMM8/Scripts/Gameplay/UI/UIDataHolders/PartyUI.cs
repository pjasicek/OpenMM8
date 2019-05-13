using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class PartyUI
    {
        public PlayerParty Party;

        public Text GoldText;
        public Text FoodText;
        public Text StatusBarText;

        public List<Image> EmptySlotBanners = new List<Image>();

        public void Refresh()
        {
            GoldText.text = Party.Gold.ToString();
            FoodText.text = Party.Food.ToString();
        }

        public void UpdateLayout()
        {
            foreach (Character chr in Party.Characters)
            {
                chr.UI.Holder.transform.localPosition =
                    new Vector3(Constants.PC_WidthDelta, 0.0f, 0.0f) * chr.GetPartyIndex();
            }

            UpdateEmptySlotBanners();
        }

        public void UpdateEmptySlotBanners()
        {
            int numPartyMembers = Party.Characters.Count;
            for (int emptySlotIdx = 0; emptySlotIdx < EmptySlotBanners.Count; emptySlotIdx++)
            {
                if (emptySlotIdx < numPartyMembers)
                {
                    EmptySlotBanners[emptySlotIdx].enabled = false;
                }
                else
                {
                    EmptySlotBanners[emptySlotIdx].enabled = true;
                }
            }
        }

        static public PartyUI Create(PlayerParty playerParty)
        {
            PartyUI ui = new PartyUI();
            ui.Party = playerParty;

            ui.GoldText = UiMgr.Instance.PartyCanvasHolder.transform.Find("GoldCountText").GetComponent<Text>();
            ui.FoodText = UiMgr.Instance.PartyCanvasHolder.transform.Find("FoodCountText").GetComponent<Text>();
            ui.StatusBarText = UiMgr.Instance.PartyCanvasHolder.transform.Find("BaseBarImage").transform.Find("HoverInfoText").GetComponent<Text>();

            ui.EmptySlotBanners.Add(UiMgr.Instance.PartyCanvasHolder.transform.Find("PC1_EmptySlot").GetComponent<Image>());
            ui.EmptySlotBanners.Add(UiMgr.Instance.PartyCanvasHolder.transform.Find("PC2_EmptySlot").GetComponent<Image>());
            ui.EmptySlotBanners.Add(UiMgr.Instance.PartyCanvasHolder.transform.Find("PC3_EmptySlot").GetComponent<Image>());
            ui.EmptySlotBanners.Add(UiMgr.Instance.PartyCanvasHolder.transform.Find("PC4_EmptySlot").GetComponent<Image>());
            ui.EmptySlotBanners.Add(UiMgr.Instance.PartyCanvasHolder.transform.Find("PC5_EmptySlot").GetComponent<Image>());

            return ui;
        }

        /*public void SetGold(int amount)
        {
            GoldText.text = amount.ToString();
        }

        public void AddGold(int amount)
        {
            GoldText.text = (int.Parse(GoldText.text) + amount).ToString();
        }

        public void SetFood(int amount)
        {
            FoodText.text = amount.ToString();
        }

        public void AddFood(int amount)
        {
            FoodText.text = (int.Parse(FoodText.text) + amount).ToString();
        }*/
    }
}
