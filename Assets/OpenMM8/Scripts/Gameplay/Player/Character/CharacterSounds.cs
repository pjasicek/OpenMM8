using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class CharacterSounds
    {
        public List<AudioClip> DisarmTrap = new List<AudioClip>();
        public List<AudioClip> ClosedStore = new List<AudioClip>();
        public List<AudioClip> SetOffTrap = new List<AudioClip>();
        public List<AudioClip> SelectCharacter = new List<AudioClip>();
        public List<AudioClip> IdentifyWeakItem = new List<AudioClip>();
        public List<AudioClip> IdentifyPowerfulItem = new List<AudioClip>();
        public List<AudioClip> FailToIdentifyItem = new List<AudioClip>();
        public List<AudioClip> RepairItem = new List<AudioClip>();
        public List<AudioClip> FailToRepairItem = new List<AudioClip>();
        public List<AudioClip> IdentifyWeakMonster = new List<AudioClip>();
        public List<AudioClip> IdentifyPowerfulMonster = new List<AudioClip>();
        public List<AudioClip> FailToIdentifyMonster = new List<AudioClip>();
        public List<AudioClip> SetNewQuickcast = new List<AudioClip>();
        public List<AudioClip> Hungry = new List<AudioClip>();
        public List<AudioClip> BarelyWounded = new List<AudioClip>();
        public List<AudioClip> BadlyWounded = new List<AudioClip>();
        public List<AudioClip> Dying = new List<AudioClip>();
        public List<AudioClip> BecomeDrunk = new List<AudioClip>();
        public List<AudioClip> BecomeInsane = new List<AudioClip>();
        public List<AudioClip> BecomePoisoned = new List<AudioClip>();
        public List<AudioClip> BecomeCursed = new List<AudioClip>();
        public List<AudioClip> Falling = new List<AudioClip>();
        public List<AudioClip> UnableToRest = new List<AudioClip>();
        public List<AudioClip> NotEnoughGold = new List<AudioClip>();
        public List<AudioClip> NoRoomInInventory = new List<AudioClip>();
        public List<AudioClip> MixedPotion = new List<AudioClip>();
        public List<AudioClip> FailMixedPotion = new List<AudioClip>();
        public List<AudioClip> LockedDoor = new List<AudioClip>();
        public List<AudioClip> LearnedNewSpell = new List<AudioClip>();
        public List<AudioClip> CantLearnSpell = new List<AudioClip>();
        public List<AudioClip> CantEquipItem = new List<AudioClip>();
        public List<AudioClip> Greeting = new List<AudioClip>();
        public List<AudioClip> KilledStrongMonster = new List<AudioClip>();
        public List<AudioClip> KilledWeakMonster = new List<AudioClip>();
        public List<AudioClip> LastPersonStanding = new List<AudioClip>();
        public List<AudioClip> LeavingDungeon = new List<AudioClip>();
        public List<AudioClip> EnteringDungeon = new List<AudioClip>();
        public List<AudioClip> Yes = new List<AudioClip>();
        public List<AudioClip> ThankYou = new List<AudioClip>();
        public List<AudioClip> Indignant = new List<AudioClip>();
        public List<AudioClip> Yell = new List<AudioClip>();

        public static CharacterSounds Load(CharacterType type)
        {
            CharacterSounds characterSounds = new CharacterSounds();

            string path = "Player/PlayerCharacters/Sounds/" + type.ToString();
            UnityEngine.Object[] sounds = Resources.LoadAll(path);
            foreach (AudioClip sound in sounds)
            {
                string soundTypeStr = sound.name.Substring(4, 2);
                if ((type == CharacterType.Lich_1) || (type == CharacterType.LichFemale_1))
                {
                    soundTypeStr = sound.name.Substring(3, 2);
                    switch (soundTypeStr)
                    {
                        case "01": characterSounds.DisarmTrap.Add(sound); break;
                        case "02": characterSounds.ClosedStore.Add(sound); break;
                        case "03": characterSounds.SetOffTrap.Add(sound); break;
                        case "04": characterSounds.SelectCharacter.Add(sound); break;
                        case "05": characterSounds.IdentifyWeakItem.Add(sound); break;
                        case "06": characterSounds.IdentifyPowerfulItem.Add(sound); break;
                        case "07": characterSounds.FailToIdentifyItem.Add(sound); break;
                        case "08": characterSounds.RepairItem.Add(sound); break;
                        case "09": characterSounds.FailToRepairItem.Add(sound); break;
                        case "10": characterSounds.IdentifyWeakMonster.Add(sound); break;
                        case "11": characterSounds.IdentifyPowerfulMonster.Add(sound); break;
                        case "12": characterSounds.FailToIdentifyMonster.Add(sound); break;
                        case "13": characterSounds.SetNewQuickcast.Add(sound); break;
                        case "14": characterSounds.Hungry.Add(sound); break;
                        case "15": characterSounds.BarelyWounded.Add(sound); break;
                        case "16": characterSounds.BadlyWounded.Add(sound); break;
                        case "17": characterSounds.Dying.Add(sound); break;
                        case "18": characterSounds.BecomeDrunk.Add(sound); break;
                        case "19": characterSounds.BecomeInsane.Add(sound); break;
                        case "20": characterSounds.BecomePoisoned.Add(sound); break;
                        case "21": characterSounds.BecomeCursed.Add(sound); break;
                        case "22": characterSounds.Falling.Add(sound); break;
                        case "23": characterSounds.UnableToRest.Add(sound); break;
                        case "24": characterSounds.NotEnoughGold.Add(sound); break;
                        case "25": characterSounds.NoRoomInInventory.Add(sound); break;
                        case "26": characterSounds.MixedPotion.Add(sound); break;
                        case "27": characterSounds.FailMixedPotion.Add(sound); break;
                        case "28": characterSounds.LockedDoor.Add(sound); break;
                        case "29": characterSounds.LearnedNewSpell.Add(sound); break;
                        case "30": characterSounds.CantLearnSpell.Add(sound); break;
                        case "31": characterSounds.CantEquipItem.Add(sound); break;
                        case "32":
                        case "33":
                        case "34":
                            characterSounds.Greeting.Add(sound);
                            break;
                        case "37": characterSounds.KilledStrongMonster.Add(sound); break;
                        case "38": characterSounds.KilledWeakMonster.Add(sound); break;
                        case "39": characterSounds.LastPersonStanding.Add(sound); break;
                        case "40": characterSounds.LeavingDungeon.Add(sound); break;
                        case "41": characterSounds.EnteringDungeon.Add(sound); break;
                        case "42": characterSounds.Yes.Add(sound); break;
                        case "44": characterSounds.Indignant.Add(sound); break;
                        case "47": characterSounds.Yell.Add(sound); break;
                        default: Debug.Log("Unknown sound: " + sound.name); break;
                    }
                }
                else if ((type == CharacterType.Dragon_1) || (type == CharacterType.Dragon_2))
                {
                    switch (soundTypeStr)
                    {
                        case "01": characterSounds.DisarmTrap.Add(sound); break;
                        case "02": characterSounds.ClosedStore.Add(sound); break;
                        case "03": characterSounds.SetOffTrap.Add(sound); break;
                        //case "04": characterSounds.SelectCharacter.Add(sound); break;
                        case "04": characterSounds.IdentifyWeakItem.Add(sound); break;
                        case "05": characterSounds.IdentifyPowerfulItem.Add(sound); break;
                        case "06": characterSounds.FailToIdentifyItem.Add(sound); break;
                        case "07": characterSounds.RepairItem.Add(sound); break;
                        case "08": characterSounds.FailToRepairItem.Add(sound); break;
                        case "09": characterSounds.IdentifyWeakMonster.Add(sound); break;
                        case "10": characterSounds.IdentifyPowerfulMonster.Add(sound); break;
                        case "11": characterSounds.FailToIdentifyMonster.Add(sound); break;
                        case "12": characterSounds.SetNewQuickcast.Add(sound); break;
                        case "13": characterSounds.Hungry.Add(sound); break;
                        case "14": characterSounds.BarelyWounded.Add(sound); break;
                        case "15": characterSounds.BadlyWounded.Add(sound); break;
                        case "16": characterSounds.Dying.Add(sound); break;
                        case "17": characterSounds.BecomeDrunk.Add(sound); break;
                        case "18": characterSounds.BecomeInsane.Add(sound); break;
                        case "19": characterSounds.BecomePoisoned.Add(sound); break;
                        case "20": characterSounds.BecomeCursed.Add(sound); break;
                        case "21": characterSounds.Falling.Add(sound); break;
                        case "22": characterSounds.UnableToRest.Add(sound); break;
                        case "23": characterSounds.NotEnoughGold.Add(sound); break;
                        case "24": characterSounds.NoRoomInInventory.Add(sound); break;
                        case "25": characterSounds.MixedPotion.Add(sound); break;
                        case "26": characterSounds.FailMixedPotion.Add(sound); break;
                        case "27": characterSounds.LockedDoor.Add(sound); break;
                        case "28": characterSounds.LearnedNewSpell.Add(sound); break;
                        case "29": characterSounds.CantLearnSpell.Add(sound); break;
                        case "30": characterSounds.CantEquipItem.Add(sound); break;
                        case "31":
                        case "32":
                        case "33":
                            characterSounds.Greeting.Add(sound);
                            break;
                        case "34": characterSounds.KilledStrongMonster.Add(sound); break;
                        case "35": characterSounds.KilledWeakMonster.Add(sound); break;
                        case "36": characterSounds.LastPersonStanding.Add(sound); break;
                        case "37": characterSounds.LeavingDungeon.Add(sound); break;
                        case "38": characterSounds.EnteringDungeon.Add(sound); break;
                        case "39": characterSounds.Yes.Add(sound); break;
                        case "40": characterSounds.Indignant.Add(sound); break;
                        default: Debug.Log("Unknown sound: " + sound.name); break;
                    }
                }
                else if (type == CharacterType.Knight_1)
                {
                    switch (soundTypeStr)
                    {
                        case "01": characterSounds.DisarmTrap.Add(sound); break;
                        case "02": characterSounds.ClosedStore.Add(sound); break;
                        case "03": characterSounds.SetOffTrap.Add(sound); break;
                        case "04": characterSounds.SelectCharacter.Add(sound); break;
                        case "05": characterSounds.IdentifyWeakItem.Add(sound); break;
                        case "06": characterSounds.IdentifyPowerfulItem.Add(sound); break;
                        case "07": characterSounds.FailToIdentifyItem.Add(sound); break;
                        case "08": characterSounds.RepairItem.Add(sound); break;
                        case "09": characterSounds.FailToRepairItem.Add(sound); break;
                        case "10": characterSounds.IdentifyWeakMonster.Add(sound); break;
                        case "11": characterSounds.IdentifyPowerfulMonster.Add(sound); break;
                        case "12": characterSounds.FailToIdentifyMonster.Add(sound); break;
                        case "13": characterSounds.SetNewQuickcast.Add(sound); break;
                        case "14": characterSounds.Hungry.Add(sound); break;
                        case "15": characterSounds.BarelyWounded.Add(sound); break;
                        case "16": characterSounds.BadlyWounded.Add(sound); break;
                        case "17": characterSounds.Dying.Add(sound); break;
                        case "18": characterSounds.BecomeDrunk.Add(sound); break;
                        case "19": characterSounds.BecomeInsane.Add(sound); break;
                        case "20": characterSounds.BecomePoisoned.Add(sound); break;
                        case "21": characterSounds.BecomeCursed.Add(sound); break;
                        case "22": characterSounds.Falling.Add(sound); break;
                        case "23": characterSounds.UnableToRest.Add(sound); break;
                        case "24": characterSounds.NotEnoughGold.Add(sound); break;
                        case "25": characterSounds.NoRoomInInventory.Add(sound); break;
                        case "26": characterSounds.MixedPotion.Add(sound); break;
                        case "27": characterSounds.FailMixedPotion.Add(sound); break;
                        case "28": characterSounds.LockedDoor.Add(sound); break;
                        case "29": characterSounds.LearnedNewSpell.Add(sound); break;
                        case "30": characterSounds.CantLearnSpell.Add(sound); break;
                        case "40": characterSounds.CantEquipItem.Add(sound); break;
                        case "31":
                        case "32":
                        case "33":
                            characterSounds.Greeting.Add(sound);
                            break;
                        case "34": characterSounds.KilledStrongMonster.Add(sound); break;
                        case "35": characterSounds.KilledWeakMonster.Add(sound); break;
                        case "36": characterSounds.LastPersonStanding.Add(sound); break;
                        case "37": characterSounds.LeavingDungeon.Add(sound); break;
                        case "38": characterSounds.EnteringDungeon.Add(sound); break;
                        case "39": characterSounds.Yes.Add(sound); break;
                        case "41": characterSounds.Indignant.Add(sound); break;
                        case "43": characterSounds.Yell.Add(sound); break;
                        default: Debug.Log("Unknown sound: " + sound.name); break;
                    }
                }
                else
                {
                    switch (soundTypeStr)
                    {
                        case "01": characterSounds.DisarmTrap.Add(sound); break;
                        case "02": characterSounds.ClosedStore.Add(sound); break;
                        case "03": characterSounds.SetOffTrap.Add(sound); break;
                        case "04": characterSounds.SelectCharacter.Add(sound); break;
                        case "05": characterSounds.IdentifyWeakItem.Add(sound); break;
                        case "06": characterSounds.IdentifyPowerfulItem.Add(sound); break;
                        case "07": characterSounds.FailToIdentifyItem.Add(sound); break;
                        case "08": characterSounds.RepairItem.Add(sound); break;
                        case "09": characterSounds.FailToRepairItem.Add(sound); break;
                        case "10": characterSounds.IdentifyWeakMonster.Add(sound); break;
                        case "11": characterSounds.IdentifyPowerfulMonster.Add(sound); break;
                        case "12": characterSounds.FailToIdentifyMonster.Add(sound); break;
                        case "13": characterSounds.SetNewQuickcast.Add(sound); break;
                        case "14": characterSounds.Hungry.Add(sound); break;
                        case "15": characterSounds.BarelyWounded.Add(sound); break;
                        case "16": characterSounds.BadlyWounded.Add(sound); break;
                        case "17": characterSounds.Dying.Add(sound); break;
                        case "18": characterSounds.BecomeDrunk.Add(sound); break;
                        case "19": characterSounds.BecomeInsane.Add(sound); break;
                        case "20": characterSounds.BecomePoisoned.Add(sound); break;
                        case "21": characterSounds.BecomeCursed.Add(sound); break;
                        case "22": characterSounds.Falling.Add(sound); break;
                        case "23": characterSounds.UnableToRest.Add(sound); break;
                        case "24": characterSounds.NotEnoughGold.Add(sound); break;
                        case "25": characterSounds.NoRoomInInventory.Add(sound); break;
                        case "26": characterSounds.MixedPotion.Add(sound); break;
                        case "27": characterSounds.FailMixedPotion.Add(sound); break;
                        case "28": characterSounds.LockedDoor.Add(sound); break;
                        case "29": characterSounds.LearnedNewSpell.Add(sound); break;
                        case "30": characterSounds.CantLearnSpell.Add(sound); break;
                        case "31": characterSounds.CantEquipItem.Add(sound); break;
                        case "32":
                        case "33":
                        case "34":
                            characterSounds.Greeting.Add(sound);
                            break;
                        case "35": characterSounds.KilledStrongMonster.Add(sound); break;
                        case "36": characterSounds.KilledWeakMonster.Add(sound); break;
                        case "37": characterSounds.LastPersonStanding.Add(sound); break;
                        case "38": characterSounds.LeavingDungeon.Add(sound); break;
                        case "39": characterSounds.EnteringDungeon.Add(sound); break;
                        case "40": characterSounds.Yes.Add(sound); break;
                        case "42": characterSounds.Indignant.Add(sound); break;
                        case "43": characterSounds.Yell.Add(sound); break;
                        default: Debug.Log("Unknown sound: " + sound.name); break;
                    }
                }
            }

            return characterSounds;
        }
    }
}
