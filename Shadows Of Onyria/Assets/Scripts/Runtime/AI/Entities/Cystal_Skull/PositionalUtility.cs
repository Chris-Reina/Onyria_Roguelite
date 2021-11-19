using UnityEngine;

namespace DoaT.AI
{
    public static class PositionalUtility
    {
        public static bool IsInFront(Vector3 objectivePosition, Vector3 personalPosition, Vector3 personalDirection)
        {
            var directionAb = (objectivePosition - personalPosition).normalized;

            var dot = Vector3.Dot(directionAb, personalDirection);

            return dot > 0;
        }
        
        public static bool IsInFront(Vector3 objectivePosition, Vector3 personalPosition, Vector3 personalDirection, float angle)
        {
            var directionAb = (objectivePosition - personalPosition).normalized;
            var usableAngle = angle / 2;

            return Vector3.Angle(directionAb, personalDirection) < usableAngle;
        }
    }
}