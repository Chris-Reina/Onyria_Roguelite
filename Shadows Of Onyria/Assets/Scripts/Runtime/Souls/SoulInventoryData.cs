using System;
using System.Collections.Generic;
using UnityEngine;

namespace DoaT
{
    [Serializable, CreateAssetMenu(menuName = "Data/Souls/Inventory", fileName = "Souls Inventory Data")]
    public class SoulInventoryData : ScriptableObject
    {
        public CharacterController baseDashController;
        public CharacterController baseMainAttackController;
        public CharacterController baseRangeAttackController;
        public CharacterController baseLocomotor;
        
        [NonSerialized] public Soul bodySlotSoul = default;
        [NonSerialized] public Soul dashSlotSoul = default;
        [NonSerialized] public Soul mainAttackSlotSoul = default;
        [NonSerialized] public Soul rangeAttackSlotSoul = default;
        public List<Soul> obtainedSouls;

        public void AssignSoulBySlots()
        {
            bodySlotSoul = default;
            dashSlotSoul = default;
            mainAttackSlotSoul = default;
            rangeAttackSlotSoul = default;
            
            foreach (var soul in obtainedSouls)
            {
                switch (soul.slotID)
                {
                    case SoulSlotType.None:
                        continue;
                    case SoulSlotType.Dash:
                        dashSlotSoul = soul;
                        break;
                    case SoulSlotType.Body:
                        bodySlotSoul = soul;
                        break;
                    case SoulSlotType.MainAttack:
                        mainAttackSlotSoul = soul;
                        break;
                    case SoulSlotType.RangeAttack:
                        rangeAttackSlotSoul = soul;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}