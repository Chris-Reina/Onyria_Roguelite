using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DoaT
{
    public abstract class BaseSceneEventTrigger : MonoBehaviour, ISceneEventTrigger
    {
        [SerializeField] protected bool _canTrigger = true;
        [SerializeField] protected SceneEvent _sceneEvent;
        protected readonly List<ISceneEventCondition> _conditionals = new List<ISceneEventCondition>();

        public virtual bool CanTrigger => _canTrigger && gameObject.activeInHierarchy && GetConditionalsCanOperateStatus();
        public virtual SceneEvent TriggeredSceneEvent => _sceneEvent;
        
        public abstract void Trigger();
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
    }
}
