using System;
using UnityEngine;

namespace DoaT
{
    [Serializable]
    public class LocomotionParameters
    {
        public float movementSpeed;
        public float rotationSpeed;
        public AnimationCurve rotationCurve;

        public LocomotionParameters(LocomotionParameters old)
        {
            movementSpeed = old.movementSpeed;
            rotationSpeed = old.rotationSpeed;
            rotationCurve = old.rotationCurve;
        }

        public LocomotionParameters(float mSpeed, float rotSpeed, AnimationCurve rCurve)
        {
            movementSpeed = mSpeed;
            rotationSpeed = rotSpeed;
            rotationCurve = rCurve;
        }
    }

    [Serializable]
    public class DashParameters
    {
        public float dashDuration;
        public float dashDistance;
        public float dashCooldown;
        
        public DashParameters(DashParameters old)
        {
            dashDuration = old.dashDuration;
            dashDistance = old.dashDistance;
            dashCooldown = old.dashCooldown;
        }
    }
}