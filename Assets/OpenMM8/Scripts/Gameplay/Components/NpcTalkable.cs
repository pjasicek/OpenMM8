using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class NpcTalkable : InteractableGameEvent
    {
        protected override bool CanInteract(GameObject interacter, RaycastHit interactRay)
        {
            Monster monster = FindTalkMonster();
            if (monster != null &&
                (monster.IsEnemy() || monster.HostilityType != HostilityType.Friendly))
            {
                return false;
            }

            return base.CanInteract(interacter, interactRay);
        }

        protected override bool Interact(GameObject interacter, RaycastHit interactRay)
        {
            Monster monster = FindTalkMonster();
            if (monster != null)
            {
                monster.FaceInteracterForConversation(interacter.transform);
            }

            return base.Interact(interacter, interactRay);
        }

        private Monster FindTalkMonster()
        {
            Monster monster = GetComponent<Monster>();
            if (monster != null)
            {
                return monster;
            }

            monster = GetComponentInParent<Monster>();
            if (monster != null)
            {
                return monster;
            }

            return GetComponentInChildren<Monster>();
        }
    }
}
