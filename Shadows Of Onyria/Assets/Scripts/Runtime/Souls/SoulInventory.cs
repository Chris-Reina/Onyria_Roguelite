using System.Collections.Generic;
using UnityEngine;

namespace DoaT
{
    public class SoulInventory : MonoBehaviour 
    {
        private static SoulInventory Instance { get; set; }
        
        public SoulInventoryData data;
        public readonly Dictionary<SoulType, SoulData> soulMap = new Dictionary<SoulType, SoulData>();

        [HideInInspector] public List<CharacterModifier> modifiers;
        
        private bool _assignedThisFrame;

        public void AssignSoulBySlots()
        {
            if (_assignedThisFrame) return;

            _assignedThisFrame = true;
            data.AssignSoulBySlots();
        }

        public ILocomotionBehaviour GetLocomotionController()
        {
            var controller = data.baseLocomotor.GetController<ILocomotionBehaviour>();
            return controller;
        }

        public IDashBehaviour GetDashController()
        {
            IDashBehaviour behaviour;
            if (data.dashSlotSoul != null && soulMap.ContainsKey(data.dashSlotSoul.type))
            {
                if (soulMap[data.dashSlotSoul.type].levelsData[data.dashSlotSoul.level - 1].dashSlot is CharacterController dashController)
                {
                    behaviour = dashController.GetController<IDashBehaviour>();
                    return behaviour;
                }
            }

            behaviour = data.baseDashController.GetController<IDashBehaviour>();
            return behaviour;
        }

        public IAttackBehaviour GetMainAttackController()
        {
            IAttackBehaviour behaviour;
            if (data.mainAttackSlotSoul != null && soulMap.ContainsKey(data.mainAttackSlotSoul.type))
            {
                if (soulMap[data.mainAttackSlotSoul.type].levelsData[data.mainAttackSlotSoul.level - 1].mainAttackSlot is CharacterController attackController)
                {
                    behaviour = attackController.GetController<IAttackBehaviour>();
                    return behaviour;
                }
            }

            behaviour = data.baseMainAttackController.GetController<IAttackBehaviour>();
            return behaviour;
        }

        public IAttackBehaviour GetRangeAttackController()
        {
            IAttackBehaviour behaviour;
            if (data.rangeAttackSlotSoul != null && soulMap.ContainsKey(data.rangeAttackSlotSoul.type))
            {
                if (soulMap[data.rangeAttackSlotSoul.type].levelsData[data.rangeAttackSlotSoul.level - 1]
                    .rangeAttackSlot is CharacterController attackController)
                {
                    behaviour = attackController.GetController<IAttackBehaviour>();
                    return behaviour;
                }
            }

            behaviour = data.baseRangeAttackController.GetController<IAttackBehaviour>();
            return behaviour;
        }

        public bool GetAttributeModifier(out IAttributeModifier modifiers)
        {
            if (data.bodySlotSoul != null && soulMap.ContainsKey(data.bodySlotSoul.type))
            {
                if (soulMap[data.bodySlotSoul.type].levelsData[data.bodySlotSoul.level - 1].bodySlot is
                    IAttributeModifier aM)
                {
                    modifiers = aM;
                    return true;
                }
            }

            modifiers = default;
            return false;
        }


        private void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            var allData = Resources.LoadAll<SoulData>("");
            var allType = Resources.LoadAll<SoulType>("");

            for (int i = 0; i < allType.Length; i++)
            {
                for (int j = 0; j < allData.Length; j++)
                {
                    if (allData[j].type != allType[i]) continue;

                    soulMap.Add(allType[i], allData[j]);
                    break;
                }
            }
        }

        private void LateUpdate()
        {
            _assignedThisFrame = false;
        }   
    }
   

    public abstract class CharacterCustomization : ScriptableObject
    {
    }

    public abstract class CharacterController : CharacterCustomization
    {
        public abstract T GetController<T>() where T : class, IBehaviour;
    }

    public abstract class CharacterModifier : CharacterCustomization
    {
    }
}
