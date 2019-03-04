using Assets.OpenMM8.Scripts.Gameplay.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class InventoryHelper
    {
        static public bool PlaceItemOnDoll(BaseItem item, Character chr)
        {
            switch (item.Data.EquipType)
            {
                case EquipType.Wand:
                case EquipType.WeaponOneHanded:
                    break;

                case EquipType.WeaponTwoHanded:
                    break;

                case EquipType.WeaponDualWield:
                    break;

                // Bow / Crossbox
                case EquipType.Missile:
                    break;

                case EquipType.Shield:
                    break;

                case EquipType.Armor:
                    break;

                case EquipType.Helmet:
                    if (chr.IsMinotaur())
                    {
                        return false;
                    }
                    break;

                case EquipType.Boots:
                    if (chr.IsMinotaur())
                    {
                        return false;
                    }
                    break;

                case EquipType.Belt:
                    break;

                case EquipType.Cloak:
                    break;

                // Magnifying glass - Amulet, Gauntlets, 6 Rings
                case EquipType.Ring:
                    break;

                case EquipType.Amulet:
                    break;

                case EquipType.Gauntlets:
                    break;

                default:
                    break;
            }

            return false;
        }

        // Returned position is relative to its parent - right hand's "fingers"
        static public Vector2 GetRightHandItemPos(BaseItem item, Character chr)
        {
            Vector2 pos = new Vector2();

            pos.x = item.Data.EquipX;
            pos.y = item.Data.EquipY - item.Data.InvSprite.rect.height;

            return pos;
        }

        // Sorry but there is really no other way other than hard-coding this..
        // this cannot be computed from any available values
        // Armor, Helmet, Belt, Boots
        static public Vector2 GetArmorItemPos(BaseItem item, Character chr)
        {
            Vector2 pos = new Vector2();

            var eqSpritePattern = "v2";
            if (item.Data.EquipType == EquipType.WeaponOneHanded && chr.Inventory.RightHandSlot != null)
            {
                eqSpritePattern = "v2a";
            }
            Sprite equipSprite = item.Data.EquipSprites.Find(sprite => sprite.name.Contains("v2a"));
            if (equipSprite == null)
            {
                equipSprite = item.Data.EquipSprites[0];
            }

            // This is by default - armor at the bottom
            pos.y = equipSprite.rect.height / 2;

            // bottom+center
            if (chr.IsFemale())
            {
                switch (item.Data.Id)
                {
                    case 84: pos.x = 2; break;
                    case 85: pos.x = -3; break;
                    case 86: pos.x = -1; break;
                    case 87: pos.x = -10; break;
                    case 88: pos.x = -5; pos.y = 187; break;
                    case 89: pos.x = -2.5f; break;
                    case 90: pos.x = -5; break;
                    case 91: pos.x = -3.5f; break;
                    case 92: pos.x = -5; pos.y = 190; break;
                    case 93: pos.x = -7; break;
                    case 94: pos.x = -7; break;
                    case 95: pos.x = -2; pos.y = 192; break;
                    case 96: pos.x = 0; pos.y = 187.5f; break;
                    case 97: pos.x = 0; break;
                    case 98: pos.x = -5; break;
                    case 251: pos.x = -5; break;
                    case 252: pos.x = -5; break;
                    case 253: pos.x = -7; break;
                    // Helmets
                    case 109: pos.Set(-8, 285); break;
                    case 110: pos.Set(-8, 277); break;
                    case 111: pos.Set(-8, 273); break;
                    case 112: pos.Set(-10, 283); break;
                    case 113: pos.Set(-8, 284); break;
                    case 114: pos.Set(-8, 288); break;
                    case 115: pos.Set(-9, 295); break;
                    case 116: pos.Set(-8, 287); break;
                    case 256: pos.Set(-8, 284); break;
                    case 257: pos.Set(-8, 283); break;
                    case 258: pos.Set(-13, 307.5f); break;
                    // Belts
                    case 117: pos.Set(-6, 176); break;
                    case 118: pos.Set(-6, 176); break;
                    case 119: pos.Set(-6, 176); break;
                    case 120: pos.Set(-6, 176); break;
                    case 121: pos.Set(-6, 176); break;
                    case 259: pos.Set(-6, 176); break;
                    // Cloaks - skipper for now, TODO
                    // 122-126
                    // Boots - posY = height / 2 - auto calculated
                    case 132: pos.x = -6; break;
                    case 133: pos.x = -6; break;
                    case 134: pos.x = -8; break;
                    case 135: pos.x = -8; break;
                    case 136: pos.x = -8; break;
                    case 262: pos.x = 0; break;

                    default:
                        Debug.LogError("Unhandled armor item ID: " + item.Data.Id);
                        break;
                }

                if (chr.Data.CharacterType == CharacterType.LichFemale_1)
                {
                    pos.y += 9.0f;
                }
            }
            else if (chr.IsMale())
            {
                switch (item.Data.Id)
                {
                    default:
                        Debug.LogError("Unhandled armor item ID: " + item.Data.Id);
                        break;
                }
            }
            else if (chr.IsMinotaur())
            {
                switch (item.Data.Id)
                {
                    default:
                        Debug.LogError("Unhandled armor item ID: " + item.Data.Id);
                        break;
                }
            }
            else if (chr.IsTroll())
            {
                switch (item.Data.Id)
                {
                    default:
                        Debug.LogError("Unhandled armor item ID: " + item.Data.Id);
                        break;
                }
            }

            return pos;
        }
    }
}
