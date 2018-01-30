using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;

namespace Assets.OpenMM8.Scripts.Gameplay.Managers
{
    class PartyUI
    {
        public Text GoldText;
        public Text FoodText;
        public List<CharacterUI> CharacterListUI = new List<CharacterUI>();

        PartyUI()
        {

        }

        void SetGold(int amount)
        {
            GoldText.text = amount.ToString();
        }

        void AddGold(int amount)
        {
            GoldText.text = (int.Parse(GoldText.text) + amount).ToString();
        }

        void SetFood(int amount)
        {
            FoodText.text = amount.ToString();
        }

        void AddFood(int amount)
        {
            FoodText.text = (int.Parse(FoodText.text) + amount).ToString();
        }

        void SetAllAgroStatus(AgroState state)
        {
            foreach (CharacterUI charUI in CharacterListUI)
            {
                if (charUI.IsOccuppied())
                {
                    charUI.SetAgroStatus(state);
                }
            }
        }

        void SetAllAvatarState(PlayerState state)
        {
            foreach (CharacterUI charUI in CharacterListUI)
            {
                if (charUI.IsOccuppied())
                {
                    charUI.SetAvatarState(state);
                }
            }
        }
    }
}
