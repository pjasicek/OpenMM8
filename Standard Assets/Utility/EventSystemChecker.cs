using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemChecker : MonoBehaviour
{
    private void Awake()
    {
        EventSystem[] systems = FindObjectsOfType<EventSystem>(true);

        if (systems.Length == 0)
        {
            GameObject obj = new GameObject("EventSystem");
            obj.AddComponent<EventSystem>();
            obj.AddComponent<StandaloneInputModule>();
            return;
        }

        if (systems.Length > 1)
        {
            Debug.LogWarning("Multiple EventSystems found in scene.");

            // Keep the first active one, remove extras.
            bool keptOne = false;
            foreach (EventSystem system in systems)
            {
                if (!keptOne)
                {
                    keptOne = true;
                    continue;
                }

                Destroy(system.gameObject);
            }
        }
    }
}