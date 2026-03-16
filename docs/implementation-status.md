# Implementation Status

This document summarizes current gameplay/system coverage in OpenMM8 compared to original Might and Magic VIII, using OpenEnroth as a practical reference for classic feature scope and behavior.

It is intentionally focused on implementation status, not design intent.

## Reference Point

Classic MM8 / OpenEnroth includes, among other things:

- Party and character runtime with full stats, conditions, skills, buffs, equipment, inventory, and persistence
- Outdoor and indoor maps with cross-map travel
- Monsters, combat, projectiles, spellcasting, loot, death state, and map-local persistence
- Chests with opened/trapped/disarmed state and chest inventory UI
- Doors, facet state changes, map object state changes, lights, textures, sprites
- NPC talk, house entry, guilds, banks, temples, training, shops, stables, boats
- Hirelings / followers / recruitable NPCs
- Arena, bounty hunts, town services, travel systems
- Save/load and persistent world state

## Strongly Implemented

These areas already have meaningful runtime implementation and are not just placeholders.

### Party and Character Core

Implemented in and around:

- [Character.cs](/home/pjasicek/github/OpenMM8/Assets/OpenMM8/Scripts/Gameplay/Game/Player/Character.cs)
- [PlayerParty.cs](/home/pjasicek/github/OpenMM8/Assets/OpenMM8/Scripts/Gameplay/Game/Player/PlayerParty.cs)

Current coverage:

- Party member list and active character selection
- Character stats, conditions, skills, recovery, experience, spell points, hit points
- Buff and condition runtime
- Learned spells list
- Gold and food on party

### Inventory Grid and Doll Equipment

Implemented in and around:

- [Inventory.cs](/home/pjasicek/github/OpenMM8/Assets/OpenMM8/Scripts/Gameplay/Game/Player/Inventory.cs)
- [UiMgr.cs](/home/pjasicek/github/OpenMM8/Assets/OpenMM8/Scripts/Gameplay/UI/UiMgr.cs)
- [Item.cs](/home/pjasicek/github/OpenMM8/Assets/OpenMM8/Scripts/Gameplay/Game/Items/Item.cs)

Current coverage:

- Inventory grid placement
- Held item flow
- Item swapping and replacement in inventory
- Equipping and unequipping through the doll
- Race/class/equipment compatibility checks
- Doll slot visuals and item sprites

### NPC Talk and Building Talk

Implemented in and around:

- [TalkEventMgr.cs](/home/pjasicek/github/OpenMM8/Assets/OpenMM8/Scripts/Gameplay/Game/GameEvents/TalkEventMgr.cs)
- [TalkUIState.cs](/home/pjasicek/github/OpenMM8/Assets/OpenMM8/Scripts/Gameplay/UI/UIState/TalkUIState.cs)
- [EventAPI.cs](/home/pjasicek/github/OpenMM8/Assets/OpenMM8/Scripts/Gameplay/Game/GameEvents/EventAPI.cs)

Current coverage:

- Outdoor NPC talk
- House/building talk scenes
- Topic handling
- Greeting/message updates
- Quest-bit-driven talk mutations
- Runtime NPC talk state mutation

### Monster Combat and AI

Implemented in and around:

- [Monster.cs](/home/pjasicek/github/OpenMM8/Assets/OpenMM8/Scripts/AI/NPC/Monster.cs)
- [MonsterAI.cs](/home/pjasicek/github/OpenMM8/Assets/OpenMM8/Scripts/AI/NPC/MonsterAI.cs)
- [Projectile.cs](/home/pjasicek/github/OpenMM8/Assets/OpenMM8/Scripts/Gameplay/Components/Projectile.cs)

Current coverage:

- Monster melee attacks
- Monster missile and spell attacks
- Projectile spawning for monster attacks
- Hostility handling
- Damage and condition application
- Loot drop hook path

### Quest Bits, Awards, Variables, Timers

Implemented in and around:

- [QuestMgr.cs](/home/pjasicek/github/OpenMM8/Assets/OpenMM8/Scripts/Gameplay/Quest/QuestMgr.cs)
- [EventAPI.cs](/home/pjasicek/github/OpenMM8/Assets/OpenMM8/Scripts/Gameplay/Game/GameEvents/EventAPI.cs)

Current coverage:

- Quest bits
- Awards
- Autonotes and history placeholders
- Map vars
- Event timers
- A broad script-variable access layer

## Partially Implemented

These systems exist, but still diverge materially from original MM8 behavior.

### Item Interaction Through the Doll

Status: partial

What works:

- Equippable items can be dragged to the doll and equipped
- Equipped items can be removed and swapped

What is still incomplete:

- Non-equip interactions are not fully realized gameplay systems
- In [Character.cs](/home/pjasicek/github/OpenMM8/Assets/OpenMM8/Scripts/Gameplay/Game/Player/Character.cs), consumables, spell scrolls, spellbooks, and readable scrolls mostly return an `ItemInteractResult`, play a reaction, and clear the held item
- They do not yet fully perform all original MM8 item-use behaviors

### House Entry vs House Services

Status: partial

What works:

- Buildings can be entered as talk scenes
- Building data supports NPCs inside, open hours, enter sound, and optional video

What is missing:

- Bank service
- Temple service
- Training
- Shop buy/sell/repair/identify
- Guild service logic
- Stable/boat/travel service logic

Reference data exists in:

- [BuildingData.cs](/home/pjasicek/github/OpenMM8/Assets/OpenMM8/Scripts/Data/DataHolders/BuildingData.cs)

But there is no service-type runtime behind it yet.

### Event Scripting Surface

Status: partial

The script-facing API is broad, but many world-facing actions are still stubs in:

- [EventAPI.cs](/home/pjasicek/github/OpenMM8/Assets/OpenMM8/Scripts/Gameplay/Game/GameEvents/EventAPI.cs)

Still stubbed or mostly stubbed:

- `OpenChest`
- `SetDoorState`
- `StopDoor`
- `SetFacetBit`
- `SetMonGroupBit`
- `ShowMovie`
- `SetTexture`
- `SetSprite`
- `SetLight`
- `CastSpell`
- `SummonMonsters`
- `SummonObject`
- `Question`
- `IsTotalBountyInRange`
- cross-map `MoveToMap`

### Roster Join / Recruitment

Status: partial

What works:

- Talk flow can present recruit/join prompts
- Yes/No nested talk topics are implemented

What is still missing:

- Real roster-id-based recruitment backend
- Correct recruited character resolution
- Proper full-party / inn behavior

Current placeholder lives in:

- [PlayerParty.cs](/home/pjasicek/github/OpenMM8/Assets/OpenMM8/Scripts/Gameplay/Game/Player/PlayerParty.cs)

### Spell Coverage

Status: partial

There is a substantial spell runtime in:

- [SpellCastHelper.cs](/home/pjasicek/github/OpenMM8/Assets/OpenMM8/Scripts/Gameplay/Game/Spells/SpellCastHelper.cs)

But many classic spells still log not implemented, including examples like:

- Town Portal
- Lloyd's Beacon
- Recharge Item
- Enchant Item
- various other school- and race-specific spells

### Player Combat

Status: partial

What works:

- Player melee combat path exists

What is still missing or incomplete:

- Full ranged weapon flow comparable to classic MM8
- Proper bow/crossbow projectile attack path
- Fuller attack event/audio polish

## Missing Systems

These are the clearest classic MM8/OpenEnroth systems that are still absent as real gameplay systems.

### Chests

Status: missing

Classic MM8/OpenEnroth has:

- Chest open state
- Trap state
- Disarmed state
- Chest inventory contents
- Chest UI interaction
- Persistence for chest state

Current OpenMM8 state:

- [EventAPI.cs](/home/pjasicek/github/OpenMM8/Assets/OpenMM8/Scripts/Gameplay/Game/GameEvents/EventAPI.cs) `OpenChest(...)` is still a stub
- There is no real chest runtime, state model, or chest UI

### Doors

Status: missing

Classic MM8/OpenEnroth has:

- Door objects and geometry updates
- Door state machine
- Door sounds
- Script-controlled open/close/stop behavior

Current OpenMM8 state:

- `SetDoorState(...)` and `StopDoor(...)` exist only as stubs in [EventAPI.cs](/home/pjasicek/github/OpenMM8/Assets/OpenMM8/Scripts/Gameplay/Game/GameEvents/EventAPI.cs)

### Cross-Map Travel / Map Loading

Status: missing

Current state:

- Same-map teleports work
- Cross-map `MoveToMap(...)` still warns and does not load another scene/map

Needed for parity:

- map-name to scene-name resolution
- pending spawn state across scene loads
- current-map identification
- map scene lifecycle cleanup/rebind

### Save / Load

Status: missing

I did not find a real savegame/loadgame runtime in the gameplay code.

That means the following are also not yet real persistent systems:

- world state persistence
- chest state persistence
- map-local object state persistence
- cross-session party persistence

### Shop / Service Houses

Status: missing

Not implemented as real systems yet:

- buy/sell
- repair
- identify item
- temple healing/donation
- bank deposit/withdrawal
- training
- guild service logic
- stable/boat travel service logic

### Hirelings / Followers

Status: missing

Classic MM8/OpenEnroth has:

- hirelings/followers
- profession bonuses
- dismiss/hire/payment logic
- map restrictions

Current OpenMM8 state:

- recruit/join talk exists only in partial roster form
- full hireling/follower runtime is not present

### Arena / Bounty / Town Hall Style Systems

Status: missing

The event variable surface mentions some of these concepts, but there is no real gameplay system yet for:

- Arena
- Bounty hunt
- town-hall-style service gameplay

### World Trap / Disarm / Steal Style Interactions

Status: missing

The skill enums and voice data exist, but the actual gameplay systems are not present for:

- trapped chests as a system
- trap disarming as a real runtime flow
- stealing / thievery interactions

## Summary by Example

### Opening Chests and Their Flags

Current status: missing

Missing pieces:

- chest object/runtime
- chest contents
- opened flag
- trapped flag
- disarmed flag
- chest UI
- persistence

### Item Interaction With the Doll

Current status: partial

Implemented:

- equip/unequip/swap

Not fully implemented:

- full original behavior for consumables, spell scrolls, spellbooks, readable items

## Recommended Next Major Systems

If the goal is to move toward original-MM8 completeness, the most valuable missing systems to implement next are:

1. Chest runtime
2. House services
3. Cross-map transitions
4. Save/load and persistent world state
5. Hirelings/followers
