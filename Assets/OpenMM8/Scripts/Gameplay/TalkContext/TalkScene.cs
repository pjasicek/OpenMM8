using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class TalkScene
    {
        public int HouseId = -1;
        public string Location;
        public List<NpcTalkProperties> TalkProperties = new List<NpcTalkProperties>();
        public VideoScene VideoScene;
        public string VideoResourcePath;
        public bool IsBuilding;
        public IHouseService HouseService;
    }
}
