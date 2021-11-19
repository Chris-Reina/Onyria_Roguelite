using System.Collections.Generic;
using UnityEngine;

namespace DoaT.AI
{
    public class PathTrackingBehaviour : SteeringBehaviour
    {
        private IPath _pathData;

        private void Awake()
        {
            _pathData ??= GetComponent<IPath>();
        }

        protected override Vector3 CalculateDirection(List<GameObject> neighbours)
        {
            if (_pathData.CurrentIndex >= _pathData.Path.Count) return new Vector3(0, 0, 0);

            var pathDirection = (_pathData.Path[_pathData.CurrentIndex] - _pathData.Position).normalized;
            return pathDirection;
        }
    }
}