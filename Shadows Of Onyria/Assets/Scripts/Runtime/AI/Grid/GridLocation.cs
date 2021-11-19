using System;
using UnityEngine;

namespace DoaT
{
    [Serializable]
    public class GridLocation
    {
        public Vector3 position;
        public Vector3 forward;

        public GridLocation(Vector3 position, Vector3 forward)
        {
            this.position = position;
            this.forward = forward;
        }

        public void Update(Vector3 position, Vector3 forward)
        {
            this.position = position;
            this.forward = forward;
        }
    }
}