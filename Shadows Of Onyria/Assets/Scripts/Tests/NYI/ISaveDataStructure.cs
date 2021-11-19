namespace DoaT.Save
{
    public interface ISaveDataStructure<in T>
    {
        void SaveFromAsset(T input);
        void LoadIntoAsset(T output);
    }
}