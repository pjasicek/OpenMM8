using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Data
{
    public class NpcTopicData
    {
        public int Id = -1;
        public string Topic = "";
        public int Requires = 0; // Looks like unused - always empty or 0
        public string Note = "";
        public int TextId = -1;
        public string Owner = "";
    }
}
