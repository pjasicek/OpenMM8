using Assets.OpenMM8.Scripts.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Data
{
    public class StartingStatsData : DbData<CharacterRace>
    {
        // int Id = Race
        public Dictionary<CharAttribute, int> MinStats = new Dictionary<CharAttribute, int>();
        public Dictionary<CharAttribute, int> MaxStats = new Dictionary<CharAttribute, int>();
        public Dictionary<CharAttribute, float> Gain = new Dictionary<CharAttribute, float>();
    }
}
