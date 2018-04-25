using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay.Managers
{
    class InitMgr : MonoBehaviour
    {
        InitMgr()
        {
            /*DbMgr.Instance.GetInstanceID();
            GameMgr.Instance.GetInstanceID();
            UiMgr.Instance.GetInstanceID();*/
        }

        private void Start()
        {
            DbMgr.Instance.Init();
            GameMgr.Instance.Init();
            UiMgr.Instance.Init();

            GameMgr.Instance.PostInit();
        }
    }
}
