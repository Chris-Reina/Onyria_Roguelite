using System.Collections.Generic;

public static class Managers
{
    private static readonly Dictionary<System.Type, IManager> _managers = new Dictionary<System.Type, IManager>();

    public static T GetManager<T>() where T : IManager, new()
    {
        if (!_managers.ContainsKey(typeof(T)))
        {
            T newManager = new T();
            newManager.Initialize();
            _managers[typeof(T)] = newManager;
        }

        return (T)_managers[typeof(T)];
    }

    public static void Clear()
    {
        _managers.Clear();
    }
}
