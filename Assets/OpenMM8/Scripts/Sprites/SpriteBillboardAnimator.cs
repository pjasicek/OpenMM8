using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.OpenMM8.Scripts.Gameplay;
using Assets.OpenMM8.Scripts;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteBillboardAnimator : MonoBehaviour
{
    public string DefaultObject = "";
    public SpriteRenderer SpriteRenderer;

    public LookDirection LookDirection = LookDirection.Front;
    public SpriteObject SpriteObject = null;

    public List<Sprite> CurrentSprites = null;

    public bool Loop = true;
    public bool IsStopped = false;

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
        if (DefaultObject != "")
        {
            SpriteObject = SpriteObjectRegistry.GetSpriteObject(DefaultObject);
            SetAnimation(SpriteObject);
        }
    }

    public void OnDestroy()
    {
        GameEvents.OnInitComplete -= OnPostInit;
    }

    private void OnPostInit()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();*/

        if (SpriteObject == null || IsStopped)
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
            if (!Loop)
            {
                AnimationTimePassed -= Time.deltaTime;
                IsStopped = true;
            }
            else
            {
                // Loop
                AnimationTimePassed = 0;
            }
        }

        UpdateFrame();

        //sw.Stop();
        //Debug.Log("Elapsed: " + sw.ElapsedMicroSeconds());
    }

    public void ResetAnimation()
    {
        Loop = true;
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

    public void SetAnimation(SpriteObject animation)
    {
        SpriteObject = animation;
        AnimationLength = SpriteObject.TotalAnimationLengthSeconds;
        ResetAnimation();
        UpdateFrame();
    }

    public void UpdateFrame()
    {
        if (SpriteObject == null)
        {
            return;
        }

        if (SpriteObject.IsAlwaysFacingCamera)
        {
            LookDirection = LookDirection.Front;
        }

        switch (LookDirection)
        {
            case LookDirection.Front:
                CurrentSprites = SpriteObject.FrontSprites;
                break;
            case LookDirection.FrontRight:
                CurrentSprites = SpriteObject.FrontLeftSprites;
                SpriteRenderer.flipX = true;
                break;
            case LookDirection.Right:
                CurrentSprites = SpriteObject.LeftSprites;
                SpriteRenderer.flipX = true;
                break;
            case LookDirection.BackRight:
                CurrentSprites = SpriteObject.BackLeftSprites;
                SpriteRenderer.flipX = true;
                break;
            case LookDirection.Back:
                CurrentSprites = SpriteObject.BackSprites;
                SpriteRenderer.flipX = false;
                break;
            case LookDirection.BackLeft:
                CurrentSprites = SpriteObject.BackLeftSprites;
                SpriteRenderer.flipX = false;
                break;
            case LookDirection.Left:
                CurrentSprites = SpriteObject.LeftSprites;
                SpriteRenderer.flipX = false;
                break;
            case LookDirection.FrontLeft:
                CurrentSprites = SpriteObject.FrontLeftSprites;
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

        if (SpriteObject == null)
        {
            Debug.LogError("Null animation: " + name);
            return;
        }

        LookDirection = newLookDirection;

        UpdateFrame();
    }
}
