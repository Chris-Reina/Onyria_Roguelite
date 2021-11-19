using UnityEngine;

namespace DoaT
{
    public struct RotationData
    {
        public Vector3 Direction { get; }
        public bool Snap { get; }

        public bool IsDefault => Snap == false && Direction == Vector3.zero;
        
        public RotationData(Vector3 direction, bool snap)
        {
            this.Direction = direction;
            this.Snap = snap;
        }
    }
}