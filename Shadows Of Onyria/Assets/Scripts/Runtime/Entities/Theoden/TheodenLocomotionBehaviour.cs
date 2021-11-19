#pragma warning disable CS0067

using System;
using UnityEngine;

namespace DoaT
{
    public class TheodenLocomotionBehaviour : ILocomotionBehaviour
    {
        public event Action<object[]> OnActionCancel;
        public event Action<object[]> OnUpdateRotationRequest;
        public event Action<bool> OnMove;
        
        private IEntity _body;
        private WallDetector _detector;

        public bool IsLocked { get; } = false;
        public bool IsOnCooldown => false;

        public ILocomotionBehaviour Initialize(IEntity owner, WallDetector detector)
        {
            _body = owner;
            _detector = detector;

            return this;
        }
        
        public void Move(Vector3 direction, LocomotionParameters data, bool canWalk)
        {
            if (direction == Vector3.zero)
            {
                OnMove?.Invoke(false);
                return;
            }

            if (canWalk && !_detector.IsColliding)
            {
                _body.Transform.position += direction * (data.movementSpeed * Time.deltaTime);
                OnMove?.Invoke(true);
            }
            else
            {
                OnMove?.Invoke(false);
            }
        }
        public void Rotate(RotationData data, LocomotionParameters locomotionParameters)
        {
            if (data.Direction == Vector3.zero) return;

            var targetRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(data.Direction, Vector3.up), Vector3.up);

            var rotTimeDeg = data.Snap ? 360 : RotationAngleFromCurve(data.Direction, locomotionParameters) * Time.deltaTime;

            _body.Transform.rotation = Quaternion.RotateTowards(_body.Transform.rotation, targetRotation, rotTimeDeg);
        }

        private float RotationAngleFromCurve(Vector3 direction, LocomotionParameters data)
        {
            return data.rotationCurve.Evaluate(Vector3.Angle(direction, _body.Transform.forward) / 180) * data.rotationSpeed * 360;
        }

        public void Unload(params object[] parameters)
        {
            _body = null;
            _detector = null;
        }

        public void OnGamePause() { }
        public void OnGameResume() { }
    }
}
#pragma warning restore CS0067