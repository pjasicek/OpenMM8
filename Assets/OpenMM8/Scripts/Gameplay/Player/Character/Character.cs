using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    class Character
    {
        public CharacterModel CharacterModel;
        public CharacterUI CharacterUI;

        public static Character Create(CharacterModel characterModel, CharacterUI characterUI)
        {
            Character character = new Character();
            character.CharacterModel = characterModel;
            character.CharacterUI = characterUI;

            return character;
        }

        public void OnUpdate(int msDiff)
        {

        }

        // Events
        void UnequipItem(ItemData item)
        {

        }

        void EquipItem(ItemData item)
        {

        }

        bool CanEquipItem(ItemData item)
        {
            return true;
        }

        public void OnItemPickedUp(ItemData item, bool fromPartyMember)
        {

        }

        public void ModifyCurrentHitPoints(int numHitPoints)
        {
            CharacterModel.CurrHitPoints += numHitPoints;
            int maxHP = CharacterModel.DefaultStats.MaxHitPoints + CharacterModel.BonusStats.MaxHitPoints;
            float healthPercent = ((float)CharacterModel.CurrHitPoints / (float)maxHP) * 100.0f;
            Debug.Log("CurrentHP : " + CharacterModel.CurrHitPoints + ", MaxHP: " + maxHP);
            CharacterUI.SetHealth(healthPercent);
        }

        public void ModifyCurrentSpellPoints(int numSpellPoints)
        {

        }

        public void AddLevel()
        {

        }

        public void IncreaseSkillLevel(SkillType skillType, int amount = 1)
        {

        }

        public void ModifyAttribute(Attribute attribute, int amount)
        {

        }

        public void ModifyResistance(SpellElement element, int amount)
        {

        }
    }
}
