using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay.Items
{
    static class ItemFactory
    {
        public static BaseItem CreateItem(ref ItemData itemData)
        {
            switch (itemData.EquipType)
            {
                case EquipType.WeaponOneHanded:
                case EquipType.WeaponTwoHanded:
                case EquipType.WeaponDualWield:
                case EquipType.Wand:
                case EquipType.Missile:
                    return new WeaponItem(ref itemData);

                case EquipType.Armor:
                case EquipType.Shield:
                case EquipType.Helmet:
                case EquipType.Belt:
                case EquipType.Cloak:
                case EquipType.Gauntlets:
                case EquipType.Boots:
                case EquipType.Ring:
                case EquipType.Amulet:
                    return new ArmorItem(ref itemData);

                case EquipType.Reagent:
                case EquipType.Bottle:
                case EquipType.SpellScroll:
                    return new ConsumableItem(ref itemData);


                case EquipType.SpellBook:
                    return new LearnableItem(ref itemData);

                case EquipType.MessageScroll:
                    return new ReadableItem(ref itemData);

                default:
                    return new BaseItem(ref itemData);
            }
        }
    }
}
