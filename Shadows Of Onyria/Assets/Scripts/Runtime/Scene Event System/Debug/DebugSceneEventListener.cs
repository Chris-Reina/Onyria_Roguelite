using System;
using System.Collections;
using System.Collections.Generic;
using DoaT;
using UnityEngine;

public class DebugSceneEventListener : BaseSceneEventListener
{
    public enum LogType
    {
        Log,
        Warning,
        Error
    }

    public LogType logType;
    public bool useCustomMessage;
    [TextArea] public string CustomDebugMessage;
    
    public override bool CanReact => _canReact;

    public override void OnEventTriggered(params object[] parameters)
    {
        if (!CanReact) return;
        
        switch (logType)
        {
            case LogType.Log:
                DebugManager.Log(useCustomMessage
                    ? CustomDebugMessage
                    : $"DebugListener ({name}): The scene event '{_sceneEvent.name}' has been raised.");
                break;
            case LogType.Warning:
                DebugManager.LogWarning(useCustomMessage
                    ? CustomDebugMessage
                    : $"DebugListener ({name}): The scene event '{_sceneEvent.name}' has been raised.");
                break;
            case LogType.Error:
                DebugManager.LogError(useCustomMessage
                    ? CustomDebugMessage
                    : $"DebugListener ({name}): The scene event '{_sceneEvent.name}' has been raised.");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
