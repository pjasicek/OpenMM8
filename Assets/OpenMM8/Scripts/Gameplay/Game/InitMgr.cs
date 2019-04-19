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
        private void Start()
        {
            DbMgr.Instance.Init();
            ItemGenerator.Instance.Init();

            TimeMgr.Instance.Init();
            GameMgr.Instance.Init();
            UiMgr.Instance.Init();
            SoundMgr.Instance.Init();
            QuestMgr.Instance.Init();
            GameEventMgr.Instance.Init();
            TalkEventMgr.Instance.Init();

            GameMgr.Instance.PostInit();
            UiMgr.Instance.PostInit();

            GameEvents.InvokeEvent_OnInitComplete();
        }
    }
}
