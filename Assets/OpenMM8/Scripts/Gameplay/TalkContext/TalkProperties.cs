using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    [System.Serializable]
    public class TalkProperties
    {
        public string Name;
        public Sprite Avatar;

        public bool IsNpcNews = false;
        public int GreetId = 0;
        public List<int> TopicIds = new List<int>();

        // State
        public bool IsVisited = false;
        public Stack<List<int>> NestedTopicIds = new Stack<List<int>>();
    }
}
