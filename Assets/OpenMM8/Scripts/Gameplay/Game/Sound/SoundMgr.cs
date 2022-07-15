using Assets.OpenMM8.Scripts.Gameplay.Data;
using Assets.OpenMM8.Scripts.Gameplay.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    class SoundMgr : Singleton<SoundMgr>
    {
        // Public

        // Private
        private AudioSource m_AudioSource;
        private AudioClip m_BackgroundMusic;

        private Dictionary<string, AudioClip> m_SoundMap = new Dictionary<string, AudioClip>();
        private Dictionary<int, AudioClip> m_SoundIdMap = new Dictionary<int, AudioClip>();

        //=================================== Unity Lifecycle ===================================

        private void Awake()
        {
            // Events
            SceneManager.sceneLoaded += OnSceneLoaded;

            GameEvents.OnPauseGame += OnGamePaused;
            GameEvents.OnUnpauseGame += OnGameUnpaused;
        }

        // Init sequence: DbMgr(1) -> GameMgr(1) -> *Mgr(1) -> GameMgr(2)
        public bool Init()
        {
            // Load sounds / music
            m_BackgroundMusic = Resources.Load<AudioClip>("Music/1");

            m_AudioSource = gameObject.AddComponent<AudioSource>();
            m_AudioSource.clip = m_BackgroundMusic;
            m_AudioSource.loop = true;
            m_AudioSource.volume = 0.33f;
            //m_AudioSource.Play();


            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            /*AudioClip[] sounds = Resources.LoadAll<AudioClip>("Sounds");
            foreach (AudioClip sound in sounds)
            {
                m_SoundMap[sound.name] = sound;
            }*/

            sw.Stop();
            //UnityEngine.Debug.LogError("Sound load elapsed: " + sw.ElapsedMilliseconds + " ms");

            return true;
        }

        private void Start()
        {

        }


        //=================================== Methods ===================================

        // TODO: Handle sound caching or something, this way most sounds get lazy loaded
        //       but the never get unloaded once they are loaded
        // TODO: Maybe use existing SoundData database to store and cache sounds

        public static void PlaySoundById(SoundType soundType, AudioSource audioSource = null)
        {
            PlaySoundById((int)soundType, audioSource);
        }

        public static void PlaySoundById(int soundId, AudioSource audioSource = null)
        {
            // 0 is not valid sound
            if (soundId == 0)
            {
                return;
            }

            if (audioSource == null)
            {
                audioSource = Instance.m_AudioSource;
            }

            SoundData soundData = DbMgr.Instance.SoundDb.Get(soundId);
            if (soundData == null)
            {
                Debug.LogError("No sound data for ID: " + soundId);
                return;
            }

            string soundName = soundData.SoundName;
            PlaySoundByName(soundName, audioSource);
        }

        public static void PlaySoundByName(string soundName, AudioSource audioSource = null)
        {
            if (audioSource == null)
            {
                audioSource = Instance.m_AudioSource;
            }

            AudioClip sound = null;
            if (!Instance.m_SoundMap.ContainsKey(soundName))
            {
                // Try to load it
                sound = Resources.Load<AudioClip>("Sounds/" + soundName);
                if (sound == null)
                {
                    Debug.LogError("Failed to load sound: " + soundName);
                    return;
                }

                Instance.m_SoundMap.Add(soundName, sound);
            }
            else
            {
                sound = Instance.m_SoundMap[soundName];
            }

            audioSource.PlayOneShot(sound);
        }

        public static void PlayRandomSound(List<int> soundIds)
        {
            PlayRandomSound(soundIds, Instance.m_AudioSource);
        }

        public static void PlayRandomSound(List<int> soundIds, AudioSource audioSource)
        {
            if (soundIds.Count == 0)
            {
                return;
            }

            int soundId = soundIds[UnityEngine.Random.Range(0, soundIds.Count)];

            PlaySoundById(soundId);
        }
    
        public static AudioClip PlayRandomSound(List<AudioClip> sounds)
        {
            return PlayRandomSound(sounds, Instance.m_AudioSource);
        }

        public static AudioClip PlayRandomSound(List<AudioClip> sounds, AudioSource audioSource)
        {
            if (sounds.Count == 0)
            {
                return null;
            }

            AudioClip sound = sounds[UnityEngine.Random.Range(0, sounds.Count)];
            audioSource.PlayOneShot(sound);

            return sound;
        }

        //=================================== Events ===================================

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            
        }

        public void OnGamePaused()
        {
            
        }

        private void OnGameUnpaused()
        {
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.UnPause();
            }
        }
    }
}
