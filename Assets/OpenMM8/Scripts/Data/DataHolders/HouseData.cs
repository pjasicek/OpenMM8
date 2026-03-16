using System;

namespace Assets.OpenMM8.Scripts.Data
{
    public class HouseData : DbData
    {
        public int LocalId;
        public string TypeName;
        public int MapId;
        public int AnimationId;
        public string Name;
        public string ProprietorName;
        public string ProprietorTitle;
        public int PictureId;
        public int State;
        public int Reputation;
        public int Personality;
        public float PriceMultiplier;
        public float SkillPriceMultiplier;
        public string ValueC;
        public int GenerationIntervalDays;
        public int OpenFrom;
        public int OpenTo;
        public int ExitPictureId;
        public int ExitMapId;
        public int RestrictionQuestBit;
        public string EnterText;
    }
}
