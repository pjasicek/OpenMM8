using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.OpenMM8.Scripts.Gameplay.Data;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class DbMgr : Singleton<DbMgr>
    {
        public ItemDb ItemDb = new ItemDb();
        public NpcDb NpcDb = new NpcDb();

        private const string MM8_DATA_PATH = @"Assets/OpenMM8/Resources/Data/";

        private void Awake()
        {
            ItemDb.Initialize(MM8_DATA_PATH + @"ItemData.csv");
            NpcDb.Initialize(MM8_DATA_PATH + @"MonsterNpcData.csv");
        }

        public bool Init()
        {
            return true;
        }
    }
}
