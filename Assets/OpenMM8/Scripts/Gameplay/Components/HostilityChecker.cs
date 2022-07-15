﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostilityChecker : MonoBehaviour
{
    public enum HostilityType { Friendly, Hostile, Undefined };
    public enum NPCRace
    {
        Undefined,
        Rock,
        Bazilisk,
        Dragon,
        DragonHatchling,
        DragonTurtle,
        Efrete,
        EtheralKnight,
        Phoenix,
        Gog,
        Gorgon,
        Lizardman,
        ThunderBird,
        Unicorn,
        LizardHabitant,
        Lizard,
        Centaur,
        Cleric,
        BoneDragon,
        Skeleton,
        Raven,
        Ratman,
        CrystalGuard,
        Rat,
        Cyclop,
        Snake,
        DragonHunter,
        Lycanthrope,
        Minotaur,
        Naga,
        Necromancer,
        Nightmare,
        FireElemental,
        WaterElemental,
        AirElemental,
        EarthElemental,
        Chaos,
        Pirate,
        Salamander,
        Wisp,
        DarkElf,
        DarkDwarf,
        Triton,
        Troll,
        Vampire,
        Vandal,
        Wolf,
        Bee,
        Ogre,
        BountyHunter,
        Player
    }

    public HostilityType m_HostilityType = HostilityType.Undefined;
    public NPCRace m_Race = NPCRace.Undefined;
    public List<NPCRace> m_SpecialHostileTo;
    public List<NPCRace> m_SpecialFriendlyTo;
    public bool m_IsHostileToPlayer = false;

    public bool IsHostileTo(GameObject what)
    {
        if (what.name == "Player")
        {
            return m_IsHostileToPlayer;
        }

        HostilityChecker otherChecker = what.GetComponent<HostilityChecker>();
        if (otherChecker == this)
        {
            return false;
        }

        if (otherChecker)
        {
            // TODO: Clarify if when hostile monsters attack eachother
            if (m_HostilityType == HostilityType.Hostile &&
                otherChecker.m_HostilityType == HostilityType.Hostile)
            {
                return false;
            }

            if (m_HostilityType == HostilityType.Friendly &&
                otherChecker.m_HostilityType == HostilityType.Hostile)
            {
                return true;
            }
            if (m_HostilityType == HostilityType.Hostile &&
                otherChecker.m_HostilityType == HostilityType.Friendly)
            {
                return true;
            }

            if (name == "Player" && otherChecker.m_IsHostileToPlayer)
            {
                return true;
            }
        }

        return false;
    }

    public void AddHostileTarget(GameObject other)
    {

    }

    private void Awake()
    {
        if (m_HostilityType == HostilityType.Hostile)
        {
            m_IsHostileToPlayer = true;
        }
    }

    // Use this for initialization
    void Start ()
    {
		
	}
}
