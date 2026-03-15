# Vendor Areas

`Assets/OpenMM8/Vendor` is the quarantine area for imported code and plugins that are not part of OpenMM8 gameplay runtime.

- `UnityStandardAssets`: imported Unity sample/runtime helpers still used by gameplay code.
- `ThirdParty`: active third-party libraries used by the project.
- `Debug`: development-only plugins such as the in-game console.
- `Legacy`: duplicate or partial vendor copies kept out of the main runtime path until they are reviewed or removed.

Do not add new gameplay code here. New OpenMM8 code should go under `Assets/OpenMM8/Runtime` once that runtime split starts.
