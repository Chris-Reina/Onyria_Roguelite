using UnityEngine;

namespace DoaT
{
    public abstract class DataVariable<T> : ScriptableObject
    {
        public abstract T Value { get; }
    }
}