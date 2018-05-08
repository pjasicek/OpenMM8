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
        public NpcGreetDb NpcGreetDb = new NpcGreetDb();
        public NpcTextDb NpcTextDb = new NpcTextDb();
        public NpcTopicDb NpcTopicDb = new NpcTopicDb();
        public NpcNewsDb NpcNewsDb = new NpcNewsDb();
        public QuestDb QuestDb = new QuestDb();

        private const string MM8_DATA_PATH = @"Assets/OpenMM8/Resources/Data/";

        private void Awake()
        {
            ItemDb.Initialize(MM8_DATA_PATH + @"ItemData.csv");
            NpcDb.Initialize(MM8_DATA_PATH + @"MonsterNpcData.csv");
            NpcGreetDb.Initialize(MM8_DATA_PATH + @"NpcGreet.csv");
            NpcTextDb.Initialize(MM8_DATA_PATH + @"NpcText.csv");
            NpcTopicDb.Initialize(MM8_DATA_PATH + @"NpcTopic.csv");
            NpcNewsDb.Initialize(MM8_DATA_PATH + @"NpcNews.csv");
            QuestDb.Initialize(MM8_DATA_PATH + @"Quests.csv");
        }

        public bool Init()
        {
            return true;
        }
    }
}
