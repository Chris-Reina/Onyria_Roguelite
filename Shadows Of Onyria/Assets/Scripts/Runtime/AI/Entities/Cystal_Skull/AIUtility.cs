using System;
using UnityEngine;

namespace DoaT.AI
{
    public static class AIUtility
    { 
        public static bool IsTargetVisible
        (
            Vector3 targetPosition, 
            Vector3 rayInitPosition,
            Tuple<float, float> viewDistanceAndAngle, 
            Vector3 position, 
            Vector3 forward
        ){
            var (distance, angle) = viewDistanceAndAngle;
            if (Vector3.Distance(targetPosition, position) > distance) return false;
            if (Vector3.Angle(forward, (targetPosition - position)) > angle / 2) return false;

            var ray = new Ray(rayInitPosition, forward);

            if (Physics.Raycast(ray, out var hit, distance, LayersUtility.PLAYER_DETECTION_SIGHT_MASK))
            {
                if (hit.collider.gameObject.layer == LayersUtility.PLAYER_MASK_INDEX)
                    return true;
            }

            return false;
        }
    }
}