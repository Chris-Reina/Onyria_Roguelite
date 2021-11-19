using System;

namespace DoaT
{
    /// <summary>
    /// Will have specific CharacterCustomizations that will be applied on demand.
    /// </summary>
    [Serializable]
    public class SoulLevelData
    {
        public CharacterCustomization bodySlot;
        public CharacterCustomization dashSlot;
        public CharacterCustomization mainAttackSlot;
        public CharacterCustomization rangeAttackSlot;
    }
}