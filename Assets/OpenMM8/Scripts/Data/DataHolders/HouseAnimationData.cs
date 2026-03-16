using System.Collections.Generic;

namespace Assets.OpenMM8.Scripts.Data
{
    public class HouseAnimationData : DbData
    {
        public int AnimationId;
        public string BuildingName;
        public List<int> NpcsInsideList = new List<int>();
        public string VideoResourcePath;
        public string EnterSoundResourcePath;
    }
}
