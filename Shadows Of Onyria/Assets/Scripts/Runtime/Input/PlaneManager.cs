using UnityEngine;

namespace DoaT
{
    public class PlaneManager
    {
        private readonly TheodenController _player;
        private Plane _plane;

        public Plane Plane => _plane;

        public PlaneManager(TheodenController player)
        {
            _player = player;
            _plane = new Plane(Vector3.up, _player.transform.position);

            _player.OnPositionChange += UpdatePlanePosition;
        }

        private void UpdatePlanePosition(IGridEntity gridEntity)
        {
            _plane.SetNormalAndPosition(Vector3.up, _player.Position);
        }
    }
}
