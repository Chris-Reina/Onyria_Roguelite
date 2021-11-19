using UnityEngine;

namespace DoaT
{
    public static class LocomotionUtility
    {
        public static Vector3 GetNewDirection(Vector3 oldDirection, float speed, IPositionProperty parentPosition)
        {
            if (Physics.Raycast(parentPosition.Position + oldDirection * (speed * Time.deltaTime) + new Vector3(0, 5, 0),
                Vector3.down,
                out var hit,
                10f,
                LayersUtility.TRAVERSABLE_MASK,
                QueryTriggerInteraction.Collide))
            {
                oldDirection = (hit.point - parentPosition.Position).normalized;
            }

            return oldDirection;
        }
    }
}