using Assets.OpenMM8.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public enum QuestState
    {
        Invalid = -1,
        NotTaken,
        InProgress,
        Completed,
        Failed
    }

    public class Quest
    {
        public QuestData Data;
        public QuestState State = QuestState.Invalid;
    }
}
