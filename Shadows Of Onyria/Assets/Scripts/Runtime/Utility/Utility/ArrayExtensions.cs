namespace DoaT
{
    public static class ArrayExtensions
    {
        public static T Random<T>(this T[] collection)
        {
            return collection[UnityEngine.Random.Range(0, collection.Length)];
        }
    }
}