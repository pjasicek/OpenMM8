﻿using UnityEngine;
using System.Collections;

using Assets.OpenMM8.Scripts.Gameplay;

public class MinimapMarker : MonoBehaviour
{
    private Color _Color;
    public Color Color
    {
        get
        {
            return _Color;
        }
        set
        {
            _Color = value;
            if (TextureMark != null)
            {
                TextureMark.SetPixel(0, 0, _Color);
                TextureMark.Apply();
            }
        }
    }
    public Texture2D TextureMark;
    public Rect DrawRect = new Rect();

    void Start()
    {
        TextureMark = new Texture2D(1, 1);
        StartCoroutine(LateStart());
    }

    public IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        TextureMark.SetPixel(0, 0, Color);
        TextureMark.Apply();
        //UiMgr.Instance.Minimap.MinimapMarkers.Add(this);

        GameEvents.InvokeEvent_OnMinimapMarkerCreated(this);
    }

    private void OnDestroy()
    {
        GameEvents.InvokeEvent_OnMinimapMarkerDestroyed(this);

        /*if (UiMgr.Instance)
        {
            UiMgr.Instance.Minimap.MinimapMarkers.Remove(this);
        }*/
    }
}
