using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class TalkScene
    {
        public string Location;
        public List<NpcTalkProperties> TalkProperties = new List<NpcTalkProperties>();
        public VideoScene VideoScene;
        public bool IsBuilding;
    }
}
