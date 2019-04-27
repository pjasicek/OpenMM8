﻿using Assets.OpenMM8.Scripts.Gameplay.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    

    [RequireComponent(typeof(HostilityChecker))]
    [RequireComponent(typeof(Damageable))]
    public class PlayerParty : MonoBehaviour, ITriggerListener
    {
        public List<Character> Characters = new List<Character>();
        public Character ActiveCharacter;

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


        public Dictionary<PartyEffectType, SpellEffect> PartyBuffMap = new Dictionary<PartyEffectType, SpellEffect>();

        // Party status flags
        public bool IsOnWater;
        public bool IsOnLava;
        public bool IsFlying;

        // Periodical ticks
        public GameTime PartyTime = new GameTime(0);
        public GameTime LastRegenTickTime = new GameTime(0);
        public GameTime LastWaterLavaTickTime = new GameTime(0);

        public int DaysPlayedWithoutRest = 0;

        // Delayed speech sound
        public Character DelayedSpeaker = null;
        public CharacterReaction DelayedAvatarReaction;
        public float TimeUntilDelayedSpeech;

        // UI
        public PartyBuffUI PartyBuffUI;

        // ========================================================================================

        private void Awake()
        {
            foreach (PartyEffectType effect in Enum.GetValues(typeof(PartyEffectType)))
            {
                PartyBuffMap.Add(effect, new SpellEffect());
            }
        }

        private void Start()
        {
            HostilityChecker = GetComponent<HostilityChecker>();

            Damageable damageable = GetComponent<Damageable>();
            damageable.OnAttackReceieved += new AttackReceived(OnAttackReceived);
            damageable.OnSpellReceived += new SpellReceived(OnSpellReceived);
            PlayerAudioSource = transform.Find("FirstPersonCharacter").GetComponent<AudioSource>();

            PartyBuffUI = PartyBuffUI.Create(this);

            //InvokeRepeating("StableUpdate", 0.0f, 0.05f);
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

            GameEvents.InvokeEvent_OnActiveCharacterChanged(chr);
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

                    GameEvents.InvokeEvent_OnHoverObject(objectHit.GetComponent<HoverInfo>());

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
                    GameEvents.InvokeEvent_OnHoverObject(objectHit.GetComponent<HoverInfo>());
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
            
            GameEvents.InvokeEvent_OnCharacterJoinedParty(character, this);

            character.UI.StatsUI.Refresh();
            character.CurrHitPoints = character.GetMaxHitPoints();
            character.CurrSpellPoints = character.GetMaxSpellPoints();

            character.UI.SkillsUI.RepositionSkillRows();
            character.UI.SkillsUI.Repaint(character.SkillPoints);
        }

        public void RemoveCharacter(Character character)
        {
            if (!Characters.Remove(character))
            {
                Logger.LogError("Attempting to remove nonexisting character from party");
                return;
            }

            GameEvents.InvokeEvent_OnCharacterLeftParty(character, this);

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
            if (hitInfo.PreferredClass != CharacterClass.None)
            {
                List<Character> preferredCharacters = new List<Character>();
                foreach (Character character in Characters)
                {
                    if (character.Class == hitInfo.PreferredClass)
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
                hitCharacter.BaseResistances, // TODO: not like this
                hitCharacter.GetActualArmorClass(),
                hitCharacter.GetActualLuck());
            result.Victim = this.gameObject;
            /*if (result.Type == AttackResultType.Miss)
            {
                return result;
            }*/

            hitCharacter.AddCurrHitPoints(-1 * result.DamageDealt);
            if (hitCharacter.CurrHitPoints <= 0)
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

                GameEvents.InvokeEvent_OnFoundGold(loot.GoldAmount);
            }

            if (loot.Item != null || loot.GoldAmount > 0)
            {
                GameEvents.InvokeEvent_OnPickedUpLoot(loot);
            }
        }

        public void AddGold(int amount)
        {
            Gold += amount;

            GameEvents.InvokeEvent_OnGoldChanged(Gold - amount, Gold, amount);
        }

        public void AddFood(int amount)
        {
            Food += amount;
        }

        public Character GetActiveOrFirstCharacter()
        {
            if (ActiveCharacter != null && ActiveCharacter.IsRecovered())
            {
                return ActiveCharacter;
            }

            return Characters[0];
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

        // Can return null if not holding anything
        public Item GetHeldItem()
        {
            return UiMgr.Instance.m_HeldItem?.Item;
        }

        // @item can be null - this way it just clears the held item slot
        public void SetHeldItem(Item item)
        {
            RemoveHeldItem();

            if (item != null)
            {
                UiMgr.Instance.SetHeldItem(item);
            }
        }

        public void RemoveHeldItem()
        {
            Item currHeldItem = GetHeldItem();
            if (currHeldItem != null)
            {
                GameObject.Destroy(UiMgr.Instance.m_HeldItem.gameObject);
                UiMgr.Instance.m_HeldItem = null;
            }
        }

        public bool IsHoldingItem()
        {
            return GetHeldItem() != null;
        }

        //=========================================================================================
        // PARTY UPDATE ROUTINES
        //=========================================================================================

        public void Update()
        {
            // TODO: Make some init method
            if (!PartyTime.IsValid()) PartyTime.GameSeconds = TimeMgr.GetCurrentTime().GameSeconds;
            if (!LastRegenTickTime.IsValid()) LastRegenTickTime.GameSeconds = TimeMgr.GetCurrentTime().GameSeconds;
            if (!LastWaterLavaTickTime.IsValid()) LastWaterLavaTickTime.GameSeconds = TimeMgr.GetCurrentTime().GameSeconds;

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

            DoPeriodEffects();

            PartyBuffUI.Refresh();

            // Handle party buff expiration (Fly, WaterWalk, Haste, ...)
            foreach (var partyBuffPair in PartyBuffMap)
            {
                SpellEffect buff = partyBuffPair.Value;
                PartyEffectType buffType = partyBuffPair.Key;

                bool justExpired = buff.IsApplied() && buff.IsExpired();
                if (justExpired)
                {
                    switch (buffType)
                    {
                        case PartyEffectType.Haste:
                            Characters.ForEach(chr =>
                            {
                                chr.SetCondition(Condition.Weak, false);
                            });
                            break;
                    }

                    buff.Reset();
                }
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

            if (Input.GetButton("Attack") && IsHoldingItem())
            {
                // TODO: Handle this more elegantly, this should not know anything about UiMgr
                // Throw the item 
                Ray throwRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.8f, 0.0f));
                ItemMgr.ThrowItem(transform, throwRay.direction, GetHeldItem());
                RemoveHeldItem();
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

        private void DoPeriodEffects()
        {
            long currGameSeconds = TimeMgr.GetCurrentTime().GameSeconds;

            long oldDay = PartyTime.GetDayOfMonth();
            long oldHour = PartyTime.GetHoursOfDay();

            PartyTime.GameSeconds = TimeMgr.GetCurrentTime().GameSeconds;

            // Check for new days effects
            if (PartyTime.GetHoursOfDay() >= 3 && (oldHour < 3 || PartyTime.GetDayOfMonth() > oldDay))
            {
                // New day
                Debug.Log("New day");

                DaysPlayedWithoutRest++;
                if (DaysPlayedWithoutRest > 1)
                {
                    Characters.ForEach(chr =>
                    {
                        chr.SetCondition(Condition.Weak, false);
                    });

                    if (Food > 0)
                    {
                        // Consume food
                    }
                    else
                    {
                        // Or suffer health loss
                        Characters.ForEach(chr =>
                        {
                            chr.CurrHitPoints = (chr.CurrHitPoints / (DaysPlayedWithoutRest + 1)) + 1;
                        });
                    }
                }

                // Long time not resting - suffer bad effects (Dead / Insane)
                if (DaysPlayedWithoutRest > 3)
                {
                    Characters.ForEach(chr =>
                    {
                        if (!chr.IsPetrified() && !chr.IsDead() && !chr.IsEradicated())
                        {
                            if (UnityEngine.Random.Range(0, 100) < 5 * DaysPlayedWithoutRest)
                            {
                                chr.SetCondition(Condition.Dead, false);
                            }
                            if (UnityEngine.Random.Range(0, 100) < 10 * DaysPlayedWithoutRest)
                            {
                                chr.SetCondition(Condition.Insane, false);
                            }
                        }
                    });
                }
            }

            // Try to do water damage tick, tick = every 2 minutes (= 1 realtime second)
            if (IsOnWater && (LastWaterLavaTickTime.GameSeconds + 120 < currGameSeconds))
            {
                LastWaterLavaTickTime.GameSeconds = currGameSeconds;

                Characters.ForEach(chr =>
                {
                    bool canWalkOnWater = PartyBuffMap[PartyEffectType.WaterWalk].IsActive() ||
                                          PartyBuffMap[PartyEffectType.Levitate].IsActive();
                    // TODO: Items can also prevent character from drowning

                    if (canWalkOnWater)
                    {
                        // Do this also when levitating ?
                        chr.PlayCharacterExpression(CharacterExpression.WaterWalkOk);
                    }
                    else
                    {
                        // Receive damage - 0.1 * maxhealth
                        // Status text: You're drowning!
                    }
                });
            }

            // Try to do lava damage tick, tick = every 2 minutes (= 1 realtime second)
            if (IsOnLava && (LastWaterLavaTickTime.GameSeconds + 120 < currGameSeconds))
            {
                LastWaterLavaTickTime.GameSeconds = currGameSeconds;

                Characters.ForEach(chr =>
                {
                    bool canWalkOnLava = PartyBuffMap[PartyEffectType.Levitate].IsActive();
                    if (!canWalkOnLava)
                    {
                        // Receive damage - 0.1 * maxhealth
                        // Status text: On fire!
                    }
                });
            }

            // Try to regenerate
            if (LastRegenTickTime.GameSeconds + 300 < currGameSeconds)
            {
                LastRegenTickTime.GameSeconds = currGameSeconds;

                // Handle immolation aura here also (5 min interval)

                Characters.ForEach(chr =>
                {
                    // Regen HP from item
                    if (chr.WearsItemWithBonusStat(StatType.HpRegeneration))
                    {
                        bool canRegenHp = !chr.IsDead() && !chr.IsEradicated() &&
                                          chr.CurrHitPoints < chr.GetMaxHitPoints();
                        if (canRegenHp)
                        {
                            chr.CurrHitPoints++;
                        }
                        if (chr.CurrHitPoints > 0 && chr.Conditions[Condition.Unconcious].IsValid())
                        {
                            chr.Conditions[Condition.Unconcious].Reset();
                        }
                    }

                    // Regen MP from item
                    if (chr.WearsItemWithBonusStat(StatType.SpRegeneration))
                    {
                        bool canRegenMana = !chr.IsDead() && !chr.IsEradicated() &&
                                            chr.CurrSpellPoints < chr.GetMaxSpellPoints();
                        if (canRegenMana)
                        {
                            chr.CurrSpellPoints++;
                        }
                    }
                });
            }
        }
    }
}