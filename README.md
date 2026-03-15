# OpenMM8 - Might & Magic 8 (2001) reimplementation

 - This project is a Unity reimplementation of original Might & Magic 8 RPG
 - Unity 2022.3 LTS is now the active editor line
 - No third-party assets are currently used
## Current State
 - Original maps are loadable
 - Semi-Working basic GUI (Working minimap + player arrow, working Compas)
 - Working mechanism of animating / rotating NPCs - they are actually 8-sided 2D sprites, not 3D models
 - Villager AI is implemented - they wander within pre-defined location, they look at Player when near and they run from Hostile NPCs (or player)

# In-Game screenshots
![alt tag](https://s9.postimg.cc/utmjrnrwv/screenshot_50.png)
![alt tag](https://s9.postimg.cc/6eix450bj/screenshot_51.png)

## Architecture

The project is being cleaned up after the Unity 2022 migration. The current architecture direction and migration rules are documented in [docs/unity-rpg-architecture.md](docs/unity-rpg-architecture.md).

## Current Priority

The current delivery target is 100% Dagger Wound Island playability. Architecture work should support that goal, not compete with it.
