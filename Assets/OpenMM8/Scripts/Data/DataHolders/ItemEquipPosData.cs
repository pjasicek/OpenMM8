using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class ItemEquipPosData : DbData
    {
        public string Name;
        public Vector2Int MaleItemPos = new Vector2Int();
        public Vector2Int FemaleItemPos = new Vector2Int();
        public Vector2Int TrollItemPos = new Vector2Int();
        public Vector2Int MinotaurItemPos = new Vector2Int();
    }
}
