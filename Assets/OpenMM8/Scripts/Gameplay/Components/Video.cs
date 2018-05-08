using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Video;
using System;

public class Video : MonoBehaviour
{

    //Raw Image to Show Video Images [Assign from the Editor]
    public RawImage image;
    //Video To Play [Assign from the Editor]
    public VideoClip videoToPlay;

    // Dirty "Double buffering"...
    private VideoPlayer videoPlayer1;
    private VideoPlayer videoPlayer2;

    AudioSource AudioSource;

    private bool m_IsPlaying = false;

    private VideoPlayer CreateVideoPlayer()
    {
        VideoPlayer plr = gameObject.AddComponent<VideoPlayer>();

        //Add AudioSource
        /*AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = 0.5f;
        audioSource.playOnAwake = false;*/

        plr.playOnAwake = false;
        //plr.isLooping = true;
        plr.skipOnDrop = true;
        plr.waitForFirstFrame = true;
        plr.source = VideoSource.VideoClip;
        /*plr.audioOutputMode = VideoAudioOutputMode.AudioSource;
        plr.SetTargetAudioSource(0, audioSource);*/
        plr.clip = videoToPlay;

        return plr;
    }

    // Use this for initialization
    void Start()
    {
        Application.runInBackground = true;

        AudioSource = GetComponent<AudioSource>();

        videoPlayer1 = CreateVideoPlayer();
        videoPlayer2 = CreateVideoPlayer();

        videoPlayer1.loopPointReached += OnLoop1;
        videoPlayer2.loopPointReached += OnLoop2;

        videoPlayer1.started += OnStarted1;
        videoPlayer2.started += OnStarted2;

        //Set video To Play then prepare Audio to prevent Buffering
        //videoPlayer.clip = videoToPlay;
    }    

    private void OnStarted1(VideoPlayer source)
    {
        videoPlayer2.Prepare();
    }

    private void OnStarted2(VideoPlayer source)
    {
        videoPlayer1.Prepare();
    }

    private void OnLoop1(VideoPlayer source)
    {
        StartCoroutine(playVideo(videoPlayer2));
    }

    private void OnLoop2(VideoPlayer source)
    {
        StartCoroutine(playVideo(videoPlayer1));
    }

    public void Play(VideoClip clip, AudioClip audioClip)
    {
        if (clip != null)
        {
            videoToPlay = clip;
        }

        if (videoToPlay == null)
        {
            Debug.LogError("null video");
            return;
        }

        AudioSource.clip = audioClip;
        image.enabled = true;
        StartCoroutine(playVideo(videoPlayer1));
    }

    public void Stop()
    {
        if (videoPlayer1.isPlaying)
        {
            videoPlayer1.Stop();
        }
        if (videoPlayer2.isPlaying)
        {
            videoPlayer2.Stop();
        }

        m_IsPlaying = false;
        AudioSource.Stop();
        image.enabled = false;
    }

    IEnumerator playVideo(VideoPlayer plr)
    {
        if (!plr.isPrepared)
        {
            plr.clip = videoToPlay;
            plr.Prepare();

            //Wait until video is prepared
            while (!plr.isPrepared)
            {
                //Debug.Log("Preparing Video");
                yield return null;
            }
        }

        //Debug.Log("Done Preparing Video");

        //Assign the Texture from Video to RawImage to be displayed
        image.texture = plr.texture;

        /*if (!m_IsPlaying)
        {
            AudioSource.Play();
        }*/
        AudioSource.Stop();
        AudioSource.Play();

        //Play Video
        plr.Play();
        m_IsPlaying = true;

        //Play Sound
        //audioSource.Play();

        //Debug.Log("Playing Video");
        /*while (videoPlayer.isPlaying)
        {
            Debug.LogWarning("Video Time: " + Mathf.FloorToInt((float)videoPlayer.time));
            yield return null;
        }

        Debug.Log("Done Playing Video");*/
    }
}
