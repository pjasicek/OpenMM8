using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class MonsterData : DbData
    {
        public MonsterType MonsterType;

        public string Name;
        public string Picture;
        public int Level;
        public int HitPoints;
        public int ArmorClass;
        public string AttackAmountText;
        public int ExperienceWorth;
        public NpcLootPrototype Treasure;
        public int Quest;
        public bool Fly;
        public MonsterMoveType MoveType;
        public MonsterAggresivityType Agressivity;
        public int Hostility;
        public int Speed;
        public float RecoveryTime;
        public AttackPreferenceMask AttackPreferenceMask;
        public int NumCharactersAffectedByBonusAbility;
        public string BonusAbility;

        public SpellElement Attack1_Element = SpellElement.None;
        public int Attack1_DamageDiceRolls;
        public int Attack1_DamageDiceSides;
        public int Attack1_DamageBonus;
        public string Attack1_Missile;

        public SpellElement Attack2_Element = SpellElement.None;
        public int Attack2_DamageDiceRolls;
        public int Attack2_DamageDiceSides;
        public int Attack2_DamageBonus;
        public string Attack2_Missile;
        public int Attack2_UseChance;

        public int Spell1_UseChance;
        public SpellType Spell1_SpellType = SpellType.None;
        public int Spell1_SkillLevel;
        public SkillMastery Spell1_SkillMastery;

        public int Spell2_UseChance;
        public SpellType Spell2_SpellType = SpellType.None;
        public int Spell2_SkillLevel;
        public SkillMastery Spell2_SkillMastery;

        public Dictionary<SpellElement, int> Resistances = new Dictionary<SpellElement, int>();

        public string SpecialAbility;
    }

    public enum MonsterType
    {
        None = 0,
        Lizardman_Male_Peasant_A = 1,
        Lizardman_Male_Peasant_B,
        Lizardman_Male_Peasant_C,
        Lizardman_Warrior_A,
        Lizardman_Warrior_B,
        Lizardman_Warrior_C,
        Winged_Snake_A,
        Winged_Snake_B,
        Winged_Snake_C,
        Pirate_Warrior_Male_A,
        Pirate_Warrior_Male_B,
        Pirate_Warrior_Male_C,
        Pirate_Crossbowman_Female_A,
        Pirate_Crossbowman_Female_B,
        Pirate_Crossbowman_Female_C,
        Pirate_Mage_Male_A,
        Pirate_Mage_Male_B,
        Pirate_Mage_Male_C,
        Dark_Elf_Peasant_Male_A,
        Dark_Elf_Peasant_Male_B,
        Dark_Elf_Peasant_Male_C,
        Dark_Elf_Peasant_Female_A,
        Dark_Elf_Peasant_Female_B,
        Dark_Elf_Peasant_Female_C,
        Dark_Elf_Warrior_A,
        Dark_Elf_Warrior_B,
        Dark_Elf_Warrior_C,
        Ogre_Peasant_Male_A,
        Ogre_Peasant_Male_B,
        Ogre_Peasant_Male_C,
        Ogre_Warrior_A,
        Ogre_Warrior_B,
        Ogre_Warrior_C,
        Wererat_Human_Form_Male_A,
        Wererat_Human_Form_Male_B,
        Wererat_Human_Form_Male_C,
        Wererat_Ratman_Form_Male_A,
        Wererat_Ratman_Form_Male_B,
        Wererat_Ratman_Form_Male_C,
        Wererat_Rat_Form_Male_A,
        Wererat_Rat_Form_Male_B,
        Wererat_Rat_Form_Male_C,
        Dragon_Hunter_A,
        Dragon_Hunter_B,
        Dragon_Hunter_C,
        Cleric_Human_Male_A,
        Cleric_Human_Male_B,
        Cleric_Human_Male_C,
        Vampire_Female_Peasant_A,
        Vampire_Female_Peasant_B,
        Vampire_Female_Peasant_C,
        Vampire_A,
        Vampire_B,
        Vampire_C,
        Necromancer_A,
        Necromancer_B,
        Necromancer_C,
        Troll_A,
        Troll_B,
        Troll_C,
        Troll_Peasant_A,
        Troll_Peasant_B,
        Troll_Peasant_C,
        Minotaur_A,
        Minotaur_B,
        Minotaur_C,
        Minotaur_Peasant_A,
        Minotaur_Peasant_B,
        Minotaur_Peasant_C,
        Dragon_A,
        Dragon_B,
        Dragon_C,
        Fire_Elemental_A,
        Fire_Elemental_B,
        Fire_Elemental_C,
        Water_Elemental_A,
        Water_Elemental_B,
        Water_Elemental_C,
        Earth_Elemental_A,
        Earth_Elemental_B,
        Earth_Elemental_C,
        Air_Elemental_A,
        Air_Elemental_B,
        Air_Elemental_C,
        Dire_Wolf_A,
        Dire_Wolf_B,
        Dire_Wolf_C,
        Thunder_Lizard_A,
        Thunder_Lizard_B,
        Thunder_Lizard_C,
        Cyclops_A,
        Cyclops_B,
        Cyclops_C,
        Serpentman_A,
        Serpentman_B,
        Serpentman_C,
        Will_O_Wisp_A,
        Will_O_Wisp_B,
        Will_O_Wisp_C,
        Centaur_A,
        Centaur_B,
        Centaur_C,
        Efreet_A,
        Efreet_B,
        Efreet_C,
        Ogre_Magi_A,
        Ogre_Magi_B,
        Ogre_Magi_C,
        Unicorn_A,
        Unicorn_B,
        Unicorn_C,
        Crystal_Dragon_A,
        Crystal_Dragon_B,
        Crystal_Dragon_C,
        Thunderbird_A,
        Thunderbird_B,
        Thunderbird_C,
        Wasp_Warrior_A,
        Wasp_Warrior_B,
        Wasp_Warrior_C,
        Wyvern_A,
        Wyvern_B,
        Wyvern_C,
        Gorgon_A,
        Gorgon_B,
        Gorgon_C,
        Salamander_A,
        Salamander_B,
        Salamander_C,
        Phoenix_A,
        Phoenix_B,
        Phoenix_C,
        Triton_A,
        Triton_B,
        Triton_C,
        Dragon_Turtle_A,
        Dragon_Turtle_B,
        Dragon_Turtle_C,
        Raven_Man_A,
        Raven_Man_B,
        Raven_Man_C,
        Malevolent_Boulder_A,
        Malevolent_Boulder_B,
        Malevolent_Boulder_C,
        Plane_Guardian_A,
        Plane_Guardian_B,
        Plane_Guardian_C,
        Crystal_Walker_A,
        Crystal_Walker_B,
        Crystal_Walker_C,
        Nightmare_A,
        Nightmare_B,
        Nightmare_C,
        Ether_Knight_A,
        Ether_Knight_B,
        Ether_Knight_C,
        Juggernaout_A,
        Juggernaout_B,
        Juggernaout_C,
        Gog_A,
        Gog_B,
        Gog_C,
        Bone_Dragon_A,
        Bone_Dragon_B,
        Bone_Dragon_C,
        Bedraggled_Human_Peasant_A,
        Bedraggled_Human_Peasant_B,
        Bedraggled_Human_Peasant_C,
        Naga_A,
        Naga_B,
        Naga_C,
        Skeleton_Archer_A,
        Skeleton_Archer_B,
        Skeleton_Archer_C,
        Dark_Dwarf_A,
        Dark_Dwarf_B,
        Dark_Dwarf_C,
        School_Of_Fish_A,
        School_Of_Fish_B,
        School_Of_Fish_C,
        Wimpy_Pirate_Warrior_Male_A,
        Wimpy_Pirate_Warrior_Male_B,
        Wimpy_Pirate_Warrior_Male_C,
        Wimpy_Pirate_Crossbowman_Female_A,
        Wimpy_Pirate_Crossbowman_Female_B,
        Wimpy_Pirate_Crossbowman_Female_C,
        Wimpy_Pirate_Mage_Male_A,
        Wimpy_Pirate_Mage_Male_B,
        Wimpy_Pirate_Mage_Male_C,
        Wimpy_Dragon_A,
        Wimpy_Dragon_B,
        Wimpy_Dragon_C,
        Human_Mercenary_A,
        Human_Mercenary_B,
        Human_Mercenary_C,
        Wimpy_Plane_Guardian_A,
        Wimpy_Plane_Guardian_B,
        Wimpy_Plane_Guardian_C,
    }
}
