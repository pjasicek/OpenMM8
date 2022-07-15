using Assets.OpenMM8.Scripts.Gameplay.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    

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

        public List<Monster> EnemyMonstersInMeleeRange = new List<Monster>();
        public List<Monster> EnemyMonstersInRangedRange = new List<Monster>();

        // Misc
        public float AttackDelayTimeLeft = 0.0f;

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
        public PartyUI PartyUI;

        public FirstPersonController Controller;

        // ========================================================================================

        private void Awake()
        {
            foreach (PartyEffectType effect in Enum.GetValues(typeof(PartyEffectType)))
            {
                PartyBuffMap.Add(effect, new SpellEffect());
            }

            Controller = GetComponent<FirstPersonController>();
        }

        private void Start()
        {
            
        }

        // Called from GameCore initialization
        public void Initialize()
        {
            HostilityChecker = GetComponent<HostilityChecker>();
            PlayerAudioSource = transform.Find("FirstPersonCharacter").GetComponent<AudioSource>();

            //=====================================================================================
            // PartyUI and PartyBuffUI
            //=====================================================================================
            PartyBuffUI = PartyBuffUI.Create(this);
            PartyUI = PartyUI.Create(this);
        }

        public void ResetParty()
        {
            // TODO
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

                if (!onlyRecovered || (Characters[tryCharIndex].IsRecovered() && Characters[tryCharIndex].CanAct()))
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
            if (ActiveCharacter != null && ActiveCharacter.IsRecovered() && ActiveCharacter.CanAct())
            {
                Monster victim = null;

                // 1) Try to attack NPC which is being targeted by the crosshair
                //    - does not have to be enemy, when we are aiming with attack
                //    directly on NPC, we can attack guard / villager this way
                RaycastHit hit;
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.595F, 0));

                // TODO: Verify that this is OK
                float maxDistanceFromMonster = Mathf.Sqrt(Monster.MAX_RANGE_DISTANCE_SQR) + 5.0f;
                if (Physics.Raycast(ray, out hit, maxDistanceFromMonster, 1 << LayerMask.NameToLayer("NPC")))
                {
                    Transform objectHit = hit.collider.transform;
                    Monster hitObjectAsMonster = objectHit.GetComponent<Monster>();

                    if ((hitObjectAsMonster != null) /*&&
                        (EnemyMonstersInMeleeRange.Contains(hitObjectAsMonster))*/)
                    {
                        victim = hitObjectAsMonster;
                    }
                }

                // 2) Try to attack enemy which is closest to Player
                if ((victim == null) && (EnemyMonstersInMeleeRange.Count > 0))
                {
                    EnemyMonstersInMeleeRange.RemoveAll(t => t == null);
                    EnemyMonstersInMeleeRange.OrderBy(t => (t.transform.position - transform.position).sqrMagnitude);
                    foreach (Monster enemyMonster in EnemyMonstersInMeleeRange)
                    {
                        if (enemyMonster.GetComponent<Renderer>().isVisible)
                        {
                            victim = enemyMonster;
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

                if (victim != null)
                {
                    Debug.Log("Should attack: " + victim.Name);
                }

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

            Characters.Add(character);
            
            //GameEvents.InvokeEvent_OnCharacterJoinedParty(character, this);

            /*character.UI.StatsUI.Refresh();
            character.CurrHitPoints = character.GetMaxHitPoints();
            character.CurrSpellPoints = character.GetMaxSpellPoints();

            character.UI.SkillsUI.RepositionSkillRows();
            character.UI.SkillsUI.Repaint(character.SkillPoints);*/

            PartyUI.UpdateLayout();
        }

        public void RemoveCharacter(Character character)
        {
            if (!Characters.Remove(character))
            {
                Logger.LogError("Attempting to remove nonexisting character from party");
                return;
            }
            character.UI.Destroy();

            //GameEvents.InvokeEvent_OnCharacterLeftParty(character, this);

            if (ActiveCharacter == character)
            {
                // Update will decide next active character
                ActiveCharacter = null;
            }

            PartyUI.UpdateLayout();
        }

        

        //---------------------------------------------------------------------
        // Triggers
        //---------------------------------------------------------------------

        [Obsolete]
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

        [Obsolete]
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

        [Obsolete]
        private void OnObjectLeftInteractibleRange(GameObject other)
        {
            
        }

        [Obsolete]
        private void OnObjectEnteredInteractibleRange(GameObject other)
        {
            
        }

        [Obsolete]
        public void OnObjectEnteredMeleeRange(GameObject other)
        {
            if (HostilityChecker.IsHostileTo(other))
            {
                EnemiesInMeleeRange.Add(other);
                UpdateAgroStatus();
            }

            ObjectsInMeleeRange.Add(other);
        }

        [Obsolete]
        public void OnObjectLeftMeleeRange(GameObject other)
        {
            EnemiesInMeleeRange.Remove(other);
            UpdateAgroStatus();

            ObjectsInMeleeRange.Remove(other);
        }

        [Obsolete]
        public void OnObjectEnteredAgroRange(GameObject other)
        {
            if (HostilityChecker.IsHostileTo(other))
            {
                EnemiesInAgroRange.Add(other);
                UpdateAgroStatus();
            }
        }

        [Obsolete]
        public void OnObjectLeftAgroRange(GameObject other)
        {
            EnemiesInAgroRange.Remove(other);
            UpdateAgroStatus();
        }

        private void UpdateAgroStatus()
        {
            if (EnemyMonstersInMeleeRange.Count > 0)
            {
                UpdatePartyAgroStatus(AgroState.HostileClose);
            }
            else if (EnemyMonstersInRangedRange.Count > 0)
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
            // Status text and character expression
            if (loot.Item != null || loot.GoldAmount > 0)
            {
                string lootString;
                if (loot.Item == null)
                {
                    lootString = string.Format("You found {0} gold!",
                        loot.GoldAmount);
                }
                else if (loot.GoldAmount == 0)
                {
                    lootString = string.Format("You found an item ({0})!",
                        loot.Item.Data.NotIdentifiedName);
                }
                else
                {
                    lootString = string.Format("You found {0} gold and an item ({1})!",
                        loot.GoldAmount,
                        loot.Item.Data.NotIdentifiedName);
                }
                GameCore.SetStatusBarText(lootString);

                GetMostRecoveredCharacter().PlayEventReaction(CharacterReaction.FoundGold);
                SoundMgr.PlaySoundById(SoundType.FoundLoot, PlayerAudioSource);
            }

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
                        GameCore.ThrowItem(transform, throwRay.direction, UiMgr.Instance.m_HeldItem.Item);
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

            GameEvents.InvokeEvent_OnPickedUpLoot(loot);
        }

        public void AddGold(int amount)
        {
            Debug.Log("Added gold");
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

        public Vector3 GetProjectileSpawnPos(int characterIndex)
        {
            float offsetX = (characterIndex - 2) * 0.3f;
            return transform.position + new Vector3(offsetX, 0.1f, 0.0f);
        }

        public bool IsInvisible()
        {
            return PartyBuffMap[PartyEffectType.Invisibility].IsActive();
        }

        // Experience from killing monsters - learning is applied
        // Experience from quests should be separate for each characters, learning is NOT applied there
        public void ReceiveExperience(int experienceAmount)
        {
            int numEligibleCharacters = GetNumConciousCharacters();
            if (numEligibleCharacters == 0)
            {
                return;
            }

            int expPerCharacter = experienceAmount / numEligibleCharacters;
            Characters.ForEach(chr =>
            {
                if (chr.IsUnconcious() || chr.IsDead() || chr.IsPetrified() || chr.IsEradicated())
                {
                    return;
                }

                float expCoeff = 1.0f;
                if (chr.HasSkill(SkillType.Learning))
                {
                    int learningMult = chr.GetSkillLevelMultiplier(SkillType.Learning, 1, 2, 3, 5);
                    expCoeff = 1.0f + (learningMult * chr.GetActualSkillLevel(SkillType.Learning)) / 100.0f;
                }

                chr.Experience += (int)(experienceAmount * expCoeff);
            });
        }

        public int GetNumConciousCharacters()
        {
            int num = 0;
            Characters.ForEach(chr =>
            {
                if (!chr.IsUnconcious() && !chr.IsDead() && !chr.IsPetrified() && !chr.IsEradicated())
                {
                    num++;
                }
            });

            return num;
        }

        public int GetNumCharactersThatCanAct()
        {
            int num = 0;
            Characters.ForEach(chr =>
            {
                if (chr.CanAct())
                {
                    num++;
                }
            });

            return num;
        }

        public bool IsGrounded()
        {
            return Controller.m_CharacterController.isGrounded;
        }

        public bool IsInAir()
        {
            return !IsGrounded();
        }

        //=========================================================================================
        // PARTY UPDATE ROUTINES
        //=========================================================================================

        public void DoUpdate(float timeDelta)
        {
            // TODO: Make some init method
            if (!PartyTime.IsValid()) PartyTime.GameSeconds = TimeMgr.GetCurrentTime().GameSeconds;
            if (!LastRegenTickTime.IsValid()) LastRegenTickTime.GameSeconds = TimeMgr.GetCurrentTime().GameSeconds;
            if (!LastWaterLavaTickTime.IsValid()) LastWaterLavaTickTime.GameSeconds = TimeMgr.GetCurrentTime().GameSeconds;

            // Some things need to be updated even if game is paused, for example character's facial expressions
            float realtimeSinceStartup = Time.realtimeSinceStartup;
            float deltaTime = realtimeSinceStartup - PreviousTimeSinceStartup;
            PreviousTimeSinceStartup = realtimeSinceStartup;

            bool isCastingTargetedSpell = SpellCastHelper.PendingPlayerSpell != null &&
                (SpellCastHelper.PendingPlayerSpell.Flags.HasFlag(CastSpellFlags.TargetNpc) ||
                 SpellCastHelper.PendingPlayerSpell.Flags.HasFlag(CastSpellFlags.TargetCorpse) ||
                 SpellCastHelper.PendingPlayerSpell.Flags.HasFlag(CastSpellFlags.TargetMesh) || // TODO
                 SpellCastHelper.PendingPlayerSpell.Flags.HasFlag(CastSpellFlags.TargetOutdoorItem)); // TODO
            if (Input.GetButtonDown("Attack") && isCastingTargetedSpell)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // TODO: Verify that this is OK
                float maxTargetDistance = Mathf.Sqrt(Monster.MAX_RANGE_DISTANCE_SQR) + 10.0f;
                if (Physics.Raycast(ray, out hit, maxTargetDistance, 1 << LayerMask.NameToLayer("NPC")))
                {
                    Transform objectHit = hit.collider.transform;
                    if ((objectHit.GetComponent<Monster>() != null))
                    {
                        Monster monster = objectHit.GetComponent<Monster>();
                        SpellCastHelper.OnCrosshairClickedOnMonster(monster);   
                    }
                }
            }

            foreach (Character character in Characters)
            {
                character.OnFixedUpdate(deltaTime);
            }

            UpdateAgroStatus();

            // TODO: Make some generic way to determine whether PlayerParty can act ...
            if (GameCore.Instance.IsGamePaused() || Time.timeScale == 0.0f)
            {
                return;
            }

            //==================================================
            // Gameplay update - game is NOT paused here
            //==================================================


            DoPeriodEffects();

            PartyUI.Refresh();
            PartyBuffUI.Refresh();

            // Handle party buff expiration (Fly, WaterWalk, Haste, ...)
            foreach (var partyBuffPair in PartyBuffMap)
            {
                SpellEffect buff = partyBuffPair.Value;
                PartyEffectType buffType = partyBuffPair.Key;

                if (buff.IsAppliedAndExpired())
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
                character.OnUpdate(deltaTime);
            }

            if (ActiveCharacter == null || !ActiveCharacter.IsRecovered() || !ActiveCharacter.CanAct())
            {
                foreach (Character character in Characters)
                {
                    if (character.CanAct() && character.IsRecovered())
                    {
                        Debug.Log("Can act !");
                        SetActiveCharacter(character);
                    }
                    else
                    {
                        //character.UI.SelectionRing.enabled = false;
                    }
                }
            }

            HandleHover();

            if (Input.GetButton("Attack") && IsHoldingItem())
            {
                // TODO: Handle this more elegantly, this should not know anything about UiMgr
                // Throw the item 
                Ray throwRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.8f, 0.0f));
                GameCore.ThrowItem(transform, throwRay.direction, GetHeldItem());
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

            AttackDelayTimeLeft -= deltaTime;
        }

        private void DoPeriodEffects()
        {
            long currGameSeconds = TimeMgr.GetCurrentTime().GameSeconds;

            long oldDay = PartyTime.GetDayOfMonth();
            long oldHour = PartyTime.GetHoursOfDay();

            PartyTime.GameSeconds = TimeMgr.GetCurrentTime().GameSeconds;

            // Check for new days effects
            bool isNewDay = PartyTime.GetHoursOfDay() >= 3 && (oldHour < 3 || PartyTime.GetDayOfMonth() > oldDay);
            if (isNewDay)
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
