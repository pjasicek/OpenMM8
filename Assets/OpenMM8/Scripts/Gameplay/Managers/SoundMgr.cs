using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay.Managers
{
    class SoundMgr : Singleton<SoundMgr>
    {
        // Public

        //=================================== Unity Lifecycle ===================================

        private void Awake()
        {
            // Events
            PlayerParty.OnCharacterJoinedParty += OnCharacterJoinedParty;
            PlayerParty.OnCharacterLeftParty += OnCharacterLeftParty;
            PlayerParty.OnGoldChanged += OnGoldChanged;
            PlayerParty.OnFoodChanged += OnFoodChanged;

            Character.OnHitNpc += OnCharHitNpc;
        }

        // Init sequence: DbMgr(1) -> GameMgr(1) -> *Mgr(1) -> GameMgr(2)
        public bool Init()
        {
            return true;
        }

        private void OnCharacterJoinedParty(Character chr, PlayerParty party)
        {

        }

        private void OnCharacterLeftParty(Character chr, PlayerParty party)
        {

        }

        private void Start()
        {

        }

        //=================================== Methods ===================================


        //=================================== Events ===================================

        public void OnGoldChanged(int oldGold, int newGold, int delta)
        {

        }

        public void OnFoodChanged(int oldFood, int newFood, int delta)
        {

        }

        public void OnCharConditionChanged(Character chr, Condition newCondition)
        {

        }

        public void OnCharHitNpc(Character chr, AttackInfo attackInfo, AttackResult result)
        {
         
        }

        public void OnCharGotHit(Character chr, AttackInfo attackInfo, AttackResult attackResult)
        {

        }

        public void OnNpcInspect(Character inspectorChr, NpcData npcData)
        {

        }

        public void OnItemInspect(Character inspectorChr, ItemData itemData/*, InspectResult result*/)
        {

        }

        public void OnItemEquip(/*Item item, EquipResult equipResult*/)
        {

        }
    }
}
