using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.OpenMM8.Scripts.Gameplay.Data;
using Assets.OpenMM8.Scripts.Data;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public delegate void QuestTaken(Quest quest);
    public delegate void QuestFinished(Quest quest);


    public class QuestMgr : Singleton<QuestMgr>
    {
        //=================================== Member Variables ===================================

        private Dictionary<int, Quest> m_QuestMap = new Dictionary<int, Quest>();

        //=================================== Unity Lifecycle ===================================

        private void Awake()
        {
            
        }

        public bool Init()
        {
            foreach (var qDataPair in DbMgr.Instance.QuestDb.Data)
            {
                Quest q = new Quest();
                q.Data = qDataPair.Value;
                q.QuestBit = 0;

                m_QuestMap.Add(q.Data.Id, q);
            }

            return true;
        }

        //=================================== Methods ===================================

        public bool IsQuestBitSet(int questId)
        {
            if (m_QuestMap.ContainsKey(questId))
            {
                return m_QuestMap[questId].QuestBit == 1;
            }
            else
            {
                Logger.LogError("Attempting to access nonexisting quest: " + questId);
            }

            return false;
        }

        public void SetQuestBit(int questId, int value)
        {
            if (m_QuestMap.ContainsKey(questId))
            {
                m_QuestMap[questId].QuestBit = value;
            }
            else
            {
                Logger.LogError("Attempting to access nonexisting quest: " + questId);
            }
        }
    }
}
