using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class EventTriggerCollider : MonoBehaviour
    {
        [Header("Event")]
        [SerializeField] private int eventNumber;
        [SerializeField] private bool triggerOnce;
        [SerializeField] private float cooldownSeconds;

        private bool hasTriggered;
        private float nextAllowedTriggerTime;

        private void Reset()
        {
            Collider triggerCollider = GetComponent<Collider>();
            if (triggerCollider != null)
            {
                triggerCollider.isTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            TryTrigger(other);
        }

        private void TryTrigger(Collider other)
        {
            if (hasTriggered && triggerOnce)
            {
                return;
            }

            if (Time.time < nextAllowedTriggerTime)
            {
                return;
            }

            PlayerParty playerParty = other.GetComponentInParent<PlayerParty>();
            if (playerParty == null)
            {
                return;
            }

            if (GameEventMgr.Instance == null)
            {
                Debug.LogError("EventTriggerCollider: GameEventMgr.Instance is null.");
                return;
            }

            GameEventMgr.Instance.ProcessGameEvent(eventNumber);

            hasTriggered = true;
            nextAllowedTriggerTime = Time.time + cooldownSeconds;
        }
    }
}
