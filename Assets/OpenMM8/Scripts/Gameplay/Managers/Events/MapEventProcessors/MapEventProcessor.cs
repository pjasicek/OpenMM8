using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    abstract public class MapEventProcessor
    {
        abstract public void ProcessEvent(int evtId);
    }
}
