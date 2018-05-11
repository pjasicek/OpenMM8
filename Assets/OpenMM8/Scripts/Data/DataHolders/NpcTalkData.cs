using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Data
{
    public class NpcTalkData : DbData
    {
        public string Name;
        public int PictureId;
        public int GreetId;
        public List<int> TopicList = new List<int>();
    }
}
