using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public enum EnchantType
    {
        Standard,
        Special,
        None
    }

    public enum EnchantPriceMultType
    {
        Add,
        Multiply
    }

    public class ItemEnchant
    {
        public EnchantType EnchantType = EnchantType.None;
        public Dictionary<StatType, int> StatBonusMap = new Dictionary<StatType, int>();
        public string BonusDescText;
        public string OfTypeText;
        public EnchantPriceMultType EnchantPriceMultType = EnchantPriceMultType.Add;
        public int PriceModAmount;
    }
}
