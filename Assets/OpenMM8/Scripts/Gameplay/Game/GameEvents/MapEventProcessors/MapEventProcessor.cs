using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    abstract public class MapEventProcessor
    {
        private List<Timer> m_Timers = new List<Timer>();

        protected void AddTimer(Timer t)
        {
            m_Timers.Add(t);
            TimeMgr.Instance.AddTimer(t);
        }

        virtual public void Init()
        {

        }

        virtual public void Shutdown()
        {
            foreach (Timer t in m_Timers)
            {
                TimeMgr.Instance.RemoveTimer(t);
            }

            m_Timers.Clear();
        }

        abstract public void ProcessEvent(int evtId);
    }
}
