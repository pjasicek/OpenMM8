using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpriteAnimation : MonoBehaviour
{
    public Sprite[] AnimationSprites;
    public float AnimationTime = 1.0f;
    public bool HideAfterFinish = true;
    public bool Loop = false;

    private Image m_Image;
    private int m_CurrSpriteIdx = 0;
    private float m_AnimationSpeed;

    // Use this for initialization
    void Start()
    {
        m_Image = GetComponent<Image>();
        m_AnimationSpeed = AnimationTime / AnimationSprites.Length;
    }

    public void SetVisible(bool visible)
    {
        m_Image.enabled = visible;
    }

    public void Play()
    {
        m_CurrSpriteIdx = 0;
        StartCoroutine(DoAnimate());
    }

    public void Stop()
    {
        StopCoroutine(DoAnimate());
        m_CurrSpriteIdx = 0;
    }

    public void Pause()
    {
        StopCoroutine(DoAnimate());
    }

    public void Resume()
    {
        StartCoroutine(DoAnimate());
    }

    private void OnAnimationFinished()
    {
        if (Loop)
        {
            StartCoroutine(DoAnimate());
        }
        else
        {
            SetVisible(HideAfterFinish);
        }
    }

    private IEnumerator DoAnimate()
    {
        for (int i = m_CurrSpriteIdx; i < AnimationSprites.Length; i++)
        {
            m_Image.sprite = AnimationSprites[i];
            yield return new WaitForSeconds(m_AnimationSpeed);
        }
    }
}
