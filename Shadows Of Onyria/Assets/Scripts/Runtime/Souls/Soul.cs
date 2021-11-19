using System;

namespace DoaT
{
    /// <summary>
    /// Serializable object that will define the type and level of an specific soul. Behaviours and specific data will
    /// be later defined in a ScriptableObject that will have a SoulType which will be used to load the behaviours
    /// matching them with the type in this class.
    /// </summary>
    [Serializable]
    public class Soul
    {
        public SoulType type;
        public int level;
        public bool IsInSlot => slotID != SoulSlotType.None;
        public SoulSlotType slotID = SoulSlotType.None;

        public Soul(int level, SoulType type)
        {
            this.type = type;
            this.level = level;
        }

        public Soul(int level, SoulType type, int slotID)
        {
            this.type = type;
            this.level = level;
            this.slotID = (SoulSlotType) slotID;
        }
    }
}
