using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Video;
using System;
using Assets.OpenMM8.Scripts.Gameplay;

public class VideoScene : MonoBehaviour
{

    //Raw Image to Show Video Images [Assign from the Editor]

    public bool UseAudioFromVideo = false;

    //Video To Play [Assign from the Editor]
    public VideoClip VideoToPlay;
    public AudioClip AudioToPlay;
    public AudioClip StartSound;
    public AudioClip EndSound;

    // Dirty "Double buffering"...
    private VideoPlayer m_VideoPlayer1;
    private VideoPlayer m_VideoPlayer2;

    private AudioSource m_AudioSource;
    private RawImage m_Image;

    private VideoPlayer CreateVideoPlayer()
    {
        VideoPlayer plr = gameObject.AddComponent<VideoPlayer>();

        plr.playOnAwake = false;
        plr.skipOnDrop = true;
        plr.waitForFirstFrame = true;
        plr.source = VideoSource.VideoClip;
        plr.clip = VideoToPlay;

        if (UseAudioFromVideo)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = 0.5f;
            audioSource.playOnAwake = false;
            plr.audioOutputMode = VideoAudioOutputMode.AudioSource;
            plr.SetTargetAudioSource(0, audioSource);
        }

        return plr;
    }

    // Use this for initialization
    void Start()
    {
        Application.runInBackground = true;

        m_AudioSource = GetComponent<AudioSource>();
        if (!UseAudioFromVideo)
        {
            m_AudioSource.clip = AudioToPlay;
            m_AudioSource.loop = true;
        }

        m_VideoPlayer1 = CreateVideoPlayer();
        m_VideoPlayer2 = CreateVideoPlayer();

        m_VideoPlayer1.loopPointReached += OnLoop1;
        m_VideoPlayer2.loopPointReached += OnLoop2;

        m_VideoPlayer1.started += OnStarted1;
        m_VideoPlayer2.started += OnStarted2;

        m_Image = UiMgr.Instance.SceneVideoImage;
        if (m_Image == null)
        {
            m_Image = GameObject.Find("PartyCanvas").transform.Find("NpcTalkCanvas")
                .transform.Find("SceneVideoImage").GetComponent<RawImage>();
        }

        if (m_Image == null)
        {
            Debug.LogError("null raw image");
        }

        if (VideoToPlay == null)
        {
            Debug.LogError("null video");
        }
        if (AudioToPlay == null)
        {
            Debug.LogWarning("null audio");
        }

        if (VideoToPlay != null)
        {
            // Pre-buffer
            StartCoroutine(Prepare(m_VideoPlayer1));
        }
    }

    private void OnStarted1(VideoPlayer source)
    {
        m_VideoPlayer2.Prepare();
    }

    private void OnStarted2(VideoPlayer source)
    {
        m_VideoPlayer1.Prepare();
    }

    private void OnLoop1(VideoPlayer source)
    {
        StartCoroutine(playVideo(m_VideoPlayer2));
    }

    private void OnLoop2(VideoPlayer source)
    {
        StartCoroutine(playVideo(m_VideoPlayer1));
    }

    public void Play()
    {
        if (VideoToPlay == null)
        {
            Debug.LogError("null video");
            return;
        }

        if (AudioToPlay == null)
        {
            Debug.LogWarning("null audio");
        }

        m_Image.texture = m_VideoPlayer1.texture;
        m_Image.enabled = true;

        // This vid should already be buffered
        StartCoroutine(playVideo(m_VideoPlayer1));

        if (StartSound != null)
        {
            m_AudioSource.PlayOneShot(StartSound);
        }
    }

    public void Stop()
    {
        m_AudioSource.Stop();
        if (m_VideoPlayer1.isPlaying)
        {
            m_VideoPlayer1.Stop();
        }
        if (m_VideoPlayer2.isPlaying)
        {
            m_VideoPlayer2.Stop();
        }
        m_Image.enabled = false;

        if (EndSound != null)
        {
            m_AudioSource.PlayOneShot(EndSound);
        }

        // Pre-buffer
        StartCoroutine(Prepare(m_VideoPlayer1));
    }

    IEnumerator Prepare(VideoPlayer plr)
    {
        if (!plr.isPrepared)
        {
            plr.Prepare();

            while (!plr.isPrepared)
            {
                yield return null;
            }
        }
    }

    IEnumerator playVideo(VideoPlayer plr)
    {
        // Vid should always be prepared
        if (!plr.isPrepared)
        {
            plr.Prepare();

            while (!plr.isPrepared)
            {
                yield return null;
            }
        }

        //Assign the Texture from Video to RawImage to be displayed
        m_Image.texture = plr.texture;

        m_AudioSource.Stop();
        m_AudioSource.Play();

        plr.Play();
    }
}
