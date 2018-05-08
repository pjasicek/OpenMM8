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


    public class QuestMgr : Singleton<DbMgr>
    {
        //=================================== Member Variables ===================================

        private Dictionary<int, Quest> m_QuestMap = new Dictionary<int, Quest>();

        //=================================== Unity Lifecycle ===================================

        private void Awake()
        {
            
        }

        public bool Init()
        {
            foreach (var qDataPair in DbMgr.Instance.QuestDb.QuestDataMap)
            {
                Quest q = new Quest();
                q.Data = qDataPair.Value;
                q.State = QuestState.NotTaken;

                m_QuestMap.Add(q.Data.Id, q);
            }

            return true;
        }

        //=================================== Methods ===================================

        public QuestState GetQuestState(int questId)
        {
            if (m_QuestMap.ContainsKey(questId))
            {
                return m_QuestMap[questId].State;
            }

            return QuestState.Invalid;
        }

        public Quest GetQuest(int questId)
        {
            if (m_QuestMap.ContainsKey(questId))
            {
                return m_QuestMap[questId];
            }

            return null;
        }

        public bool IsQuestFinished(int questId)
        {
            Quest q = GetQuest(questId);
            if (q != null)
            {
                return q.State == QuestState.Completed;
            }

            return false;
        }

        public bool TryFinishQuest(int questId)
        {
            Quest q = GetQuest(questId);
            if (q != null)
            {
                // TODO: Add logic
                return false;
            }

            return false;
        }

        public bool TryStartQuest(int questId)
        {
            Quest q = GetQuest(questId);
            if (q != null)
            {
                // TODO: Add logic
                return false;
            }

            return false;
        }

        //=================================== Events ===================================
    }
}
