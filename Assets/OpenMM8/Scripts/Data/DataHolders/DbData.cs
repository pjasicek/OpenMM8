using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts
{
    public abstract class DbData<KeyType>
    {
        public KeyType Id;
    }

    // Default database data - Key is int
    public abstract class DbData : DbData<int>
    {
    }
}
