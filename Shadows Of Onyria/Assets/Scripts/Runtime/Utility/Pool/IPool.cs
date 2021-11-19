namespace DoaT
{
    public interface IPool<T> where T : IPoolSpawn
    {
        T GetObject();
        void ReturnObject(T obj);
    }
}