using Assets.OpenMM8.Scripts.Gameplay.Data;
using Assets.OpenMM8.Scripts.Gameplay.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class InspectableUiSpell : InspectableUiText
    {
        private SpellType m_SpellType;

        private void Start()
        {
            m_SpellType = GetComponent<SpellbookSpellButton>().SpellType;
        }

        override public string GetHeader()
        {
            SpellData data = DbMgr.Instance.SpellDataDb.Get(m_SpellType);

            return data.Name;
        }

        override public string GetInfoText()
        {
            SpellData data = DbMgr.Instance.SpellDataDb.Get(m_SpellType);

            string text = data.Description + "\n\n" +
                          "    Normal: " + data.NormalDescription + "\n" +
                          "    Expert: " + data.ExpertDescription + "\n" +
                          "    Master: " + data.MasterDescription + "\n" +
                          "    Grandmaster: " + data.GrandmasterDescription + "\n\n" + 
                          "    MP Cost: TODO";

            return text;
        }
    }
}
