using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public enum NpcDialogueOfferType
    {
        None = 0,
        MasteryTeacher = 1
    }

    [System.Serializable]
    public class NpcDialogueOffer
    {
        public NpcDialogueOfferType OfferType = NpcDialogueOfferType.None;
        public int SourceTopicId = 0;
        public int MessageTextId = 0;
        public List<int> TopicIds = new List<int>();
    }

    [System.Serializable]
    public class NpcTalkProperties
    {
        // If this field is > 0 then the data will be loaded from the .CSV file
        // Otherwise it has to be specified by hand
        public int NpcId = -1;
        public int HouseId = -1;

        public string Name;
        public Sprite Avatar;

        public bool IsNpcNews = false;
        public int GreetId = 0;
        public List<int> TopicIds = new List<int>(5);

        public bool IsPresent = true;
        public bool HasGoodbyeMessage = false;

        // State
        public bool IsVisited = false;
        public Stack<List<int>> NestedTopicIds = new Stack<List<int>>();
        public Stack<string> RuntimeMenuIds = new Stack<string>();
        public NpcDialogueOffer CurrentOffer;

        [NonSerialized]
        public IHouseService HouseService;
    }
}
