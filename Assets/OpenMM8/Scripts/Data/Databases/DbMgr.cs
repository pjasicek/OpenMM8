﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.OpenMM8.Scripts.Gameplay.Data;
using LINQtoCSV;
using Assets.OpenMM8.Scripts.Data;
using System.Diagnostics;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class DbMgr : Singleton<DbMgr>
    {
        public ItemDb ItemDb = new ItemDb();
        public NpcDb MonsterDb = new NpcDb();
        public NpcGreetDb NpcGreetDb = new NpcGreetDb();
        public NpcTextDb NpcTextDb = new NpcTextDb();
        public NpcTopicDb NpcTopicDb = new NpcTopicDb();
        public NpcNewsDb NpcNewsDb = new NpcNewsDb();
        public QuestDb QuestDb = new QuestDb();
        public NpcTalkDb NpcTalkDb = new NpcTalkDb();
        public BuildingDb BuildingDb = new BuildingDb();
        public ItemEquipPosDb ItemEquipPosDb = new ItemEquipPosDb();
        public ObjectDisplayDb ObjectDisplayDb = new ObjectDisplayDb();

        private const string MM8_DATA_PATH = @"Data/";

        private void Awake()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            /*ItemDb.Initialize(MM8_DATA_PATH + @"ItemData.csv");
            NpcDb.Initialize(MM8_DATA_PATH + @"MonsterNpcData.csv");
            NpcGreetDb.Initialize(MM8_DATA_PATH + @"NpcGreet.csv");
            NpcTextDb.Initialize(MM8_DATA_PATH + @"NpcText.csv");
            NpcTopicDb.Initialize(MM8_DATA_PATH + @"NpcTopic.csv");
            NpcNewsDb.Initialize(MM8_DATA_PATH + @"NpcNews.csv");
            QuestDb.Initialize(MM8_DATA_PATH + @"Quests.csv");
            NpcTalkDb.Initialize(MM8_DATA_PATH + @"NpcTalkData.csv");*/

            ItemDb.Initialize(MM8_DATA_PATH + @"ITEMS", 2);
            MonsterDb.Initialize(MM8_DATA_PATH + @"MONSTERS", 2);
            NpcGreetDb.Initialize(MM8_DATA_PATH + @"NPC_GREET");
            NpcTextDb.Initialize(MM8_DATA_PATH + @"NPC_TOPIC_TEXT");
            NpcTopicDb.Initialize(MM8_DATA_PATH + @"NPC_TOPIC");
            NpcNewsDb.Initialize(MM8_DATA_PATH + @"NPC_NEWS");
            QuestDb.Initialize(MM8_DATA_PATH + @"QUESTS");
            NpcTalkDb.Initialize(MM8_DATA_PATH + @"NPC", 2);
            BuildingDb.Initialize(MM8_DATA_PATH + @"BUILDINGS");
            ItemEquipPosDb.Initialize(MM8_DATA_PATH + @"ITEM_EQUIP_POS");
            ObjectDisplayDb.Initialize(MM8_DATA_PATH + @"OBJLIST");

            stopwatch.Stop();
            UnityEngine.Debug.Log("elapsed ms: " + stopwatch.ElapsedMilliseconds);

            //NpcGreetDb.Initialize(MM8_DATA_PATH + @"NpcGreet.csv");

            /*CsvFileDescription inputFileDescription = new CsvFileDescription
            {
                SeparatorChar = '\t',
                FirstLineHasColumnNames = true,
                TextEncoding = Encoding.UTF8
            };
            CsvContext cc = new CsvContext();
            IEnumerable<test> products =
                cc.Read<test>(MM8_DATA_PATH + @"npcgreet.txt", inputFileDescription);

            foreach (test t in products)
            {
                Debug.Log("1: " + t.Id + ", 2: " + t.Greeting1 + ", 3: " + t.Greeting2);
            }*/
        }

        public bool Init()
        {
            return true;
        }
    }
}
