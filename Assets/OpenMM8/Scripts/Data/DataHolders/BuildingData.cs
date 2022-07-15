using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Data
{
    public class BuildingData : DbData
    {
        public int MapId;
        public string BuildingName;
        public List<int> NpcsInsideList = new List<int>();
        public string VideoResourcePath;
        public string EnterSoundResourcePath;
        public int OpenFrom;
        public int OpenTo;
    }
}
