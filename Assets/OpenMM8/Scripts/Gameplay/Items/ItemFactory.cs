using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay.Items
{
    static class ItemFactory
    {
        public static BaseItem CreateItem(ItemData itemData)
        {
            switch (itemData.ItemType)
            {
                case ItemType.WeaponOneHanded:
                case ItemType.WeaponTwoHanded:
                case ItemType.WeaponDualWield:
                case ItemType.Wand:
                case ItemType.Missile:
                    return new WeaponItem(itemData);

                case ItemType.Armor:
                case ItemType.Shield:
                case ItemType.Helmet:
                case ItemType.Belt:
                case ItemType.Cloak:
                case ItemType.Gauntlets:
                case ItemType.Boots:
                case ItemType.Ring:
                case ItemType.Amulet:
                    return new ArmorItem(itemData);

                case ItemType.Reagent:
                case ItemType.Bottle:
                case ItemType.SpellScroll:
                    return new ConsumableItem(itemData);


                case ItemType.SpellBook:
                    return new LearnableItem(itemData);

                case ItemType.MessageScroll:
                    return new ReadableItem(itemData);

                default:
                    return new BaseItem(itemData);
            }
        }
    }
}
