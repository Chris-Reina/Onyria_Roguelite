using System.Collections.Generic;
using UnityEngine;

namespace DoaT
{
    public class SetActiveBatchEventListener : BaseSceneEventListener
    {
        [SerializeField] private List<GameObject> _objects;
        [SerializeField] private bool _setActive;

        public override void OnEventTriggered(params object[] parameters)
        {
            if (!CanReact) return;

            for (int i = 0; i < _objects.Count; i++)
            {
                if (_objects[i] == null) continue;
            
                _objects[i].SetActive(_setActive);
            }
        }
    }
}