using System;
using UnityEngine;

namespace DoaT.AI
{
    public class PathRequest
    {
        public readonly Func<Vector3> initialPosition;
        public readonly Func<Vector3> targetPosition;
        public readonly Action<Path> output;

        public PathRequest(Action<Path> output, Func<Vector3> initialPosition, Func<Vector3> targetPosition)
        {
            this.output = output;
            this.initialPosition = initialPosition;
            this.targetPosition = targetPosition;
        }
    }
}