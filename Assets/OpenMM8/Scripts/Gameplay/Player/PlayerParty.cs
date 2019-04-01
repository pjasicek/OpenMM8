using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public delegate void CharacterJoinedParty(Character chr, PlayerParty party);
    public delegate void CharacterLeftParty(Character chr, PlayerParty party);
    public delegate void HoverObject(HoverInfo hoverInfo);
    public delegate void GoldChanged(int oldGold, int newGold, int delta);
    public delegate void FoodChanged(int oldFood, int newFood, int delta);
    public delegate void FoundGold(int amount);
    public delegate void PickedUpLoot(Loot loot);
    public delegate void ActiveCharacterChanged(Character newSelChar);

    [RequireComponent(typeof(HostilityChecker))]
    [RequireComponent(typeof(Damageable))]
    public class PlayerParty : MonoBehaviour, ITriggerListener
    {
        public List<Character> Characters = new List<Character>();
        public Character ActiveCharacter;

        // Events
        static public event CharacterJoinedParty OnCharacterJoinedParty;
        static public event CharacterLeftParty OnCharacterLeftParty;
        static public event HoverObject OnHoverObject;
        static public event GoldChanged OnGoldChanged;
        static public event FoodChanged OnFoodChanged;
        static public event FoundGold OnFoundGold;
        static public event PickedUpLoot OnPickedUpLoot;
        static public event ActiveCharacterChanged OnActiveCharacterChanged;

        [SerializeField]
        private int MinutesSinceSleep;

        private HostilityChecker HostilityChecker;
        public AudioSource PlayerAudioSource;

        public List<GameObject> EnemiesInMeleeRange = new List<GameObject>();
        public List<GameObject> EnemiesInAgroRange = new List<GameObject>();
        public List<GameObject> ObjectsInMeleeRange = new List<GameObject>();

        // Misc
        public float AttackDelayTimeLeft = 0.0f;
        private float TimeSinceLastPartyText = 0.0f;

        private float PreviousTimeSinceStartup = 0.0f;

        public int Gold = 200;
        public int Food = 0;

        private void Awake()
        {

        }

        private void Start()
        {
            HostilityChecker = GetComponent<HostilityChecker>();

            Damageable damageable = GetComponent<Damageable>();
            damageable.OnAttackReceieved += new AttackReceived(OnAttackReceived);
            damageable.OnSpellReceived += new SpellReceived(OnSpellReceived);
            PlayerAudioSource = transform.Find("FirstPersonCharacter").GetComponent<AudioSource>();

            //InvokeRepeating("StableUpdate", 0.0f, 0.05f);
        }

        public void Update()
        {
            // Some things need to be updated even if game is paused, for example character's facial expressions
            float realtimeSinceStartup = Time.realtimeSinceStartup;
            float deltaTime = realtimeSinceStartup - PreviousTimeSinceStartup;
            PreviousTimeSinceStartup = realtimeSinceStartup;

            foreach (Character character in Characters)
            {
                character.OnFixedUpdate(deltaTime);
            }

            // TODO: Make some generic way to determine whether PlayerParty can act ...
            if (GameMgr.Instance.IsGamePaused() || Time.timeScale == 0.0f)
            {
                return;
            }

            foreach (Character character in Characters)
            {
                character.OnUpdate(Time.deltaTime);
            }

            if (ActiveCharacter == null || !ActiveCharacter.IsRecovered())
            {
                foreach (Character character in Characters)
                {
                    if (character.IsRecovered() && ((ActiveCharacter == null) || !ActiveCharacter.IsRecovered()))
                    {
                        SetActiveCharacter(character);
                    }
                    else
                    {
                        character.UI.SelectionRing.enabled = false;
                    }
                }
            }

            HandleHover();

            if (Input.GetButton("Attack") && UiMgr.Instance.m_HeldItem != null)
            {
                // TODO: Handle this more elegantly, this should not know anything about UiMgr
                // Throw the item 
                Ray throwRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.8f, 0.0f));
                ItemMgr.ThrowItem(transform, throwRay.direction, UiMgr.Instance.m_HeldItem.Item);
                GameObject.Destroy(UiMgr.Instance.m_HeldItem.gameObject);
                AttackDelayTimeLeft = 0.1f;
            }
            else if (Input.GetButton("Attack") && (AttackDelayTimeLeft <= 0.0f))
            {
                //Debug.Log("AttackDelayTimeLeft: " + AttackDelayTimeLeft);
                Attack();
            }

            if (Input.GetButtonDown("Interact"))
            {
                Interact();
            }

            if (Input.GetButtonDown("NextPlayer"))
            {
                TrySelectNextCharacter(true);
            }

            UpdateAgroStatus();

            AttackDelayTimeLeft -= Time.deltaTime;
        }

        public void StableUpdate()
        {
            foreach (Character character in Characters)
            {
                character.OnFixedUpdate(0.05f);
            }
        }

        public void SelectNextCharacter()
        {
            if (Characters.Count == 0)
            {
                return;
            }

            if (ActiveCharacter == null)
            {
                SelectCharacter(0);
            }

            TrySelectNextCharacter(false);
        }

        public bool TrySelectNextCharacter(bool onlyRecovered)
        {
            if (ActiveCharacter == null || Characters.Count <= 1)
            {
                return false;
            }

            bool found = false;
            int tries = Characters.Count - 1;
            int tryCharIndex = ActiveCharacter.GetPartyIndex() + 1;
            for (int i = 0; i < tries; i++)
            {
                if (tryCharIndex >= Characters.Count)
                {
                    tryCharIndex = 0;
                }

                if (!onlyRecovered || Characters[tryCharIndex].IsRecovered())
                {
                    SelectCharacter(tryCharIndex);
                    found = true;
                    break;
                }

                tryCharIndex++;
            }

            return found;
        }

        public bool TrySelectCharacter(int chrIndex)
        {
            if (chrIndex < Characters.Count && Characters[chrIndex].IsRecovered())
            {
                Characters.ForEach(ch => ch.UI.SelectionRing.enabled = false);
                SetActiveCharacter(Characters[chrIndex]);
                return true;
            }

            return false;
        }

        public void SelectCharacter(int chrIndex)
        {
            if (chrIndex < Characters.Count)
            {
                Characters.ForEach(ch => ch.UI.SelectionRing.enabled = false);
                SetActiveCharacter(Characters[chrIndex]);
            }
        }

        private void SetActiveCharacter(Character chr)
        {
            ActiveCharacter = chr;
            ActiveCharacter.UI.SelectionRing.enabled = true;
            if (OnActiveCharacterChanged != null)
            {
                OnActiveCharacterChanged(chr);
            }
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public bool IsFull()
        {
            return Characters.Count >= 5;
        }

        private void Attack()
        {
            if (ActiveCharacter != null && ActiveCharacter.IsRecovered())
            {
                Damageable victim = null;

                // 1) Try to attack NPC which is being targeted by the crosshair
                //    - does not have to be enemy, when we are aiming with attack
                //    directly on NPC, we can attack guard / villager this way
                RaycastHit hit;
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.595F, 0));
                if (Physics.Raycast(ray, out hit, 100.0f, 1 << LayerMask.NameToLayer("NPC")))
                {
                    Transform objectHit = hit.collider.transform;
                    if ((objectHit.GetComponent<Damageable>() != null) &&
                        (ObjectsInMeleeRange.Contains(objectHit.gameObject)))
                    {
                        victim = objectHit.GetComponent<Damageable>();
                    }
                }

                // 2) Try to attack enemy which is closest to Player
                if ((victim == null) && (EnemiesInMeleeRange.Count > 0))
                {
                    EnemiesInMeleeRange.RemoveAll(t => t == null);
                    EnemiesInMeleeRange.OrderBy(t => (t.transform.position - transform.position).sqrMagnitude);
                    foreach (GameObject enemyObject in EnemiesInMeleeRange)
                    {
                        if (enemyObject.GetComponent<Renderer>().isVisible)
                        {
                            victim = enemyObject.GetComponent<Damageable>();
                            break;
                        }
                    }
                }

                if (victim == null)
                {
                    // No victim in Player's melee range was found. Try to find some in range if player has a bow / crossbow
                }

                /*if (victim != null)
                {
                    Debug.Log("Hit with ");
                }*/

                ActiveCharacter.Attack(victim);
                if (!TrySelectNextCharacter(true))
                {
                    ActiveCharacter = null;
                }
                AttackDelayTimeLeft = 0.2f;
            }
        }

        private float GetRayDistance(Transform from, RaycastHit ray)
        {
            MeshCollider mc = ray.collider.transform.GetComponent<MeshCollider>();
            if (mc)
            {
                return ray.distance;
            }
            else
            {
                return Vector3.Distance(from.position, ray.collider.transform.position);
            }
        }

        private bool Interact()
        {
            Interactable interactObject = null;

            ObjectsInMeleeRange.RemoveAll(go => go == null || 
                Vector3.Distance(transform.position, go.transform.position) > Constants.MeleeRangeDistance);

            // 1) Try to interact with object being targeted by Crosshair
            List<Interactable> interactables = new List<Interactable>();

            RaycastHit hit;
            Ray ray = Camera.main.ViewportPointToRay(Constants.CrosshairScreenRelPos);

            int layerMask = ~((1 << LayerMask.NameToLayer("NpcRangeTrigger")) | (1 << LayerMask.NameToLayer("Player")));
            if (Physics.Raycast(ray, out hit, 100.0f, layerMask))
            {
                Transform objectHit = hit.collider.transform;

                /*MeshCollider mc = objectHit.GetComponent<MeshCollider>();
                float distance = 1000000.0f;
                if (mc)
                {
                    Debug.Log("MC center: " + mc.bounds.center.ToString());
                    Debug.Log("Player center: " + transform.position.ToString());
                    Debug.Log("Distance: " + Vector3.Distance(transform.position, mc.bounds.center).ToString());
                    Debug.Log("Hit distance: " + hit.distance.ToString());
                    distance = hit.distance;
                }
                else
                {
                    distance = Vector3.Distance(transform.position, objectHit.transform.position);
                }*/

                float distance = GetRayDistance(transform, hit);
                if (distance < Constants.MeleeRangeDistance)
                {
                    foreach (Interactable interactable in objectHit.GetComponents<Interactable>())
                    {
                        if (interactable.enabled)
                        {
                            interactables.Add(interactable);
                        }
                    }
                }

                if (interactObject != null)
                {
                    Debug.Log("+++ Can interact with: " + objectHit.name);
                    
                }
                else
                {
                    Debug.Log("--- Cannot interact with: " + objectHit.name);
                }

                // Handle also HoverInfo
                if ((objectHit.GetComponent<HoverInfo>() != null) &&
                    (objectHit.GetComponent<HoverInfo>().enabled))
                {
                    if (OnHoverObject != null)
                    {
                        OnHoverObject(objectHit.GetComponent<HoverInfo>());
                    }

                    /*string hoverText = objectHit.GetComponent<HoverInfo>().HoverText;
                    SetPartyInfoText(hoverText, true);*/
                }
            }

            // 2) Try to interact within any visible object within melee distance
            // Should I even want this to happen ?
            // Problem here is that Player's melee sensor does not intersect corpse's modified collider
            if (interactObject == null)
            {
                /*List<RaycastHit> closeObjects = Physics.SphereCastAll(
                    transform.position,
                    Constants.MeleeRangeDistance,
                    transform.forward, 
                    layerMask)
                    .ToList();

                Debug.Log("Found: " + closeObjects.Count);

                if (closeObjects.Count > 0)
                {
                    closeObjects.RemoveAll(r => r.distance > Constants.MeleeRangeDistance ||
                        r.collider.gameObject.GetComponent<Renderer>() == null ||
                        r.collider.gameObject.GetComponent<Interactable>() == null ||
                        r.collider.gameObject.GetComponent<Renderer>().isVisible == false);
                    if (closeObjects.Count > 0)
                    {
                        interactObject = closeObjects.OrderBy(r => (r.transform.position - transform.position).sqrMagnitude).
                            FirstOrDefault().
                            transform.GetComponent<Interactable>();
                    }
                }*/

                /*if ((interactObject == null) && (ObjectsInMeleeRange.Count > 0))
                {
                    ObjectsInMeleeRange.OrderBy(t => (t.transform.position - transform.position).sqrMagnitude);
                    foreach (GameObject closeObject in ObjectsInMeleeRange)
                    {
                        if (closeObject.GetComponent<Renderer>().isVisible &&
                            closeObject.GetComponent<Interactable>() != null)
                        {
                            interactObject = closeObject.GetComponent<Interactable>();
                            break;
                        }
                    }
                }*/
            }

            bool didInteract = false;
            foreach (Interactable ir in interactables)
            {
                if (ir.TryInteract(this.gameObject, hit))
                {
                    didInteract = true;
                }
            }

            /*if (interactObject != null)
            {
                return interactObject.TryInteract(this.gameObject, hit);
            }*/

            return didInteract;
        }

        private bool HandleHover()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ViewportPointToRay(Constants.CrosshairScreenRelPos);
            int layerMask = ~((1 << LayerMask.NameToLayer("NpcRangeTrigger")) | (1 << LayerMask.NameToLayer("Player")));
            if (Physics.Raycast(ray, out hit, 200.0f, layerMask))
            {
                Transform objectHit = hit.collider.transform;

                if ((objectHit.GetComponent<HoverInfo>() != null) &&
                    (objectHit.GetComponent<HoverInfo>().enabled))
                {
                    if (OnHoverObject != null)
                    {
                        OnHoverObject(objectHit.GetComponent<HoverInfo>());
                    }

                    return true;
                }
            }

            return false;
        }

        private void UpdatePartyEffects(int msDiff)
        {

        }

        private void UpdateConditions(int msDiff)
        {

        }

        private void UpdatePartyAgroStatus(AgroState agroState)
        {
            foreach (Character character in Characters)
            {
                character.UI.SetAgroStatus(agroState);
            }
        }

        public void AddCharacter(Character character)
        {
            if (Characters.Count == 5)
            {
                Logger.LogError("Already 5 characters in party, cannot add more !");
                return;
            }

            character.Party = this;
            Characters.Add(character);
            
            if (OnCharacterJoinedParty != null)
            {
                OnCharacterJoinedParty(character, this);
            }
        }

        public void RemoveCharacter(Character character)
        {
            if (!Characters.Remove(character))
            {
                Logger.LogError("Attempting to remove nonexisting character from party");
                return;
            }

            if (OnCharacterLeftParty != null)
            {
                OnCharacterLeftParty(character, this);
            }

            if (ActiveCharacter == character)
            {
                // Update will decide next active character
                ActiveCharacter = null;

                /*Character mostRecChr = GetMostRecoveredCharacter();
                if (mostRecChr != null && mostRecChr.IsRecovered())
                {
                    ActiveCharacter = mostRecChr;
                }*/
            }
        }

        // Damageable events
        AttackResult OnAttackReceived(AttackInfo hitInfo, GameObject source)
        {
            Character hitCharacter = null;
            if (hitInfo.PreferredClass != Class.None)
            {
                List<Character> preferredCharacters = new List<Character>();
                foreach (Character character in Characters)
                {
                    if (character.Data.Class == hitInfo.PreferredClass)
                    {
                        preferredCharacters.Add(character);
                    }
                }

                if (preferredCharacters.Count > 0)
                {
                    hitCharacter = preferredCharacters[UnityEngine.Random.Range(0, preferredCharacters.Count)];
                }
                else
                {
                    hitCharacter = Characters[UnityEngine.Random.Range(0, Characters.Count)];
                }
            }
            else
            {
                hitCharacter = Characters[UnityEngine.Random.Range(0, Characters.Count)];
            }

            if (hitCharacter == null)
            {
                Debug.LogError("hitCharacter is null !");
                return new AttackResult();
            }

            AttackResult result = DamageCalculator.DamageFromNpcToPlayer(hitInfo,
                hitCharacter.Data.DefaultStats.Resistances,
                hitCharacter.Data.DefaultStats.ArmorClass,
                hitCharacter.Data.DefaultStats.Attributes[Attribute.Luck]);
            result.Victim = this.gameObject;
            /*if (result.Type == AttackResultType.Miss)
            {
                return result;
            }*/

            hitCharacter.AddCurrHitPoints(-1 * result.DamageDealt);
            if (hitCharacter.Data.CurrHitPoints <= 0)
            {
                result.Type = AttackResultType.Kill;
            }

            hitCharacter.OnAttackReceived(hitInfo, result);

            return result;
        }

        SpellResult OnSpellReceived(SpellInfo hitInfo, GameObject source)
        {
            return new SpellResult();
        }

        //---------------------------------------------------------------------
        // Triggers
        //---------------------------------------------------------------------
        public void OnObjectEnteredMyTrigger(GameObject other, TriggerType triggerType)
        {
            //Debug.Log("Entered: " + other.name);
            switch (triggerType)
            {
                case TriggerType.MeleeRange:
                    OnObjectEnteredMeleeRange(other);
                    break;

                case TriggerType.AgroRange:
                    OnObjectEnteredAgroRange(other);
                    break;

                case TriggerType.ObjectTrigger:
                    OnObjectEnteredInteractibleRange(other);
                    break;

                default:
                    Debug.LogError("Unhandled Trigger Type: " + triggerType);
                    break;
            }
        }

        public void OnObjectLeftMyTrigger(GameObject other, TriggerType triggerType)
        {
            switch (triggerType)
            {
                case TriggerType.MeleeRange:
                    OnObjectLeftMeleeRange(other);
                    break;

                case TriggerType.AgroRange:
                    OnObjectLeftAgroRange(other);
                    break;

                case TriggerType.ObjectTrigger:
                    OnObjectLeftInteractibleRange(other);
                    break;

                default:
                    Debug.LogError("Unhandled Trigger Type: " + triggerType);
                    break;
            }
        }

        private void OnObjectLeftInteractibleRange(GameObject other)
        {
            
        }

        private void OnObjectEnteredInteractibleRange(GameObject other)
        {
            
        }

        public void OnObjectEnteredMeleeRange(GameObject other)
        {
            if (HostilityChecker.IsHostileTo(other))
            {
                EnemiesInMeleeRange.Add(other);
                UpdateAgroStatus();
            }

            ObjectsInMeleeRange.Add(other);
        }

        public void OnObjectLeftMeleeRange(GameObject other)
        {
            EnemiesInMeleeRange.Remove(other);
            UpdateAgroStatus();

            ObjectsInMeleeRange.Remove(other);
        }

        public void OnObjectEnteredAgroRange(GameObject other)
        {
            if (HostilityChecker.IsHostileTo(other))
            {
                EnemiesInAgroRange.Add(other);
                UpdateAgroStatus();
            }
        }

        public void OnObjectLeftAgroRange(GameObject other)
        {
            EnemiesInAgroRange.Remove(other);
            UpdateAgroStatus();
        }

        private void UpdateAgroStatus()
        {
            if (EnemiesInMeleeRange.Count > 0)
            {
                UpdatePartyAgroStatus(AgroState.HostileClose);
            }
            else if (EnemiesInAgroRange.Count > 0)
            {
                UpdatePartyAgroStatus(AgroState.HostileNearby);
            }
            else
            {
                UpdatePartyAgroStatus(AgroState.Safe);
            }
        }

        public void OnAcquiredLoot(Loot loot)
        {
            // Handle item
            if (loot.Item != null)
            {
                bool placed = false;
                if (!GetMostRecoveredCharacter().Inventory.AddItem(loot.Item))
                {
                    foreach (Character chr in Characters)
                    {
                        if (chr.Inventory.AddItem(loot.Item))
                        {
                            placed = true;
                            break;
                        }
                    }
                }
                else
                {
                    placed = true;
                }

                if (!placed)
                {
                    if (UiMgr.Instance.m_HeldItem != null)
                    {
                        Ray throwRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.8f, 0.0f));
                        ItemMgr.ThrowItem(transform, throwRay.direction, UiMgr.Instance.m_HeldItem.Item);
                        GameObject.Destroy(UiMgr.Instance.m_HeldItem.gameObject);
                    }

                    UiMgr.Instance.SetHeldItem(loot.Item);
                }
            }

            if (loot.GoldAmount > 0)
            {
                AddGold(loot.GoldAmount);

                if (OnFoundGold != null)
                {
                    OnFoundGold(loot.GoldAmount);
                }
            }

            if (OnPickedUpLoot != null && (loot.Item != null || loot.GoldAmount > 0))
            {
                OnPickedUpLoot(loot);
            }
        }

        public void AddGold(int amount)
        {
            Gold += amount;

            if (OnGoldChanged != null)
            {
                OnGoldChanged(Gold - amount, Gold, amount);
            }
        }

        public void AddFood(int amount)
        {
            Food += amount;
        }

        public Character GetMostRecoveredCharacter()
        {
            if (ActiveCharacter != null && ActiveCharacter.IsRecovered())
            {
                return ActiveCharacter;
            }

            return Characters.Aggregate((ch1, ch2) => ch1.TimeUntilRecovery < ch2.TimeUntilRecovery ? ch1 : ch2);
        }

        public Character GetRandomCharacter()
        {
            return Characters[UnityEngine.Random.Range(0, Characters.Count - 1)];
        }

        public Character GetActiveCharacter()
        {
            if (ActiveCharacter != null && ActiveCharacter.IsRecovered())
            {
                return ActiveCharacter;
            }
            else
            {
                return null;
            }
        }

        public Character GetFirstCharacter()
        {
            return Characters[0];
        }
    }
}
