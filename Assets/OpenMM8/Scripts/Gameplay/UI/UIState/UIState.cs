using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public partial class UiMgr
    {
        abstract public class UIState
        {
            abstract public bool EnterState(object stateArgs);
            abstract public void LeaveState();

            // Mapped action was pressed (e.g. Escape, LMB, M, etc)
            // Returns true if action was consumed
            abstract public bool OnActionPressed(string action);

            // Returns true if this state blocks game updates
            virtual public bool IsGameBlocking() { return true; }
        }
    }
}
