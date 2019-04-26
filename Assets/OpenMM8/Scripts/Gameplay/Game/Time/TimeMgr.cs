using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.OpenMM8.Scripts.Gameplay.Data;
using Assets.OpenMM8.Scripts.Data;


namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class Timer
    {
        public string Name = "MM8 Timer";
        public long DelayMinutes; // When first timer event will be invoked
        public int IntervalInMinutes = 0; // Interval of occurance after first "DelayMInutes" passed
        public GameTime StartTime = new GameTime(0);
        public Action<Timer, GameTime> OnTimer;
    }

    public class TimeMgr : Singleton<TimeMgr>
    {
        //=================================== Member Variables ===================================

        public const int REALTIME_SECONDS_TO_INGAME_MINUTE = 2;
        public const int START_YEAR = 1172;

        public GameTime CurrentTime = new GameTime(0);
        private GameTime m_StartTime = new GameTime(0);

        private float m_MinutesSinceStart = 0;
        private List<Timer> m_Timers = new List<Timer>();

        private float m_RealtimeSecondsElapsed = 0.0f;

        private float m_GameSecondsElapsed = 0.0f;

        //=================================== Unity Lifecycle ===================================

        private void Awake()
        {
            /*m_MinutesSinceStart = m_StartMinutes;
            RecalcCurrentTime();*/
        }

        public bool Init()
        {
            /*Timer t = new Timer();
            t.Delay = 5;
            t.IntervalInMinutes = 1;
            t.OnTimer = (Timer timer, TimeInfo currTime) => { Debug.Log("On Timer Callback"); };
            AddTimer(t);*/

            // TODO: 
            m_StartTime = new GameTime(0, 0, 9);
            CurrentTime = new GameTime(m_StartTime);

            return true;
        }

        public void Update()
        {
            m_RealtimeSecondsElapsed += Time.deltaTime;

            CurrentTime.GameSeconds = m_StartTime.GameSeconds + (long)(m_RealtimeSecondsElapsed / 2 * 60);

            UpdateTimers();
        }

        //=================================== Methods ===================================

        public void AddMinutes(int num)
        {
            m_StartTime.GameSeconds += 60 * num;
            CurrentTime.GameSeconds += 60 * num;
        }

        public GameTime GetCurrentTime()
        {
            return CurrentTime;
        }

        public void AddTimer(Timer timer)
        {
            timer.StartTime = new GameTime(CurrentTime);
            m_Timers.Add(timer);
        }

        public void RemoveTimer(Timer timer)
        {
            m_Timers.Remove(timer);
        }

        private void UpdateTimers()
        {
            foreach (Timer timer in m_Timers)
            {
                if (CurrentTime.GetMinutes() >= (timer.StartTime.GetMinutes() + timer.DelayMinutes))
                {
                    timer.OnTimer(timer, CurrentTime);

                    if (timer.IntervalInMinutes > 0)
                    {
                        timer.StartTime.GameSeconds = CurrentTime.GameSeconds;
                        timer.DelayMinutes = timer.IntervalInMinutes;
                    }
                }
            }

            m_Timers.RemoveAll(t => m_MinutesSinceStart >= (t.StartTime.GetMinutes() + t.DelayMinutes) && t.IntervalInMinutes == 0);
        }
    }
}
