using UnityEngine;
using System.Collections;
using Assets.OpenMM8.Scripts.Gameplay;
using UnityStandardAssets.Utility;
using Assets.OpenMM8.Scripts.Gameplay.Data;

public class OutdoorSpriteEffect : MonoBehaviour
{
    static public void Spawn(Vector3 position, Quaternion rotation, ObjectDisplayData displayData)
    {
        GameObject effectObject = (GameObject)Instantiate(Resources.Load("Prefabs/OutdoorSpriteEffect"));
        effectObject.transform.position = position;
        effectObject.transform.rotation = rotation;

        SpriteBillboardAnimator animator = effectObject.GetComponent<SpriteBillboardAnimator>();
        SpriteObject spriteObject = SpriteObjectRegistry.GetSpriteObject(displayData.SFTLabel);
        animator.SetAnimation(spriteObject);
        animator.Loop = false;

        //Debug.LogError("Lifetime: " + spriteObject.TotalAnimationLengthSeconds);
        //effectObject.GetComponent<OutdoorSpriteEffect>().SetLifetime(spriteObject.TotalAnimationLengthSeconds);
    }

    private void SetLifetime(float lifetime)
    {
        Invoke("DestroyNow", lifetime);
    }

    private void DestroyNow()
    {
        transform.DetachChildren();
        Destroy(gameObject);
    }
}
