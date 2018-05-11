using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public delegate void InitComplete();

    class InitMgr : MonoBehaviour
    {
        static public event InitComplete OnInitComplete;

        private void Start()
        {
            DbMgr.Instance.Init();
            GameMgr.Instance.Init();
            UiMgr.Instance.Init();
            SoundMgr.Instance.Init();
            QuestMgr.Instance.Init();
            TalkMgr.Instance.Init();

            GameMgr.Instance.PostInit();

            if (OnInitComplete != null)
            {
                OnInitComplete();
            }
        }
    }
}
