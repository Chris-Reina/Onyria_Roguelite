// ReSharper disable InconsistentNaming

namespace DoaT
{
    public static class GameEvents
    {
        public const string SpawnZombieSoul = "SpawnZombieSoul";
        public const string SpawnCrystalSkullSoul = "SpawnCrystalSkullSoul";
        public const string SpawnDemonSoul = "SpawnDemonSoul";
        public const string SpawnBansheeSoul = "SpawnBansheeSoul";
        
        /// <summary>
        /// Params: (IInteractable interactable, string message)
        /// </summary>
        public const string OnInteractableAdd = "OnInteractableAdd";
        /// <summary>
        /// Params: IInteractable
        /// </summary>
        public const string OnInteractableRemove = "OnInteractableRemove";

        /// <summary>
        /// Params:
        /// </summary>
        public const string OnSceneUnload = "OnSceneUnload";
        
        /// <summary>
        /// Parameters: Vector3(Location), Item(Spawnned Item)
        /// </summary>
        public const string OnWorldLootSpawn = "OnWorldLootSpawn";


        /// <summary>
        /// Parameters: Item(Picked Item), Action (Dispose)
        /// </summary>
        public const string OnItemPickUp = "OnItemPickUp";
    }

    public static class PlayerEvents
    {

        public const string OnLevelConfirmation = "PlayerOnLevelConfirmation";
        
        /// <summary>
        /// Paramteres: SColor colorData
        /// </summary>
        public const string OnMainAttackChange = "PlayerOnMainAttackChange";
        
        /// <summary>
        /// Paramteres: SColor colorData
        /// </summary>
        public const string OnRangeAttackChange = "PlayerOnRangeAttackChange";
        
        public const string OnDisableInputs = "PlayerOnDisableInputs";
        public const string OnEnableInputs = "PlayerOnEnableInputs";

        /// <summary>
        /// [UI event] Parameters: Attribute
        /// </summary>
        public const string OnAttributeChange = "PlayerOnAttributeChange";

        /// <summary>
        /// Params: (Vector3 location, float Amount)
        /// </summary>
        public const string OnHeal = "PlayerOnHeal";
        
        /// <summary>
        /// Params: (Vector3 location, float Damage)
        /// </summary>
        public const string OnDamageTaken = "PlayerOnDamageTaken";
        
        //public const string | = "Player";
    }

    public static class EntityEvents
    {
        /// <summary>
        /// Params: (Vector3 location, IAttackable target, float Damage, bool isCritical, AttackInfo info)
        /// </summary>
        public const string OnDamageTaken = "OnEntityDamageTaken";
        
        /// <summary>
        /// Params: (Vector3 location)
        /// </summary>
        public const string OnDeath = "OnEntityDeath";
    }

    public static class UIEvents
    {
        public const string OnUISoulNeeded = "UIOnUIsoulNeeded";

        public const string OnVendorSlotSelected = "UIOnVendorSlotSelected";
        //MODIFIER
        public const string TESTOnBodySoulAdded = "UITESTOnBodySoulAdded";
        public const string TESTOnBodySoulRemoved = "TESTOnBodySoulRemoved";

        /// <summary>
        /// Params: (SoulSlotType type, UISoul uiSoul)
        /// </summary>
        public const string OnSoulSlotLoad = "UIOnSoulSlotLoad";

        /// <summary>
        /// Params: ()
        /// </summary>
        public const string OnSoulWindowApply = "UIOnSoulWindowApply";
        
        /// <summary>
        /// Params: (string message, bool show)
        /// </summary>
        public const string OnInteractableUpdate = "UIOnInteractableUpdate";
        
        /// <summary>
        /// Parameters: ITargetable
        /// </summary>
        public const string OnSlotPointerEnter = "UIOnSlotPointerEnter";

        /// <summary>
        /// Parameters: ITargetable
        /// </summary>
        public const string OnSlotPointerExit = "UIOnSlotPointerExit";
        
        /// <summary>
        /// Parameters: (ITargetable container, Soul contained)
        /// </summary>
        public const string OnSoulSelected = "UIOnSoulSelected";
        
        /// <summary>
        /// Parameters: (ITargetable oldContainer, ITargetable newContainer )
        /// </summary>
        public const string OnSoulDropped = "UIOnSoulDropped";
        
        /// <summary>
        /// Parameters: (ITargetable target)
        /// </summary>
        public const string OnTargetableClicked = "UIOnTargetableClicked";
    }

    public static class ItemEvents
    {
        /// <summary>
        /// Parameters: (int Amount)
        /// </summary>
        public const string HealthPotion = "ItemHealthPotion";
        public const string DarknessPotion = "ItemDarknessPotion";
        
        /// <summary>
        /// Parameters: (int Amount, float duration)
        /// </summary>
        public const string AgilityPotion = "ItemHealthPotion";
        public const string StrengthPotion = "ItemHealthPotion";
        public const string VitalityPotion = "ItemHealthPotion";
        public const string SpiritPotion = "ItemHealthPotion";
    }
}
