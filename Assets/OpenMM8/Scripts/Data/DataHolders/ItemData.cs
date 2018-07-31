using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class ItemData : DbData
    {
        public string ImageName;
        public string Name;
        public int GoldValue;
        public EquipType EquipType;
        public SkillGroup SkillGroup;
        public string Mod1;
        public string Mod2;
        public string Material;
        public int QualityLevel;
        public string NotIdentifiedName;
        public int SpriteIndex;
        public string VarA;
        public string VarB;
        public int EquipX;
        public int EquipY;
        public string Notes;

        // Unity specific but common for all items with this specific data
        // This is set up upon UiMgr initialization
        public Sprite InvSprite;
        public List<Sprite> EquipSprites = new List<Sprite>();
        public Vector2Int InvSize;
    }
}
