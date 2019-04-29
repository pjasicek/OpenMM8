using Assets.OpenMM8.Scripts;
using Assets.OpenMM8.Scripts.Gameplay;
using Assets.OpenMM8.Scripts.Gameplay.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;


// TODO: Think of some more explaining word for this class / "OpenMM8_SpriteAnimation" class
//       since not all of these entries are animations ... some have just a single sprite
//
// All spriesheets used for this HAVE TO be in "Resources/Sprites/OriginalSprites" !!!
public class SpriteRegistry
{
    static public Dictionary<string, OpenMM8_SpriteAnimation> SpriteAnimationMap = 
        new Dictionary<string, OpenMM8_SpriteAnimation>();

    // WHAT TO DO WITH THE ONE IN UIMGR
    static public Dictionary<string, Sprite> SpriteMap = new Dictionary<string, Sprite>();

    static private List<string> m_LoadedSpritesheets = new List<string>();

    static public OpenMM8_SpriteAnimation GetSpriteAnimation(string name, string spritesheetName = "")
    {
        name = name.ToLower();

        OpenMM8_SpriteAnimation anim = null;
        if (SpriteAnimationMap.TryGetValue(name, out anim))
        {
            return anim;
        }
        else
        {
            // Load it and cache it
            SpriteAnimationData dbData = DbMgr.Instance.SpriteAnimationDb.Get(name);
            Assert.IsTrue(dbData != null);
            Assert.IsTrue(dbData.AnimFrameNames.Count > 0);

            if (spritesheetName != "")
            {
                LoadSpritesheet(spritesheetName);
            }

            // Try to look for first sprite
            string spriteToTest = dbData.AnimFrameNames[0];
            bool hasRotatedSpriteVariations = false;

            // Try to look for both regulat and "Front" facing sprite which ends with "0"
            if (!SpriteMap.ContainsKey(spriteToTest))
            {
                spriteToTest += "0";
                if (!SpriteMap.ContainsKey(spriteToTest))
                {
                    Debug.LogError("Cannot load spriteAnimation " + name + " (" + spriteToTest + ")" +
                        ": Required sprites are not loaded");
                    return null;
                }
                else
                {
                    hasRotatedSpriteVariations = true;
                }
            }

            // Here we assume all required sprites are available in our cache (@this.SpriteMap)
            anim = new OpenMM8_SpriteAnimation();
            anim.Name = name;
            anim.TotalAnimationLengthSeconds = dbData.TotalAnimationLengthSeconds;
            anim.Scale = dbData.Scale;
            anim.IsAlwaysFacingCamera = dbData.IsAlwaysLookingFront;
            anim.IsAnimated = dbData.AnimFrameNames.Count > 1;
            anim.FrameDurationsSeconds = dbData.AnimFrameLengths.ToArray();

            if (hasRotatedSpriteVariations)
            {
                // From Sprite's perspective:
                // 0 = front
                // 1 = front left
                // 2 = left
                // 3 = back left
                // 4 = back

                foreach (string spriteFrameBaseName in dbData.AnimFrameNames)
                {
                    anim.FrontSprites.Add(SpriteMap[spriteFrameBaseName + "0"]);
                    anim.FrontLeftSprites.Add(SpriteMap[spriteFrameBaseName + "1"]);
                    anim.LeftSprites.Add(SpriteMap[spriteFrameBaseName + "2"]);
                    anim.BackLeftSprites.Add(SpriteMap[spriteFrameBaseName + "3"]);
                    anim.BackSprites.Add(SpriteMap[spriteFrameBaseName + "4"]);
                }
            }
            else
            {
                foreach (string spriteName in dbData.AnimFrameNames)
                {
                    anim.FrontSprites.Add(SpriteMap[spriteName]);
                }

                anim.BackSprites = null;
                anim.BackLeftSprites = null;
                anim.LeftSprites = null;
                anim.FrontLeftSprites = null;
            }

            SpriteAnimationMap.Add(name, anim);
            return anim;
        }

        return null;
    }

    static public bool LoadSpriteAnimation(string animationName)
    {
        SpriteAnimationData dbData = DbMgr.Instance.SpriteAnimationDb.Get(animationName);
        Assert.IsTrue(dbData != null);

        return false;
    }

    static public void LoadSpritesheet(string spritesheetName)
    {
        if (m_LoadedSpritesheets.Contains(spritesheetName))
        {
            Debug.LogError(spritesheetName + " was already loaded !");
            return;
        }

        OpenMM8Util.AppendResourcesToMap(SpriteMap, "Sprites/SpritesOriginal/" + spritesheetName);
        m_LoadedSpritesheets.Add(spritesheetName);
    }

    // I really should not care that much about memory - even if it takes up 1GB its fine these days
    static public void UnloadAnimation(string name)
    {
        SpriteAnimationMap.Remove(name);
    }
}
