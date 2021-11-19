using System;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public event Action<bool> OnDebugStatusChange;
    
    private static DebugManager Instance { get; set; }
    
    private bool _shouldDebug ;
    [SerializeField] private bool _showDebugElements = default;
    
    public bool ShowDebugElements => _showDebugElements;
    public Color activeDebugColor = Color.green;
    public Color inactiveDebugColor = Color.grey;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else 
        if (Instance != this) Destroy(this);

#if UNITY_EDITOR
        _shouldDebug = true;
#else
        _shouldDebug = false;
#endif
    }
    
    public void ToggleDebugElements()
    {
        _showDebugElements = !_showDebugElements;
        OnDebugStatusChange?.Invoke(_showDebugElements);
    }
    
    public static void Log(object message)
    {
        if (Instance == null) return;
        if (!Instance._shouldDebug) return;
        Debug.Log(message);
    }
    public static void LogWarning(object message)
    {
        if (Instance == null) return;
        if (!Instance._shouldDebug) return;
        Debug.LogWarning(message);
    }
    public static void LogError(object message)
    {
        if (Instance == null) return;
        if (!Instance._shouldDebug) return;
        Debug.LogError(message);
    }
}
