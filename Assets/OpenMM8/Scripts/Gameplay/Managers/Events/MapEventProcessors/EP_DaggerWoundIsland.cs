using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class EP_DaggerWoundIsland : MapEventProcessor
    {
        public override void ProcessEvent(int evtId)
        {
            switch (evtId)
            {
                case 11: // Hiss' Hut
                    Game.EnterHouse(224);
                    break;

                case 13: // Rohtnax's House
                    Game.EnterHouse(225);
                    break;

                case 15: // Tisk's Hut
                    Game.EnterHouse(226);
                    break;

                case 17: // Thadin's House
                    Game.EnterHouse(227);
                    break;

                case 19: // House of Ich
                    Game.EnterHouse(228);
                    break;

                case 21: //  Languid's House
                    Game.EnterHouse(229);
                    break;

                case 23: //  House of Thistle
                    Game.EnterHouse(230);
                    break;

                case 25: //  Zevah's Hut
                    Game.EnterHouse(231);
                    break;

                case 27: //  Isthric's House
                    Game.EnterHouse(232);
                    break;

                case 29: //  Bone's House
                    Game.EnterHouse(233);
                    break;

                case 31: // Lasatin's House
                    Game.EnterHouse(234);
                    break;

                case 33: //  Menasaur's House
                    Game.EnterHouse(235);
                    break;

                case 35: //  Husk's Hut
                    Game.EnterHouse(236);
                    break;

                case 37: // Talimere's Hut
                    Game.EnterHouse(237);
                    break;

                case 39: //House of Reshie
                    Game.EnterHouse(238);
                    break;

                case 41: // House
                    break;

                case 43: // Long-Tail's Hut
                    Game.EnterHouse(240);
                    break;

                case 45: // Aislen's House
                    Game.EnterHouse(241);
                    break;

                case 47: // House of Grivic
                    Game.EnterHouse(242);
                    break;

                case 49: // Ush's Hut
                    Game.EnterHouse(243);
                    break;



                case 171: // True Mettle - Weapon Shop
                    Game.EnterHouse(1);
                    break;

                case 173: // The Tannery - Armor Shop
                    Game.EnterHouse(15);
                    break;

                case 175: // Fearsome Fetishes - Magic Shop
                    Game.EnterHouse(29);
                    break;

                case 177: // Herbal Elixirs - Alchemy Shop
                    Game.EnterHouse(42);
                    break;

                case 179: // Cures and Curses - Spell Shop
                    Game.EnterHouse(139);
                    break;

                case 183: // The Windling - Ship to Ravenshore
                    Game.EnterHouse(63);
                    break;

                case 185: // Mystic Medicine - Temple
                    Game.EnterHouse(74);
                    break;

                case 187: // Rites of Passage - Training Hall
                    Game.EnterHouse(89);
                    break;

                case 191: // The Grog and Grub - Inn
                    Game.EnterHouse(107);
                    break;

                case 193: // The Some Place Safe - Bank
                    Game.EnterHouse(128);
                    break;

                case 197: // Clan Leader's Hall 
                    Game.EnterHouse(173);
                    break;

                case 199: // Adventurer's Inn
                    Game.EnterHouse(155);
                    break;



                case 1000: // Lizardman Peasant - NPC News
                    if (Game.IsQuestBitSet(6))
                    {
                        Game.TalkNPCNews(516, 2);
                    }
                    else
                    {
                        Game.TalkNPCNews(516, 1);
                    }
                    
                    break;

                case 1001: // Lizardman Guard - NPC News
                    Game.TalkWithNPC(517);
                    break;

                default:
                    Logger.LogError("Unimplemented Map Game Event: " + evtId);
                    break;
            }
        }
    }
}
