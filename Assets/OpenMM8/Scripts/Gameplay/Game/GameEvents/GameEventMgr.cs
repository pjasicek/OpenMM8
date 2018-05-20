using Assets.OpenMM8.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class GameEventMgr : Singleton<GameEventMgr>
    {
        //=================================== Member Variables ===================================

        // Dagger Wound Island = 1
        private int m_CurrentMap = 1;

        private Dictionary<int, MapEventProcessor> m_MapEventProcessorMap =
            new Dictionary<int, MapEventProcessor>();

        //=================================== Unity Lifecycle ===================================

        void Awake()
        {
            m_MapEventProcessorMap.Add(1, new EP_DaggerWoundIsland());
        }

        public bool Init()
        {
            m_MapEventProcessorMap[m_CurrentMap].Init();

            return true;   
        }


        //=================================== Methods ===================================

        public void ProcessGameEvent(int evt)
        {
            if (!m_MapEventProcessorMap.ContainsKey(m_CurrentMap))
            {
                Debug.LogError("No map processor for Map ID: " + m_CurrentMap + " is available.");
                return;
            }

            m_MapEventProcessorMap[m_CurrentMap].ProcessEvent(evt);
        }

        //=================================== Events ===================================
    }
}
