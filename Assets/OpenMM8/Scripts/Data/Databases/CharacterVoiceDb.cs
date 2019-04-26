using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Assets.OpenMM8.Scripts.Gameplay.Items;
using Assets.OpenMM8.Scripts.Data;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay.Data
{
    // CHARACTER_VOICES
    public class CharacterVoiceData : DbData
    {
        // int Id = character id (CharacterData)
        // Lists of sound IDs - Sound IDs <-> Sound Names are defined in SoundData
        public List<int> TrapDisarmedOk = new List<int>();
        public List<int> TrapDisarmedFail = new List<int>();
        public List<int> DoorsClosed = new List<int>();
        public List<int> ChooseMe = new List<int>();
        public List<int> BadItem = new List<int>();
        public List<int> GoodItem = new List<int>();
        public List<int> CantIdentify = new List<int>();
        public List<int> Repaired = new List<int>();
        public List<int> CantRepair = new List<int>();
        public List<int> EasyFight = new List<int>();
        public List<int> HardFight = new List<int>();
        public List<int> CantIdMonster = new List<int>();
        public List<int> QuickSpell = new List<int>();
        public List<int> Hungry = new List<int>();
        public List<int> SoftInjured = new List<int>();
        public List<int> Injured = new List<int>();
        public List<int> HardInjured = new List<int>();
        public List<int> Drunk = new List<int>();
        public List<int> Insane = new List<int>();
        public List<int> Poisoned = new List<int>();
        public List<int> Misc = new List<int>();
        public List<int> Fall = new List<int>();
        public List<int> CantRestHere = new List<int>();
        public List<int> NeedMoreGold = new List<int>();
        public List<int> InventoryFull = new List<int>();
        public List<int> PotionMixed = new List<int>();
        public List<int> FailMixing = new List<int>();
        public List<int> NeedAKey = new List<int>();
        public List<int> LearnSpell = new List<int>();
        public List<int> CantLearn = new List<int>();
        public List<int> CantEquip = new List<int>();
        public List<int> GoodDay = new List<int>();
        public List<int> GoodEvening = new List<int>(); // 21pm - 5am
        public List<int> Win = new List<int>();
        public List<int> Heh = new List<int>();
        public List<int> LastStanding = new List<int>();
        public List<int> HardFightEnd = new List<int>();
        public List<int> EnterDungeon = new List<int>();
        public List<int> Yes = new List<int>();
        public List<int> Thanks = new List<int>();
        public List<int> SomeoneWasRude = new List<int>();
        public List<int> Move = new List<int>();
    }

    public class CharacterVoiceDb : DataDb<CharacterVoiceData>
    {
        override public CharacterVoiceData ProcessCsvDataRow(int row, string[] columns)
        {
            if (row == 0 || row > 31)
            {
                return null;
            }

            CharacterVoiceData data = new CharacterVoiceData();
            data.Id = int.Parse(columns[0]);

            data.TrapDisarmedOk.Add(int.Parse(columns[ColumnToNumber("B")]));
            data.TrapDisarmedOk.Add(int.Parse(columns[ColumnToNumber("C")]));
            data.DoorsClosed.Add(int.Parse(columns[ColumnToNumber("D")]));
            data.DoorsClosed.Add(int.Parse(columns[ColumnToNumber("E")]));
            data.TrapDisarmedFail.Add(int.Parse(columns[ColumnToNumber("F")]));
            data.TrapDisarmedFail.Add(int.Parse(columns[ColumnToNumber("G")]));
            data.ChooseMe.Add(int.Parse(columns[ColumnToNumber("H")]));
            data.ChooseMe.Add(int.Parse(columns[ColumnToNumber("I")]));
            data.BadItem.Add(int.Parse(columns[ColumnToNumber("J")]));
            data.BadItem.Add(int.Parse(columns[ColumnToNumber("K")]));
            data.GoodItem.Add(int.Parse(columns[ColumnToNumber("L")]));
            data.GoodItem.Add(int.Parse(columns[ColumnToNumber("M")]));
            data.CantIdentify.Add(int.Parse(columns[ColumnToNumber("N")]));
            data.CantIdentify.Add(int.Parse(columns[ColumnToNumber("O")]));
            data.Repaired.Add(int.Parse(columns[ColumnToNumber("P")]));
            data.Repaired.Add(int.Parse(columns[ColumnToNumber("Q")]));
            data.CantRepair.Add(int.Parse(columns[ColumnToNumber("R")]));
            data.CantRepair.Add(int.Parse(columns[ColumnToNumber("S")]));
            data.EasyFight.Add(int.Parse(columns[ColumnToNumber("T")]));
            data.EasyFight.Add(int.Parse(columns[ColumnToNumber("U")]));
            data.HardFight.Add(int.Parse(columns[ColumnToNumber("V")]));
            data.HardFight.Add(int.Parse(columns[ColumnToNumber("W")]));
            data.CantIdMonster.Add(int.Parse(columns[ColumnToNumber("X")]));
            data.CantIdMonster.Add(int.Parse(columns[ColumnToNumber("Y")]));
            data.QuickSpell.Add(int.Parse(columns[ColumnToNumber("Z")]));

            data.QuickSpell.Add(int.Parse(columns[ColumnToNumber("AA")]));
            data.Hungry.Add(int.Parse(columns[ColumnToNumber("AB")]));
            data.Hungry.Add(int.Parse(columns[ColumnToNumber("AC")]));
            data.SoftInjured.Add(int.Parse(columns[ColumnToNumber("AD")]));
            data.SoftInjured.Add(int.Parse(columns[ColumnToNumber("AE")]));
            data.Injured.Add(int.Parse(columns[ColumnToNumber("AF")]));
            data.HardInjured.Add(int.Parse(columns[ColumnToNumber("AG")]));
            data.HardInjured.Add(int.Parse(columns[ColumnToNumber("AH")]));
            data.Drunk.Add(int.Parse(columns[ColumnToNumber("AI")]));
            data.Drunk.Add(int.Parse(columns[ColumnToNumber("AK")]));
            data.Insane.Add(int.Parse(columns[ColumnToNumber("AL")]));
            data.Insane.Add(int.Parse(columns[ColumnToNumber("AM")]));
            data.Poisoned.Add(int.Parse(columns[ColumnToNumber("AN")]));
            data.Poisoned.Add(int.Parse(columns[ColumnToNumber("AO")]));
            data.Misc.Add(int.Parse(columns[ColumnToNumber("AP")]));
            data.Misc.Add(int.Parse(columns[ColumnToNumber("AQ")]));
            data.Fall.Add(int.Parse(columns[ColumnToNumber("AR")]));
            data.Fall.Add(int.Parse(columns[ColumnToNumber("AS")]));
            data.CantRestHere.Add(int.Parse(columns[ColumnToNumber("AT")]));
            data.CantRestHere.Add(int.Parse(columns[ColumnToNumber("AU")]));
            data.NeedMoreGold.Add(int.Parse(columns[ColumnToNumber("AV")]));
            data.NeedMoreGold.Add(int.Parse(columns[ColumnToNumber("AW")]));
            data.InventoryFull.Add(int.Parse(columns[ColumnToNumber("AX")]));
            data.InventoryFull.Add(int.Parse(columns[ColumnToNumber("AY")]));
            data.PotionMixed.Add(int.Parse(columns[ColumnToNumber("AZ")]));

            data.PotionMixed.Add(int.Parse(columns[ColumnToNumber("BA")]));
            data.FailMixing.Add(int.Parse(columns[ColumnToNumber("BB")]));
            data.FailMixing.Add(int.Parse(columns[ColumnToNumber("BC")]));
            data.NeedAKey.Add(int.Parse(columns[ColumnToNumber("BD")]));
            data.NeedAKey.Add(int.Parse(columns[ColumnToNumber("BE")]));
            data.LearnSpell.Add(int.Parse(columns[ColumnToNumber("BF")]));
            data.LearnSpell.Add(int.Parse(columns[ColumnToNumber("BG")]));
            data.CantLearn.Add(int.Parse(columns[ColumnToNumber("BH")]));
            data.CantLearn.Add(int.Parse(columns[ColumnToNumber("BI")]));
            data.CantEquip.Add(int.Parse(columns[ColumnToNumber("BJ")]));
            data.CantEquip.Add(int.Parse(columns[ColumnToNumber("BK")]));
            data.GoodDay.Add(int.Parse(columns[ColumnToNumber("BL")]));
            data.GoodDay.Add(int.Parse(columns[ColumnToNumber("BM")]));
            data.GoodDay.Add(int.Parse(columns[ColumnToNumber("BN")]));
            data.GoodDay.Add(int.Parse(columns[ColumnToNumber("BO")]));
            data.GoodEvening.Add(int.Parse(columns[ColumnToNumber("BP")]));
            data.GoodEvening.Add(int.Parse(columns[ColumnToNumber("BQ")]));

            data.Win.Add(int.Parse(columns[ColumnToNumber("BV")]));
            data.Win.Add(int.Parse(columns[ColumnToNumber("BW")]));
            data.Heh.Add(int.Parse(columns[ColumnToNumber("BX")]));
            data.Heh.Add(int.Parse(columns[ColumnToNumber("BY")]));
            data.LastStanding.Add(int.Parse(columns[ColumnToNumber("BZ")]));

            data.LastStanding.Add(int.Parse(columns[ColumnToNumber("CA")]));
            data.HardFightEnd.Add(int.Parse(columns[ColumnToNumber("CB")]));
            data.EnterDungeon.Add(int.Parse(columns[ColumnToNumber("CC")]));
            data.EnterDungeon.Add(int.Parse(columns[ColumnToNumber("CD")]));
            data.EnterDungeon.Add(int.Parse(columns[ColumnToNumber("CE")]));
            data.Yes.Add(int.Parse(columns[ColumnToNumber("CF")]));
            data.Yes.Add(int.Parse(columns[ColumnToNumber("CG")]));
            data.Thanks.Add(int.Parse(columns[ColumnToNumber("CH")]));
            data.Thanks.Add(int.Parse(columns[ColumnToNumber("CI")]));
            data.SomeoneWasRude.Add(int.Parse(columns[ColumnToNumber("CJ")]));
            data.SomeoneWasRude.Add(int.Parse(columns[ColumnToNumber("CK")]));

            data.Move.Add(int.Parse(columns[ColumnToNumber("CP")]));
            data.Move.Add(int.Parse(columns[ColumnToNumber("CQ")]));

            return data;
        }
    }
}
