namespace DoaT.Vendor
{
    public interface ISelectableSlot
    {
        bool Selectable { get; }
        
        void Select();
        void Deselect();
    }
}