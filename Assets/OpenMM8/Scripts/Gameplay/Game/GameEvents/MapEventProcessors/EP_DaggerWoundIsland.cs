using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class EP_DaggerWoundIsland : MapEventProcessor
    {
        Timer m_StonTimer;

        public override void Init()
        {

            m_StonTimer = new Timer()
            {
                Name = "S'ton Timer",
                Delay = 10 * TimeMgr.HOUR_IN_MINUTES,
                OnTimer = (Timer t, GameTime time) => { ProcessEvent(500); }
            };
            AddTimer(m_StonTimer);
        }

        public override void ProcessEvent(int evtId)
        {
            switch (evtId)
            {
                case 11: // Hiss' Hut
                    EventAPI.EnterHouse(224);
                    break;

                case 13: // Rohtnax's House
                    EventAPI.EnterHouse(225);
                    break;

                case 15: // Tisk's Hut
                    EventAPI.EnterHouse(226);
                    break;

                case 17: // Thadin's House
                    EventAPI.EnterHouse(227);
                    break;

                case 19: // House of Ich
                    EventAPI.EnterHouse(228);
                    break;

                case 21: //  Languid's House
                    EventAPI.EnterHouse(229);
                    break;

                case 23: //  House of Thistle
                    EventAPI.EnterHouse(230);
                    break;

                case 25: //  Zevah's Hut
                    EventAPI.EnterHouse(231);
                    break;

                case 27: //  Isthric's House
                    EventAPI.EnterHouse(232);
                    break;

                case 29: //  Bone's House
                    EventAPI.EnterHouse(233);
                    break;

                case 31: // Lasatin's House
                    EventAPI.EnterHouse(234);
                    break;

                case 33: //  Menasaur's House
                    EventAPI.EnterHouse(235);
                    break;

                case 35: //  Husk's Hut
                    EventAPI.EnterHouse(236);
                    break;

                case 37: // Talimere's Hut
                    EventAPI.EnterHouse(237);
                    break;

                case 39: //House of Reshie
                    EventAPI.EnterHouse(238);
                    break;

                case 41: // House
                    break;

                case 43: // Long-Tail's Hut
                    EventAPI.EnterHouse(240);
                    break;

                case 45: // Aislen's House
                    EventAPI.EnterHouse(241);
                    break;

                case 47: // House of Grivic
                    EventAPI.EnterHouse(242);
                    break;

                case 49: // Ush's Hut
                    EventAPI.EnterHouse(243);
                    break;



                case 171: // True Mettle - Weapon Shop
                    EventAPI.EnterHouse(1);
                    break;

                case 173: // The Tannery - Armor Shop
                    EventAPI.EnterHouse(15);
                    break;

                case 175: // Fearsome Fetishes - Magic Shop
                    EventAPI.EnterHouse(29);
                    break;

                case 177: // Herbal Elixirs - Alchemy Shop
                    EventAPI.EnterHouse(42);
                    break;

                case 179: // Cures and Curses - Spell Shop
                    EventAPI.EnterHouse(139);
                    break;

                case 183: // The Windling - Ship to Ravenshore
                    EventAPI.EnterHouse(63);
                    break;

                case 185: // Mystic Medicine - Temple
                    EventAPI.EnterHouse(74);
                    break;

                case 187: // Rites of Passage - Training Hall
                    EventAPI.EnterHouse(89);
                    break;

                case 191: // The Grog and Grub - Inn
                    EventAPI.EnterHouse(107);
                    break;

                case 193: // The Some Place Safe - Bank
                    EventAPI.EnterHouse(128);
                    break;

                case 197: // Clan Leader's Hall 
                    EventAPI.EnterHouse(173);
                    break;

                case 199: // Adventurer's Inn
                    EventAPI.EnterHouse(155);
                    break;



                case 500: // S'ton
                    if (!EventAPI.IsQuestBitSet(232))
                    {
                        EventAPI.TalkWithNPC(31);
                        EventAPI.AddQuestBit(232);
                    }
                    break;

                case 1000: // Lizardman Peasant - NPC News
                    if (EventAPI.IsQuestBitSet(6))
                    {
                        EventAPI.TalkNPCNews(516, 2);
                    }
                    else
                    {
                        EventAPI.TalkNPCNews(516, 1);
                    }
                    
                    break;

                case 1001: // Lizardman Guard - NPC News
                    EventAPI.TalkWithNPC(517);
                    break;

                default:
                    Logger.LogError("Unimplemented Map Game Event: " + evtId);
                    break;
            }
        }
    }
}
