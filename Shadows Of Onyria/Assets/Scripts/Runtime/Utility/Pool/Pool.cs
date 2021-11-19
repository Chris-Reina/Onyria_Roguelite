using System;
using System.Collections.Generic;
using System.Linq;
using DoaT;

public class Pool<T> : IPool<T> where T : IPoolSpawn
{
    private readonly T _objectToPool;
    private readonly bool _isDynamic;
    private readonly Func<object, T> _factory;
    private readonly List<PoolObject<T>> _internal;

    private readonly HashSet<T> _currentActiveObjects = new HashSet<T>();
    private readonly HashSet<T> _currentInactiveObjects = new HashSet<T>();

    public HashSet<T> CurrentActiveObjects => _currentActiveObjects;
    // public List<T> CurrentActiveObjects => _internal.Where(x => !x.IsAvailable)
    //                                                 .Select(x => x.Object)
    //                                                 .ToList();

    public HashSet<T> CurrentInactiveObjects => _currentInactiveObjects;
    // public List<T> CurrentInactiveObjects => _internal.Where(x => x.IsAvailable)
    //                                                   .Select(x => x.Object)
    //                                                   .ToList();

    public Pool(T objectToPool, int initialStock, Func<object, T> factory, bool isDynamic)
    {
        _objectToPool = objectToPool;
        _factory = factory;
        _isDynamic = isDynamic;
        
        _internal = new List<PoolObject<T>>();

        if (initialStock <= 0) return;
        
        for (var i = 0; i < initialStock; i++)
        {
            var newPoolObj = new PoolObject<T>(_factory(_objectToPool)) { IsAvailable = true };
            _currentInactiveObjects.Add(newPoolObj.Object);
            newPoolObj.Object.SetParentPool(this);
            _internal.Add(newPoolObj);
        }
    }

    public T GetObject()
    {
        if (_internal.Any(x => x.IsAvailable))
        {
            var obj = _internal.First(x => x.IsAvailable).GetObject();
            if (_currentInactiveObjects.Contains(obj)) _currentInactiveObjects.Remove(obj);
            if (!_currentActiveObjects.Contains(obj)) _currentActiveObjects.Add(obj);
            return obj;
        }
        
        if (!_isDynamic) return default;
        
        var newPoolObj = new PoolObject<T>(_factory(_objectToPool)) { IsAvailable = false };
        _currentActiveObjects.Add(newPoolObj.Object);
        newPoolObj.Object.SetParentPool(this);
        _internal.Add(newPoolObj);

        return newPoolObj.GetObject();
    }

    public void ReturnObject(T obj)
    {
        var temp = _internal.First(x => Equals(x.Object, obj));
        if (_currentActiveObjects.Contains(obj)) _currentActiveObjects.Remove(obj);
        if (!_currentInactiveObjects.Contains(obj)) _currentInactiveObjects.Add(obj);
        temp.IsAvailable = true;
    }
}
