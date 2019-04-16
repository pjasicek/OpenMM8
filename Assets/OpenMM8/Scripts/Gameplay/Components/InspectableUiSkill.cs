using Assets.OpenMM8.Scripts.Data;
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
    public class InspectableUiSkill : InspectableUiText, IPointerDownHandler
    {
        public SkillType SkillType = SkillType.None;

        override public string GetHeader()
        {
            SkillDescriptionData descData = DbMgr.Instance.SkillDescriptionDb.Get(SkillType);
            if (descData != null)
            {
                return descData.Name;
            }

            return "N/A";
        }

        override public string GetInfoText()
        {
            // TODO: Better way ?
            Character owner = GameMgr.Instance.PlayerParty.GetActiveCharacter();
            SkillDescriptionData descData = DbMgr.Instance.SkillDescriptionDb.Get(SkillType);
            if (owner != null && descData != null)
            {
                CharacterClass nextPromotionClass = GameMechanics.GetNextClassPromotion(owner.Class);

                ClassSkillsData currClassSkillsData = DbMgr.Instance.ClassSkillsDb.Get(owner.Class);
                ClassSkillsData nextClassSkillsData = null;

                if (nextPromotionClass != CharacterClass.None)
                {
                    nextClassSkillsData = DbMgr.Instance.ClassSkillsDb.Get(nextPromotionClass);
                }

                string infoText = "";
                infoText += descData.Description + "\n\n";
                
                // TODO: Wrap into some helper method ?
                // Normal
                if (currClassSkillsData.SkillTypeToSkillMasteryMap[SkillType] >= SkillMastery.Normal)
                {
                    // Skill mastery available with current class
                    infoText += "Normal: " + descData.Normal + "\n";
                }
                else if (nextClassSkillsData != null &&
                    nextClassSkillsData.SkillTypeToSkillMasteryMap[SkillType] >= SkillMastery.Normal)
                {
                    // Skill mastery available with next class (after promotion) - yellow
                    infoText += "<color=yellow>Normal: " + descData.Normal + "</color>\n";
                }
                else
                {
                    // Skill mastery not available at all - red
                    infoText += "<color=red>Normal: " + descData.Normal + "</color>\n";
                }

                // Expert
                if (currClassSkillsData.SkillTypeToSkillMasteryMap[SkillType] >= SkillMastery.Expert)
                {
                    // Skill mastery available with current class
                    infoText += "Expert: " + descData.Expert + "\n";
                }
                else if (nextClassSkillsData != null &&
                    nextClassSkillsData.SkillTypeToSkillMasteryMap[SkillType] >= SkillMastery.Expert)
                {
                    // Skill mastery available with next class (after promotion) - yellow
                    infoText += "<color=yellow>Expert: " + descData.Expert + "</color>\n";
                }
                else
                {
                    // Skill mastery not available at all - red
                    infoText += "<color=red>Expert: " + descData.Expert + "</color>\n";
                }

                // Master
                if (currClassSkillsData.SkillTypeToSkillMasteryMap[SkillType] >= SkillMastery.Master)
                {
                    // Skill mastery available with current class
                    infoText += "Master: " + descData.Master + "\n";
                }
                else if (nextClassSkillsData != null &&
                    nextClassSkillsData.SkillTypeToSkillMasteryMap[SkillType] >= SkillMastery.Master)
                {
                    // Skill mastery available with next class (after promotion) - yellow
                    infoText += "<color=yellow>Master: " + descData.Master + "</color>\n";
                }
                else
                {
                    // Skill mastery not available at all - red
                    infoText += "<color=red>Master: " + descData.Master + "</color>\n";
                }

                // GrandMaster
                if (currClassSkillsData.SkillTypeToSkillMasteryMap[SkillType] >= SkillMastery.Grandmaster)
                {
                    // Skill mastery available with current class
                    infoText += "Grandmaster: " + descData.GrandMaster;
                }
                else if (nextClassSkillsData != null &&
                    nextClassSkillsData.SkillTypeToSkillMasteryMap[SkillType] >= SkillMastery.Grandmaster)
                {
                    // Skill mastery available with next class (after promotion) - yellow
                    infoText += "<color=yellow>Grandmaster: " + descData.GrandMaster + "</color>";
                }
                else
                {
                    // Skill mastery not available at all - red
                    infoText += "<color=red>Grandmaster: " + descData.GrandMaster + "</color>";
                }

                int skillBonus = owner.GetItemsBonus(GameMechanics.SkillToAttributeBonus(SkillType));
                if (skillBonus > 0)
                {
                    infoText += "\n\nBonus: " + skillBonus;
                }

                return infoText;
            }

            return "N/A";
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Character owner = GameMgr.Instance.PlayerParty.GetActiveCharacter();
                owner.OnSkillClicked(SkillType);
            }
        }
    }
}
