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
    public class InspectableUiStat : InspectableUiText
    {
        public string Header;
        public string InfoText;

        override public string GetHeader()
        {
            return Header;
        }

        override public string GetInfoText()
        {
            return InfoText;
        }
    }
}
