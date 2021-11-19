using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DoaT
{
    public abstract class BaseSceneEventListener : MonoBehaviour, ISceneEventListener
    {
        protected readonly List<ISceneEventCondition> _conditionals = new List<ISceneEventCondition>();
        
        [SerializeField] protected bool _canReact = true;
        [SerializeField] protected SceneEvent _sceneEvent;
        
        public virtual SceneEvent ListenedSceneEvent => _sceneEvent;
        
        public virtual bool CanReact => _canReact && gameObject.activeInHierarchy && GetConditionalsCanOperateStatus();

        protected virtual void Awake()
        {
            if (_sceneEvent == null)
                DebugManager.LogWarning(
                    $"Listener of type {GetType()} in object {name} has no SceneEvent and will not be subscribed.");
            else
                EventManager.Subscribe(_sceneEvent, OnEventTriggered);
        }
        
        public abstract void OnEventTriggered(params object[] parameters);
        
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

        protected bool GetConditionalsCanOperateStatus()
        {
            return _conditionals.Count == 0 || _conditionals.All(t => t.CanOperate);
        }

        protected virtual void OnDestroy()
        {
            EventManager.Unsubscribe(_sceneEvent, OnEventTriggered);
        }
    }
}
