using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenMM8.Scripts
{
    public class Logger
    {
        static public void LogDebug(string text)
        {
            Debug.Log(text);
        }

        static public void LogError(string text)
        {
            Debug.LogError(text);
        }
    }
}
