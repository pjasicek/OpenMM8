using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using Assets.OpenMM8.Scripts.Gameplay.Data;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    class GameMgr : MonoBehaviour
    {
        // Singleton
        static public GameMgr Instance = new GameMgr();
        private GameMgr() { }

        // Databases
        public static ItemDb ItemDb;
        public static ItemEnchantDb ItemEnchantDb;
        public static NpcDb NpcDb;

        // States
        public static GameState GameState;
        public static MapType MapType;

        // Player
        public PlayerParty PlayerParty;

        // Events
        public delegate void GamePausedAction();
        public delegate void GameUnpausedAction();

        /*public event GamePausedAction OnGamePaused;
        public event GameUnpausedAction OnGameUnpaused;*/

        // UI Canvases
        public Canvas PartyCanvas;
        public Canvas PartyBuffsAndButtonsCanvas;
        public Canvas PartyInventoryCanvas;
        public Canvas HouseCanvas;
        public Canvas NpcTalkCanvas;


        static GameMgr()
        {
            ItemDb = new ItemDb();
            ItemEnchantDb = new ItemEnchantDb();
            NpcDb = new NpcDb();

            GameState = GameState.Ingame;
            MapType = MapType.Outdoor;
        }

        void Awake()
        {

        }

        void Start()
        {
            // 1) Gather relevant game objects

            PlayerParty = GameObject.Find("Player").GetComponent<PlayerParty>();

            GameObject PartyCanvasObject = GameObject.Find("PartyCanvas");
            if (PartyCanvasObject != null)
            {
                PartyCanvas = PartyCanvasObject.GetComponent<Canvas>();
                PartyBuffsAndButtonsCanvas = PartyCanvasObject.transform.Find("BuffsAndButtonsCanvas").GetComponent<Canvas>();
            }
            else
            {
                Debug.LogError("Could not find PartyCanvas gameobject !");
            }

            // 2) Initialize Player's party
                
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameState == GameState.Ingame)
                {
                    PauseGame();
                    GameState = GameState.IngamePaused;
                }
                else
                {
                    UnpauseGame();
                    GameState = GameState.Ingame;
                }
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                PartyBuffsAndButtonsCanvas.enabled = false;
            }
        }

        void PauseGame()
        {
            Time.timeScale = 0;
            AudioListener.pause = true;

            //OnGamePaused();
        }

        void UnpauseGame()
        {
            Time.timeScale = 1;
            AudioListener.pause = false;

            //OnGameUnpaused();
        }
    }
}
