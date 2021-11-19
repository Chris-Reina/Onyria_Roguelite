using System.Collections.Generic;
using UnityEngine;

namespace DoaT
{
    public abstract class BaseSceneEventCondition : MonoBehaviour, ISceneEventCondition
    {
        protected readonly List<ISceneEventComponent> _eventComponents = new List<ISceneEventComponent>();

        public abstract bool CanOperate { get; }
        
        protected virtual void Awake()
        {
            _eventComponents.AddRange(GetComponents<ISceneEventComponent>());

            if (_eventComponents.Count == 0) return;
            
            foreach (var component in _eventComponents)
            {
                component.AddConditional(this);
            }
        }

        protected virtual void OnDestroy()
        {
            for (int i = 0; i < _eventComponents.Count; i++)
            {
                if (_eventComponents[i] == default) continue;
                
                _eventComponents[i].RemoveConditional(this);
            }
        }
    }
}
