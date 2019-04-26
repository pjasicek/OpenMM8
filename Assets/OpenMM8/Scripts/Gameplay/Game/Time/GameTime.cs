using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class GameTime
    {
        public long GameSeconds;

        public GameTime(long seconds, long minutes = 0, long hours = 0, long days = 0,
            long weeks = 0, long months = 0, long years = 0)
        {
            GameSeconds = seconds +
                60 * minutes +
                3600 * hours +
                86400 * days +
                604800 * weeks +
                2419200 * months +
                29030400 * years;
        }

        public GameTime(GameTime other)
        {
            GameSeconds = other.GameSeconds;
        }

        static public GameTime FromCurrentTime(long additionalSeconds)
        {
            GameTime currTime = TimeMgr.Instance.GetCurrentTime();

            GameTime newTime = new GameTime(currTime);
            newTime.GameSeconds += additionalSeconds;

            return newTime;
        }

        public void AddMinutes(int minutes)
        {
            GameSeconds += 60 * minutes;
        }

        public void Reset()
        {
            GameSeconds = 0;
        }

        public bool IsValid()
        {
            return GameSeconds > 0;
        }

        public bool IsExpired()
        {
            Debug.Log(TimeMgr.Instance.GetCurrentTime().GetSeconds() + "/" + GameSeconds);
            return TimeMgr.Instance.GetCurrentTime().GetSeconds() > GameSeconds;
        }

        public long GetSeconds() { return GameSeconds; }
        public long GetMinutes() { return GetSeconds() / 60; }
        public long GetHours() { return GetMinutes() / 60; }
        public long GetDays() { return GetHours() / 24; }
        public long GetWeeks() { return GetDays() / 7; }
        public long GetMonths() { return GetWeeks() / 4; }
        public long GetYears() { return GetMonths() / 12; }

        public long GetSecondsFraction() { return GetSeconds() % 60; }
        public long GetMinutesFraction() { return (GetSeconds() / 60) % 60; }
        public long GetHoursOfDay() { return (GetSeconds() / 3600) % 24; }
        public long GetDayOfWeek() { return GetDays() % 7; }
        public long GetDayOfMonth() { return GetDays() % 28; }
        public long GetWeekOfMonth() { return GetWeeks() % 4; }
        public long GetMonthOfYear() { return GetMonths() % 12; }

        static public GameTime FromSeconds(long seconds)
        {
            return new GameTime(seconds, 0, 0, 0, 0, 0, 0);
        }
        static public GameTime FromMinutes(long minutes)
        {
            return new GameTime(0, minutes, 0, 0, 0, 0, 0);
        }
        static public GameTime FromHours(long hours)
        {
            return new GameTime(0, 0, hours, 0, 0, 0, 0);
        }
        static public GameTime FromDays(long days)
        {
            return new GameTime(0, 0, 0, days, 0, 0, 0);
        }
        static public GameTime FromYears(long years)
        {
            return new GameTime(0, 0, 0, 0, 0, 0, years);
        }
    }
}
