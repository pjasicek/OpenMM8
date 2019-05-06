using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class SpriteObject
{
    public string Name;
    public float TotalAnimationLengthSeconds;
    public float Scale;
    public bool IsAlwaysFacingCamera;
    public bool IsAnimated;

    // This is to be as much cache-friendly as possible
    public List<Sprite> FrontSprites = new List<Sprite>();
    public List<Sprite> BackSprites = new List<Sprite>();
    public List<Sprite> BackLeftSprites = new List<Sprite>();
    public List<Sprite> LeftSprites = new List<Sprite>();
    public List<Sprite> FrontLeftSprites = new List<Sprite>();

    // To be completely honest... I am not sure if there are animations in which
    // different frames have different duration
    public float[] FrameDurationsSeconds;
}