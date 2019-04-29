using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.OpenMM8.Scripts.Gameplay;
using Assets.OpenMM8.Scripts;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteBillboardAnimator : MonoBehaviour
{
    public string DefaultAnimation = "";
    public SpriteRenderer SpriteRenderer;

    public LookDirection LookDirection = LookDirection.Front;
    public OpenMM8_SpriteAnimation Animation = null;

    public List<Sprite> CurrentSprites = null;

    bool IsStopped = false;

    public float AnimationLength = 0.0f;
    public float AnimationTimePassed = 0.0f;

    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();

        GameEvents.OnInitComplete += OnPostInit;
    }

    // Use this for initialization
    void Start()
    {
        
    }

    public void OnDestroy()
    {
        GameEvents.OnInitComplete -= OnPostInit;
    }

    private void OnPostInit()
    {
        if (DefaultAnimation != "")
        {
            Animation = SpriteRegistry.GetSpriteAnimation(DefaultAnimation);
            SetAnimation(Animation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        if (Animation == null || IsStopped)
        {
            return;
        }

        if (CurrentSprites == null || CurrentSprites.Count == 0)
        {
            Debug.LogError("Null or empty current sprites: " + gameObject.name);
            return;
        }

        AnimationTimePassed += Time.deltaTime;
        if (AnimationTimePassed >= AnimationLength)
        {
            // Loop
            AnimationTimePassed = 0;
        }

        UpdateFrame();

        sw.Stop();
        //Debug.Log("Elapsed: " + sw.ElapsedMicroSeconds());
    }

    public void ResetAnimation()
    {
        IsStopped = false;
        AnimationTimePassed = 0.0f;
    }

    public void StopAnimation()
    {
        IsStopped = true;
        AnimationTimePassed = 0.0f;
    }

    public void StartAnimation()
    {
        IsStopped = false;
        AnimationTimePassed = 0.0f;
    }

    public void SetAnimation(OpenMM8_SpriteAnimation animation)
    {
        Animation = animation;
        AnimationLength = Animation.TotalAnimationLengthSeconds;
        ResetAnimation();
        UpdateFrame();
    }

    public void UpdateFrame()
    {
        if (Animation == null)
        {
            return;
        }

        if (Animation.IsAlwaysFacingCamera)
        {
            LookDirection = LookDirection.Front;
        }

        switch (LookDirection)
        {
            case LookDirection.Front:
                CurrentSprites = Animation.FrontSprites;
                break;
            case LookDirection.FrontRight:
                CurrentSprites = Animation.FrontLeftSprites;
                SpriteRenderer.flipX = true;
                break;
            case LookDirection.Right:
                CurrentSprites = Animation.LeftSprites;
                SpriteRenderer.flipX = true;
                break;
            case LookDirection.BackRight:
                CurrentSprites = Animation.BackLeftSprites;
                SpriteRenderer.flipX = true;
                break;
            case LookDirection.Back:
                CurrentSprites = Animation.BackSprites;
                SpriteRenderer.flipX = false;
                break;
            case LookDirection.BackLeft:
                CurrentSprites = Animation.BackLeftSprites;
                SpriteRenderer.flipX = false;
                break;
            case LookDirection.Left:
                CurrentSprites = Animation.LeftSprites;
                SpriteRenderer.flipX = false;
                break;
            case LookDirection.FrontLeft:
                CurrentSprites = Animation.FrontLeftSprites;
                SpriteRenderer.flipX = false;
                break;
            default:
                break;
        }

        // TODO: Count time passed in each individual frame
        int totalFrames = CurrentSprites.Count;
        int currentFrame = 0;
        if (totalFrames > 1 && AnimationLength > 0.0f)
        {
            currentFrame = (int)((AnimationTimePassed / AnimationLength) * totalFrames);
        }

        SpriteRenderer.sprite = CurrentSprites[currentFrame];
    }

    public void SetLookDirection(LookDirection newLookDirection)
    {
        if (newLookDirection == LookDirection)
        {
            return;
        }

        if (Animation == null)
        {
            Debug.LogError("Null animation");
            return;
        }

        LookDirection = newLookDirection;

        UpdateFrame();
    }
}
