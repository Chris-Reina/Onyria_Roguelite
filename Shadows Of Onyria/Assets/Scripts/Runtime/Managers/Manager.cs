using System;
using UnityEngine;

namespace DoaT
{
    public abstract class Manager<T> : IManager where T : ScriptableObject
    {
        protected T _config;

        public Manager() { }

        public virtual void Initialize()
        {
            _config = Resources.LoadAll<T>("")[0];
            if (_config == null)
            {
                throw new Exception($"The configuration file of type {typeof(T)} is missing.");
            }
        }
    }
}
