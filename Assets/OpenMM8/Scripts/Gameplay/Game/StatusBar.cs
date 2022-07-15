using Assets.OpenMM8.Scripts.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public enum StatusOverrideType
{
    OverrideExisting,
    DoNotOverrideExisting,
}

public class StatusTextBar
{
    public Text TargetText;
    public float LastUpdateRealtime = 0.0f;
    public float CurrTextDuration = 0.0f;

    public void DoUpdate()
    {
        if (TargetText.text == "")
        {
            return;
        }

        if (Time.realtimeSinceStartup > (LastUpdateRealtime + CurrTextDuration))
        {
            TargetText.text = "";
        }
    }

    public void SetText(string text, bool overrideExisting = true, float duration = 2.0f)
    {
        if (TargetText.text != "" && !overrideExisting)
        {
            return;
        }

        LastUpdateRealtime = Time.realtimeSinceStartup;
        CurrTextDuration = duration;
        TargetText.text = text;
    }
}
