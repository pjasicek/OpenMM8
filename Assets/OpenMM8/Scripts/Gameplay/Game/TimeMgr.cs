using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.OpenMM8.Scripts.Gameplay.Data;
using Assets.OpenMM8.Scripts.Data;


namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class TimeInfo
    {
        public int Minute;
        public int Hour;
        public int Day;
        public int Month;
        public int Year;
        public DayOfWeek DayOfWeek;
    }

    public class Timer
    {
        public string Name = "MM8 Timer";
        public int Delay;
        public int IntervalInMinutes = 0;
        public int StartTime;
        public Action<Timer, TimeInfo> OnTimer;
    }

    public class TimeMgr : Singleton<TimeMgr>
    {
        //=================================== Member Variables ===================================

        public const int YEAR_IN_MINUTES = 365 * 24 * 60;
        public const int MONTH_IN_MINUTES = 30 * 24 * 60;
        public const int DAY_IN_MINUTES = 24 * 60;
        public const int HOUR_IN_MINUTES = 60;

        private const int REALTIME_SECOND_TO_INGAME_MINUTES = 2;

        public TimeInfo CurrentTime = new TimeInfo();

        private float m_MinutesSinceStart = 0;
        private const int m_StartMinutes = 9 * HOUR_IN_MINUTES;
        private const int m_StartYear = 1172;
        private List<Timer> m_Timers = new List<Timer>();

        //=================================== Unity Lifecycle ===================================

        private void Awake()
        {
            m_MinutesSinceStart = m_StartMinutes;
            RecalcCurrentTime();
        }

        public bool Init()
        {
            /*Timer t = new Timer();
            t.Delay = 5;
            t.IntervalInMinutes = 1;
            t.OnTimer = (Timer timer, TimeInfo currTime) => { Debug.Log("On Timer Callback"); };
            AddTimer(t);*/

            return true;
        }

        public void Update()
        {
            float elapsedMinutes = Time.deltaTime / REALTIME_SECOND_TO_INGAME_MINUTES;

            int prev = (int)m_MinutesSinceStart;
            m_MinutesSinceStart += elapsedMinutes;
            int curr = (int)m_MinutesSinceStart;

            if (prev != curr)
            {
                RecalcCurrentTime();
            }

            UpdateTimers();
        }

        //=================================== Methods ===================================

        public void AddMinutes(int num)
        {
            m_MinutesSinceStart += num;
            RecalcCurrentTime();
        }

        public TimeInfo GetCurrentTime()
        {
            return CurrentTime;
        }

        private void RecalcCurrentTime()
        {
            int mins = (int)m_MinutesSinceStart;

            int dayOfWeek = ((mins / DAY_IN_MINUTES) % 7) + 1;
            if (dayOfWeek == 7)
            {
                dayOfWeek = 0;
            }

            int elapsedYears = mins / YEAR_IN_MINUTES;
            mins = mins % YEAR_IN_MINUTES;

            int elapsedMonths = mins / MONTH_IN_MINUTES;
            mins = mins % MONTH_IN_MINUTES;

            int elapsedDays = mins / DAY_IN_MINUTES;
            mins = mins % DAY_IN_MINUTES;

            int elapsedHours = mins / HOUR_IN_MINUTES;
            mins = mins % HOUR_IN_MINUTES;

            int elapsedMinutes = mins;

            CurrentTime.Year = elapsedYears + m_StartYear;
            CurrentTime.Month = elapsedMonths + 1;
            CurrentTime.Day = elapsedDays + 1;
            CurrentTime.Hour = elapsedHours;
            CurrentTime.Minute = elapsedMinutes;
            CurrentTime.DayOfWeek = (DayOfWeek)dayOfWeek;

            /*string timeStr = "Year: " + CurrentTime.Year + ", Month: " + CurrentTime.Month +
                ", Day: " + CurrentTime.Day + ", Hour: " + CurrentTime.Hour + 
                " Minute: " + CurrentTime.Minute + ", Weekday: " + CurrentTime.DayOfWeek;
            Debug.Log("Time... " + timeStr);*/
        }

        public void AddTimer(Timer timer)
        {
            timer.StartTime = (int)m_MinutesSinceStart;
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
                if (m_MinutesSinceStart >= (timer.StartTime + timer.Delay))
                {
                    timer.OnTimer(timer, CurrentTime);

                    if (timer.IntervalInMinutes > 0)
                    {
                        timer.StartTime = (int)m_MinutesSinceStart;
                        timer.Delay = timer.IntervalInMinutes;
                    }
                }
            }

            m_Timers.RemoveAll(t => m_MinutesSinceStart >= (t.StartTime + t.Delay) && t.IntervalInMinutes == 0);
        }
    }
}
