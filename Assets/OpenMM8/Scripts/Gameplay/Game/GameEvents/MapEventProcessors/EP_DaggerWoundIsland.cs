using System.Collections.Generic;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class EP_DaggerWoundIsland : MapEventProcessor
    {
        private static readonly Dictionary<int, int> s_HouseEventMap = new Dictionary<int, int>()
        {
            { 11, 224 }, { 12, 224 },
            { 13, 225 }, { 14, 225 },
            { 15, 226 }, { 16, 226 },
            { 17, 227 }, { 18, 227 },
            { 19, 228 }, { 20, 228 },
            { 21, 229 }, { 22, 229 },
            { 23, 230 }, { 24, 230 },
            { 25, 231 }, { 26, 231 },
            { 27, 232 }, { 28, 232 },
            { 29, 233 }, { 30, 233 },
            { 31, 234 }, { 32, 234 },
            { 33, 235 }, { 34, 235 },
            { 35, 236 }, { 36, 236 },
            { 37, 237 }, { 38, 237 },
            { 39, 238 }, { 40, 238 },
            { 41, 239 }, { 42, 239 },
            { 43, 240 }, { 44, 240 },
            { 45, 241 }, { 46, 241 },
            { 47, 242 }, { 48, 242 },
            { 49, 243 }, { 50, 243 },
            { 171, 1 }, { 172, 1 },
            { 173, 15 }, { 174, 15 },
            { 175, 29 }, { 176, 29 },
            { 177, 42 }, { 178, 42 },
            { 179, 139 }, { 180, 139 },
            { 183, 63 }, { 184, 63 },
            { 185, 74 }, { 186, 74 },
            { 187, 89 }, { 188, 89 },
            { 191, 107 }, { 192, 107 },
            { 193, 128 }, { 194, 128 },
            { 197, 173 }, { 198, 173 },
            { 199, 185 }, { 200, 185 },
        };

        private static readonly HashSet<int> s_NoOpEvents = new HashSet<int>()
        {
            4, 6, 7, 8, 9, 10,
            401, 402, 403, 404, 405, 406, 407, 408,
            449, 450, 464, 470, 479,
        };

        private static readonly Vector3[] s_Event456TargetsA =
        {
            new Vector3(-9216, -12848, 3000),
            new Vector3(-9232, -7680, 3000),
            new Vector3(-5872, -8832, 3000),
            new Vector3(-7200, -4656, 3000),
            new Vector3(-4960, -3440, 3000),
            new Vector3(-3472, -6544, 3000),
        };

        private static readonly Vector3[] s_Event456TargetsB =
        {
            new Vector3(-96, -3568, 3000),
            new Vector3(3184, -976, 3000),
            new Vector3(912, 544, 3000),
            new Vector3(608, 2976, 3000),
            new Vector3(2976, 2240, 3000),
            new Vector3(6144, 832, 3000),
            new Vector3(9376, -4416, 3000),
        };

        private static readonly Vector3[] s_Event456TargetsC =
        {
            new Vector3(12256, -8640, 3000),
            new Vector3(5136, -14192, 3000),
            new Vector3(6320, -15840, 3000),
            new Vector3(7584, -20016, 3000),
            new Vector3(-3504, -15744, 3000),
            new Vector3(560, -9776, 3000),
        };

        private static readonly Vector3[] s_Event468Targets =
        {
            new Vector3(15920, 2850, 977),
            new Vector3(14789, 3735, 705),
            new Vector3(13056, 3752, 736),
            new Vector3(15926, 1274, 420),
        };

        public override void Init()
        {
            ProcessEvent(1);
            ProcessEvent(2);
            ProcessEvent(3);
            ProcessEvent(4);

            AddRepeatingTimer(456, 0, 15);
            AddRepeatingTimer(460, 0, 15);
            AddRepeatingTimer(463, 0, 10);
            AddRepeatingTimer(468, 0, 20);
            AddRepeatingTimer(469, 0, 20);
            AddOneShotHourTimer(500, 10);
        }

        public override void ProcessEvent(int evtId)
        {
            if (s_HouseEventMap.TryGetValue(evtId, out int houseId))
            {
                EventAPI.EnterHouse(houseId);
                return;
            }

            if (s_NoOpEvents.Contains(evtId))
            {
                return;
            }

            switch (evtId)
            {
                case 1:
                    if (EventAPI.IsQuestBitSet(2))
                    {
                        EventAPI.SetFacetBit(10, EventFacetBit.Invisible, false);
                        EventAPI.SetFacetBit(10, EventFacetBit.Untouchable, false);
                    }
                    break;

                case 2:
                    if (EventAPI.IsQuestBitSet(6))
                    {
                        EventAPI.SetFacetBit(20, EventFacetBit.Invisible, true);
                        EventAPI.SetFacetBit(20, EventFacetBit.Untouchable, true);
                        EventAPI.SetMonGroupBit(14, EventMonsterBit.Invisible, true);
                        EventAPI.SetMonGroupBit(10, EventMonsterBit.Invisible, true);
                        EventAPI.SetMonGroupBit(11, EventMonsterBit.Invisible, true);
                        EventAPI.SetNPCGroupNews(12, 13);
                        EventAPI.SetNPCGroupNews(13, 13);
                        EventAPI.SetNPCGroupNews(1, 2);
                    }
                    else
                    {
                        EventAPI.SetNPCGroupNews(12, 12);
                        EventAPI.SetNPCGroupNews(13, 12);
                    }

                    if (EventAPI.IsQuestBitSet(36) || EventAPI.IsQuestBitSet(38))
                    {
                        EventAPI.SetFacetBit(25, EventFacetBit.Invisible, false);
                        EventAPI.SetFacetBit(25, EventFacetBit.Untouchable, false);
                        EventAPI.SetFacetBit(26, EventFacetBit.Untouchable, true);
                    }
                    else
                    {
                        EventAPI.SetFacetBit(25, EventFacetBit.Invisible, true);
                        EventAPI.SetFacetBit(25, EventFacetBit.Untouchable, true);
                    }
                    break;

                case 3:
                    if (!EventAPI.IsQuestBitSet(226))
                    {
                        EventAPI.AddQuestBit(226);
                        EventAPI.AddQuestBit(185);
                        EventAPI.AddQuestBit(401);
                        EventAPI.AddQuestBit(407);
                    }
                    break;

                case 81:
                    EventAPI.OpenChest(EventAPI.IsCharacterInParty(2) ? 3 : 0);
                    break;

                case 82:
                case 83:
                case 85:
                case 86:
                case 87:
                case 88:
                case 89:
                case 90:
                case 91:
                case 92:
                    EventAPI.OpenChest(evtId == 82 ? 1 :
                        evtId == 83 ? 2 :
                        evtId == 85 ? 4 :
                        evtId == 86 ? 5 :
                        evtId == 87 ? 6 :
                        evtId == 88 ? 7 :
                        evtId == 89 ? 8 :
                        evtId == 90 ? 9 :
                        evtId == 91 ? 10 : 11);
                    break;

                case 93:
                case 94:
                case 95:
                case 96:
                case 97:
                case 98:
                case 99:
                case 100:
                    EventAPI.OpenChest(evtId - 81);
                    break;

                case 101:
                    HandleIntellectWell();
                    break;

                case 102:
                    HandleLuckWell();
                    break;

                case 103:
                    HandleGoldWell();
                    break;

                case 104:
                    HandleHealingFountain();
                    break;

                case 150:
                    if (!EventAPI.IsQuestBitSet(186))
                    {
                        EventAPI.StatusText("summerday");
                        EventAPI.AddAutonote(25);
                        EventAPI.AddQuestBit(186);
                    }
                    break;

                case 451:
                    EventAPI.MoveToMap(-480, 5432, 384, 512);
                    break;

                case 452:
                    EventAPI.MoveToMap(10123, 4488, 736);
                    break;

                case 453:
                    HandlePowerStoneTeleport(1, 617, 8, new Vector3(-21528, -1384, 0), 512);
                    break;

                case 454:
                    EventAPI.FaceAnimation(EventAPI.GetCurrentCharacter(), (CharacterExpression)18);
                    break;

                case 455:
                    foreach (Character chr in EventAPI.PartyCharacters())
                    {
                        EventAPI.SetVar("Dead", 0, chr);
                    }
                    break;

                case 456:
                    CastRandomSpell(9, 10, 3, new Vector3(19872, -19824, 5084), s_Event456TargetsA);
                    CastRandomSpell(9, 10, 3, new Vector3(19872, -19824, 5084), s_Event456TargetsB);
                    CastRandomSpell(9, 10, 3, new Vector3(19872, -19824, 5084), s_Event456TargetsC);
                    break;

                case 457:
                    EventAPI.CastSpell(6, 1, 1, new Vector3(8704, 2000, 686), new Vector3(8704, 1965, 686));
                    EventAPI.CastSpell(136, 1, 1, new Vector3(8704, 1950, 686), new Vector3(8704, -2592, 686));
                    break;

                case 458:
                    EventAPI.CastSpell(6, 1, 1, new Vector3(15872, 1500, 686), new Vector3(15872, -1000, 686));
                    EventAPI.CastSpell(136, 1, 1, new Vector3(15872, 1440, 686), new Vector3(15872, -3100, 686));
                    break;

                case 459:
                    EventAPI.CastSpell(6, 1, 1, new Vector3(18400, 3584, 652), new Vector3(18522, 3584, 652));
                    EventAPI.CastSpell(136, 1, 1, new Vector3(18522, 3584, 652), new Vector3(22880, 3584, 100));
                    break;

                case 460:
                    if (!EventAPI.IsQuestBitSet(6))
                    {
                        if (!EventAPI.CheckMonstersKilled(4, 8, 1))
                        {
                            EventAPI.CastSpell(6, 1, 1, new Vector3(15872, 1500, 686), new Vector3(15872, -1000, 686));
                            EventAPI.CastSpell(136, 1, 1, new Vector3(15872, 1440, 686), new Vector3(15872, -3100, 100));
                        }

                        if (!EventAPI.CheckMonstersKilled(4, 9, 1))
                        {
                            EventAPI.CastSpell(6, 1, 1, new Vector3(1536, 16400, 682), new Vector3(1536, 16480, 682));
                            EventAPI.CastSpell(136, 1, 1, new Vector3(1536, 16480, 682), new Vector3(1536, 21528, 100));
                        }
                    }
                    break;

                case 461:
                    EventAPI.CastSpell(6, 1, 1, new Vector3(1536, 16400, 682), new Vector3(1536, 16480, 682));
                    EventAPI.CastSpell(136, 1, 1, new Vector3(1536, 16480, 682), new Vector3(1536, 21528, 100));
                    break;

                case 462:
                    EventAPI.CastSpell(6, 1, 1, new Vector3(-520, 15360, 682), new Vector3(-610, 15360, 682));
                    EventAPI.CastSpell(136, 1, 1, new Vector3(-610, 15360, 682), new Vector3(-6320, 15360, 100));
                    break;

                case 463:
                    if (!EventAPI.IsQuestBitSet(6))
                    {
                        if (EventAPI.CheckMonstersKilled(1, 10, 0))
                        {
                            SpawnReinforcementWave10();
                        }

                        if (EventAPI.CheckMonstersKilled(1, 11, 0))
                        {
                            SpawnReinforcementWave11();
                        }

                        if (EventAPI.CheckMonstersKilled(1, 12, 0))
                        {
                            SpawnReinforcementWave12();
                        }

                        if (EventAPI.CheckMonstersKilled(1, 13, 0))
                        {
                            SpawnReinforcementWave13();
                        }
                    }
                    break;

                case 465:
                    HandlePowerStoneTeleport(1, 617, 8, new Vector3(-12496, -9728, 160), 512);
                    break;

                case 466:
                    HandlePowerStoneTeleport(2, 618, null, new Vector3(-13912, 14096, 0), 512);
                    break;

                case 467:
                    HandlePowerStoneTeleport(2, 618, null, new Vector3(-18952, 8608, 96), 1536);
                    break;

                case 468:
                    if (!EventAPI.IsQuestBitSet(6))
                    {
                        CastRandomSpell(6, 1, 1, new Vector3(16433, -3164, 180), s_Event468Targets);
                    }
                    break;

                case 469:
                    if (!EventAPI.IsQuestBitSet(6))
                    {
                        CastRandomSpell(6, 1, 1, new Vector3(1496, 21593, 180), s_Event468Targets);
                    }
                    break;

                case 471:
                    EventAPI.MoveToMap(8760, 4408, 736, 1536);
                    if (!EventAPI.IsQuestBitSet(227))
                    {
                        EventAPI.AddQuestBit(227);
                    }
                    break;

                case 472:
                    if (EventAPI.IsQuestBitSet(227))
                    {
                        EventAPI.MoveToMap(21216, 18680, 0, 1024);
                    }
                    break;

                case 494:
                    HandleHarvestablePalmTree();
                    break;

                case 495:
                    HandleHarvestableFlower();
                    break;

                case 497:
                    HandleBuoySkillPoints(268, 13, 2);
                    break;

                case 498:
                    HandleBuoySkillPoints(269, 20, 5);
                    break;

                case 500:
                    if (!EventAPI.IsQuestBitSet(232))
                    {
                        EventAPI.TalkWithNPC(31);
                        EventAPI.AddQuestBit(232);
                    }
                    break;

                case 501:
                    EventAPI.MoveToMap(-3008, -1696, 2464, 512, 0, 0, 191, 1, "D05.blv");
                    break;

                case 502:
                    EventAPI.MoveToMap(-7, -714, 1, 512, 0, 0, 192, 1, "D06.blv");
                    break;

                case 503:
                    EventAPI.MoveToMap(-592, 624, 0, 552, 0, 0, 0, 1, "d40.blv");
                    break;

                case 504:
                    EventAPI.MoveToMap(12704, 2432, 385, 0, 0, 0, 500, 1, "d05.blv");
                    break;

                case 505:
                    EventAPI.MoveToMap(0, 0, 49, 512, 0, 0, 221, 1, "ElemE.blv");
                    break;

                case 1000:
                    EventAPI.TalkNPCNews(516, EventAPI.IsQuestBitSet(6) ? 2 : 1);
                    break;

                case 1001:
                    EventAPI.TalkWithNPC(517);
                    break;

                default:
                    Logger.LogError("Unimplemented Map Game Event: " + evtId);
                    break;
            }
        }

        private void AddRepeatingTimer(int eventId, int startDelayMinutes, int intervalMinutes)
        {
            AddTimer(new Timer()
            {
                Name = "DaggerWoundIsland Event " + eventId,
                DelayMinutes = startDelayMinutes,
                IntervalInMinutes = intervalMinutes,
                OnTimer = (timer, time) => { ProcessEvent(eventId); }
            });
        }

        private void AddOneShotHourTimer(int eventId, int targetHour)
        {
            AddTimer(new Timer()
            {
                Name = "DaggerWoundIsland Event " + eventId,
                DelayMinutes = GetDelayUntilHour(targetHour),
                OnTimer = (timer, time) => { ProcessEvent(eventId); }
            });
        }

        private long GetDelayUntilHour(int targetHour)
        {
            long currentMinutes = TimeMgr.Instance.CurrentTime.GetMinutes();
            long targetMinutes = new GameTime(0, 0, targetHour).GetMinutes();
            long dayMinutes = GameTime.FromDays(1).GetMinutes();

            while (targetMinutes < currentMinutes)
            {
                targetMinutes += dayMinutes;
            }

            return targetMinutes - currentMinutes;
        }

        private void HandleIntellectWell()
        {
            Character currentCharacter = EventAPI.GetCurrentCharacter();
            if (currentCharacter != null && EventAPI.GetVar("IntellectBonus", currentCharacter) < 15)
            {
                EventAPI.AddVar("IntellectBonus", 15, currentCharacter);
                EventAPI.StatusText("Intellect +15 (Temporary)");
                EventAPI.AddAutonote(245);
            }
            else
            {
                EventAPI.StatusText("Refreshing");
            }
        }

        private void HandleLuckWell()
        {
            Character currentCharacter = EventAPI.GetCurrentCharacter();
            if (currentCharacter != null && EventAPI.GetVar("BaseLuck", currentCharacter) < 16)
            {
                EventAPI.AddVar("BaseLuck", 2, currentCharacter);
                EventAPI.StatusText("Luck +2 (Permanent)");
                EventAPI.AddAutonote(246);
            }
            else
            {
                EventAPI.StatusText("Refreshing");
            }
        }

        private void HandleGoldWell()
        {
            Character currentCharacter = EventAPI.GetCurrentCharacter();
            if (currentCharacter != null &&
                EventAPI.GetMapVar("MapVar29") < 2 &&
                EventAPI.GetVar("Gold", currentCharacter) < 99 &&
                EventAPI.GetVar("BankGold", currentCharacter) < 99 &&
                EventAPI.GetVar("BaseLuck", currentCharacter) >= 14)
            {
                EventAPI.AddGold(1000);
                EventAPI.SetMapVar("MapVar29", EventAPI.GetMapVar("MapVar29") + 1);
                EventAPI.AddAutonote(247);
            }
            else
            {
                EventAPI.StatusText("Refreshing");
            }
        }

        private void HandleHealingFountain()
        {
            Character currentCharacter = EventAPI.GetCurrentCharacter();
            if (currentCharacter != null && EventAPI.GetVar("HasFullHP", currentCharacter) == 0)
            {
                EventAPI.AddVar("HP", 25, currentCharacter);
                EventAPI.StatusText("Your Wounds begin to Heal");
                EventAPI.AddAutonote(248);
            }
            else
            {
                EventAPI.StatusText("Refreshing");
            }
        }

        private void HandlePowerStoneTeleport(int unlockQuestBit, int powerStoneItemId, int? requiredQuestBit,
            Vector3 position, int direction)
        {
            if (!EventAPI.IsQuestBitSet(unlockQuestBit))
            {
                if (!EventAPI.CheckItemsCount(powerStoneItemId, 1))
                {
                    EventAPI.StatusText("You need a power stone to operate this teleporter");
                    return;
                }

                if (requiredQuestBit.HasValue && !EventAPI.IsQuestBitSet(requiredQuestBit.Value))
                {
                    return;
                }

                EventAPI.RemoveItems(powerStoneItemId, 1);
                EventAPI.AddQuestBit(unlockQuestBit);
            }

            EventAPI.MoveToMap(position.x, position.y, position.z, direction);
        }

        private void CastRandomSpell(int spellId, int skillLevel, int skillMastery, Vector3 fromPosition, Vector3[] targets)
        {
            if (targets.Length == 0)
            {
                return;
            }

            Vector3 target = targets[Random.Range(0, targets.Length)];
            EventAPI.CastSpell(spellId, skillLevel, skillMastery, fromPosition, target);
        }

        private void SpawnReinforcementWave10()
        {
            EventAPI.SummonMonsters(2, 2, 3, new Vector3(776, -66192, 0), 10);
            EventAPI.SummonMonsters(2, 2, 3, new Vector3(0, -5608, 0), 10);
            EventAPI.SummonMonsters(2, 2, 3, new Vector3(-656, -5696, 23), 10);
            EventAPI.SummonMonsters(2, 2, 3, new Vector3(-1280, -5720, 0), 10);
        }

        private void SpawnReinforcementWave11()
        {
            EventAPI.SummonMonsters(2, 2, 3, new Vector3(-2744, 4864, 176), 11);
            EventAPI.SummonMonsters(2, 2, 3, new Vector3(-2984, 4208, 561), 11);
            EventAPI.SummonMonsters(2, 2, 3, new Vector3(-3624, 4280, 400), 11);
            EventAPI.SummonMonsters(2, 2, 3, new Vector3(-3504, 4992, 74), 11);
        }

        private void SpawnReinforcementWave12()
        {
            EventAPI.SummonMonsters(1, 2, 3, new Vector3(5208, -736, 46), 12);
            EventAPI.SummonMonsters(1, 2, 3, new Vector3(3120, 800, 226), 12);
            EventAPI.SummonMonsters(1, 2, 3, new Vector3(3480, -2656, 88), 12);
            EventAPI.SummonMonsters(1, 2, 3, new Vector3(2080, -2248, 539), 12);
        }

        private void SpawnReinforcementWave13()
        {
            EventAPI.SummonMonsters(1, 2, 3, new Vector3(-896, 55504, 384), 13);
            EventAPI.SummonMonsters(1, 2, 3, new Vector3(-104, 5328, 384), 13);
            EventAPI.SummonMonsters(1, 2, 3, new Vector3(-880, 4464, 510), 13);
            EventAPI.SummonMonsters(1, 2, 3, new Vector3(-1256, 5296, 241), 13);
        }

        private void HandleHarvestablePalmTree()
        {
            Character currentCharacter = EventAPI.GetCurrentCharacter();
            if (currentCharacter == null ||
                EventAPI.IsQuestBitSet(270) ||
                EventAPI.GetVar("RepairSkill", currentCharacter) < 3)
            {
                return;
            }

            int[] objectIds = { 200, 205, 210, 215, 220 };
            int objectId = objectIds[Random.Range(0, objectIds.Length)];
            EventAPI.SummonObject(objectId, new Vector3(3896, 8080, 544), 1000, 1, true);
            EventAPI.AddQuestBit(270);
        }

        private void HandleHarvestableFlower()
        {
            Character currentCharacter = EventAPI.GetCurrentCharacter();
            if (currentCharacter == null ||
                EventAPI.IsQuestBitSet(271) ||
                EventAPI.GetVar("RepairSkill", currentCharacter) < 5)
            {
                return;
            }

            int[] objectIds = { 2138, 2139, 2140, 2141 };
            int objectId = objectIds[Random.Range(0, objectIds.Length)];
            EventAPI.SummonObject(objectId, new Vector3(-18832, 5840, 330), 1000, 1, true);
            EventAPI.AddQuestBit(271);
        }

        private void HandleBuoySkillPoints(int questBitId, int requiredBaseLuck, int skillPointReward)
        {
            Character currentCharacter = EventAPI.GetCurrentCharacter();
            if (currentCharacter == null ||
                EventAPI.IsQuestBitSet(questBitId) ||
                EventAPI.GetVar("BaseLuck", currentCharacter) < requiredBaseLuck)
            {
                return;
            }

            EventAPI.AddQuestBit(questBitId);
            EventAPI.AddVar("SkillPoints", skillPointReward, currentCharacter);
        }
    }
}
