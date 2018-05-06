using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.OpenMM8.Scripts.Gameplay.Data;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class QuestMgr : Singleton<DbMgr>
    {

        private void Awake()
        {
            
        }

        public bool Init()
        {
            return true;
        }
    }
}
