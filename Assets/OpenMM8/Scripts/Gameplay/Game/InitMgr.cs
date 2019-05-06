using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    

    class InitMgr : MonoBehaviour
    {
        private void Awake()
        {
            DbMgr.Instance.Init();
        }

        private void Start()
        {
            
            ItemGenerator.Instance.Init();

            TimeMgr.Instance.Init();
            UiMgr.Instance.Init();
            GameCore.Instance.Init();
            SoundMgr.Instance.Init();
            QuestMgr.Instance.Init();
            GameEventMgr.Instance.Init();
            TalkEventMgr.Instance.Init();

            UiMgr.Instance.PostInit();
            GameCore.Instance.PostInit();

            GameEvents.InvokeEvent_OnInitComplete();
        }
    }
}
