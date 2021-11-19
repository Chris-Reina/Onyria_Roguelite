using System;
using System.Collections;
using UnityEngine;

namespace DoaT
{
    public interface IDashBehaviour : IBehaviour
    {
        event Action OnDashBegin;
        event Action OnDashEnd;
        
        IDashBehaviour Initialize(IEntity entity, WallDetector detector, Func<IEnumerator, Coroutine> startCoroutine);
        void Dash(Vector3 direction, DashParameters data);
    }
}