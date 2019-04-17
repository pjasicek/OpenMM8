using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class DollUI
    {
        public int EquipDisplayId = 1;

        public GameObject Holder;
        public Image BackgroundImage;
        public Image LH_OpenImage;
        public Image LH_ClosedImage;
        public Image LH_HoldImage;
        public Image RH_OpenImage;
        public Image RH_HoldImage;
        public GameObject RH_WeaponAnchorHolder;
        //public Image RH_HoldFingersImage;
        public Image BodyImage;

        public InventoryItem Cloak;
        public InventoryItem Bow;
        public InventoryItem Armor;
        public InventoryItem Boots;
        public InventoryItem Helmet;
        public InventoryItem Belt;
        public InventoryItem RH_Weapon;
        public InventoryItem LH_Weapon;

        public GameObject AccessoryBackgroundHolder;
        public InventoryItem Ring_1;
        public InventoryItem Ring_2;
        public InventoryItem Ring_3;
        public InventoryItem Ring_4;
        public InventoryItem Ring_5;
        public InventoryItem Ring_6;
        public InventoryItem Gauntlets;
        public InventoryItem Necklace;
    }
}
