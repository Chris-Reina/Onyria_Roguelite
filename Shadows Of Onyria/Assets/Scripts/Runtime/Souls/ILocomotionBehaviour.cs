using System;
using UnityEngine;

namespace DoaT
{
    public interface ILocomotionBehaviour : IBehaviour
    {
        event Action<bool> OnMove;

        ILocomotionBehaviour Initialize(IEntity owner, WallDetector detector);
        void Move(Vector3 direction, LocomotionParameters data, bool canWalk);
        void Rotate(RotationData data, LocomotionParameters locomotionParameters);
    }
}