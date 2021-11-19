using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DoaT
{
    public abstract class BaseSceneEventListenerDual : MonoBehaviour, ISceneEventListenerDual
    {
        protected readonly List<ISceneEventCondition> _conditionals = new List<ISceneEventCondition>();

        [SerializeField] protected bool _ignoreDebugs;
        [SerializeField] protected bool _canReact = true;
        [SerializeField] protected SceneEvent _sceneEvent;
        [SerializeField] protected SceneEvent _sceneEventDual;
        
        public virtual SceneEvent ListenedSceneEvent => _sceneEvent;
        public virtual SceneEvent ListenedSceneEventDual => _sceneEventDual;

        public virtual bool CanReact => _canReact && gameObject.activeInHierarchy && GetConditionalsCanOperateStatus();

        protected virtual void Awake()
        {
            if (!_ignoreDebugs && (_sceneEvent == null || _sceneEventDual == null))
                DebugManager.LogWarning(
                    $"Listener of type {GetType()} in object {name} has no SceneEvent and will not be subscribed.");
            else
            {
                if(_sceneEvent != null) EventManager.Subscribe(_sceneEvent, OnEventTriggered);
                if(_sceneEventDual != null) EventManager.Subscribe(_sceneEventDual, OnEventTriggeredDual);
            }
        }
        
        public abstract void OnEventTriggered(params object[] parameters);
        public abstract void OnEventTriggeredDual(params object[] parameters);
        
        public virtual void AddConditional(ISceneEventCondition condition)
        {
            if (_conditionals.Contains(condition)) return;
            
            _conditionals.Add(condition);
        }

        public virtual void RemoveConditional(ISceneEventCondition condition)
        {
            if (!_conditionals.Contains(condition)) return;

            _conditionals.Remove(condition);
        }

        protected virtual bool GetConditionalsCanOperateStatus()
        {
            return _conditionals.Count == 0 || _conditionals.All(t => t.CanOperate);
        }

        protected virtual void OnDestroy()
        {
            if(_sceneEvent != null) EventManager.Unsubscribe(_sceneEvent, OnEventTriggered);
            if(_sceneEventDual != null) EventManager.Unsubscribe(_sceneEventDual, OnEventTriggeredDual);
        }
    }
}
