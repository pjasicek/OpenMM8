using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public enum AwardType
    {
        NormalQuest,
        TransferQuest
    }

    class Award
    {
        public AwardType Type;
        public string Description;
    }
}
