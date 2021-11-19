namespace DoaT
{
    public interface IAttributeModifier
    {
        void Modify(TheodenData data);
        void Restore(TheodenData data);
    }
}
