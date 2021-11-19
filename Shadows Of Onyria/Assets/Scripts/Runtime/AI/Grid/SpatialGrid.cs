using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DoaT
{
    public class SpatialGrid
    {
        private readonly Dictionary<IGridEntity, GridLocation> _lastPositions =
            new Dictionary<IGridEntity, GridLocation>();

        private readonly HashSet<IGridEntity> _bucket = new HashSet<IGridEntity>();
        private static readonly Vector3 Outside = new Vector3(-10000, -10000, -10000);

        public void AddGridEntity(IGridEntity e)
        {
            if (_bucket.Contains(e)) return;

            e.OnPositionChange += UpdateEntity;
            UpdateEntity(e);
        }

        public void RemoveGridEntity(IGridEntity e)
        {
            if (!_bucket.Contains(e)) return;

            e.OnPositionChange -= UpdateEntity;
            _bucket.Remove(e);
            _lastPositions.Remove(e);
        }

        private void UpdateEntity(IGridEntity entity)
        {
            var contains = _bucket.Contains(entity);

            var lastPos = contains ? _lastPositions[entity].position : Outside;
            var lastForward = contains ? _lastPositions[entity].forward : Vector3.forward;
            var currentPos = entity.Position.Clone();
            var currentForward = entity.Direction.Clone();

            if (lastPos.Equals(currentPos) && lastForward.Equals(currentForward))
                return;

            if (!contains)
            {
                _bucket.Add(entity);
                _lastPositions.Add(entity, new GridLocation(currentPos, currentForward));
                return;
            }

            _lastPositions[entity].Update(currentPos, currentForward);
        }

        public List<IGridEntity> Query(Vector3 aabbFrom, Vector3 aabbTo, Func<Vector3, bool> filterByPosition)
        {
            var from = new Vector3(Mathf.Min(aabbFrom.x, aabbTo.x), Mathf.Min(aabbFrom.y, aabbTo.y),
                Mathf.Min(aabbFrom.z, aabbTo.z));
            var to = new Vector3(Mathf.Max(aabbFrom.x, aabbTo.x), Mathf.Max(aabbFrom.y, aabbTo.y),
                Mathf.Max(aabbFrom.z, aabbTo.z));

            return _bucket.Where(e =>
                    from.x <= e.Position.x && e.Position.x <= to.x &&
                    from.y <= e.Position.y && e.Position.y <= to.y &&
                    from.z <= e.Position.z && e.Position.z <= to.z
                )
                .Where(n => filterByPosition(n.Position))
                .ToList();
        }

        public void Clear()
        {
            _lastPositions.Clear();
            _bucket.Clear();
        }
    }
}