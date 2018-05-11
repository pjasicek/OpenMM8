using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Data
{
    public class NpcGreetData : DbData
    {
        public string Greeting1 = "";
        public string Greeting2 = "";
        public string Note = "";
        public string Owner = "";

        public NpcGreetData(int id, string greeting1, string greeting2, string note, string owner)
        {
            Id = id;
            Greeting1 = greeting1;
            Greeting2 = greeting2;
            Note = note;
            Owner = owner;
        }
    }
}
