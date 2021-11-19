using System;
using UnityEngine;

namespace DoaT
{
    public sealed class AreaSceneEventTrigger : BaseSceneEventTrigger
    {
        private enum TriggerType
        {
            Enter,
            Exit,
            Stay
        }
        
#pragma warning disable 649
        [SerializeField] private TriggerType eventType;
        [SerializeField] private LayerMask affectedLayers;
#pragma warning restore 649
        
        public override bool CanTrigger => GetConditionalsCanOperateStatus();

        public override void Trigger()
        {
            if(!CanTrigger) return;
            EventManager.Raise(_sceneEvent);
        }

        public void OnTriggerEnter(Collider other)
        {
            
            if (eventType != TriggerType.Enter) return;
            
            if(affectedLayers.ContainsLayer(other.gameObject.layer))
            {
                Trigger();
            }
        }
        public void OnTriggerStay(Collider other)
        {
            if (eventType != TriggerType.Stay) return;
            
            if(affectedLayers.ContainsLayer(other.gameObject.layer))
            {
                Trigger();
            }
        }
        public void OnTriggerExit(Collider other)
        {
            if (eventType != TriggerType.Exit) return;
            
            if(affectedLayers.ContainsLayer(other.gameObject.layer))
            {
                Trigger();
            }
        }
    }
}
