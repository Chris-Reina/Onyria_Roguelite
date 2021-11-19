#pragma warning disable CS0067

using System;
using UnityEngine;
using System.Collections;

namespace DoaT
{
    [Serializable, CreateAssetMenu(menuName = "Abilities/Concrete/Skull/Dash", fileName = "Controller_SkullDash")]
    public class SkullSoulDash : CharacterController, IClone<SkullSoulDashBehaviour>
    {
        public SkullSoulDashBehaviour Clone()
        {
            return new SkullSoulDashBehaviour();
        }

        public override T GetController<T>()
        {
            return Clone() as T;
        }
    }

    public sealed class SkullSoulDashBehaviour : IDashBehaviour
    {
        public event Action<object[]> OnActionCancel;
        public event Action<object[]> OnUpdateRotationRequest;
        public event Action OnDashBegin;
        public event Action OnDashEnd;
        
        private IEntity _body;
        private TheodenController _player;
        private WallDetector _detector;
        private Func<IEnumerator,Coroutine> _startCoroutine;
            
        private readonly TimerHandler _dashCooldownTimer = new TimerHandler();
        private readonly TimerHandler _dashEffectDuration = new TimerHandler();
        
        public bool IsOnCooldown => _dashCooldownTimer.IsActive;
        public bool IsLocked { get; private set; }
        public void Unload(params object[] parameters) { }
        
        
        public IDashBehaviour Initialize(IEntity entity, WallDetector detector, Func<IEnumerator, Coroutine> startCoroutine)
        {
            _startCoroutine = startCoroutine;
            _detector = detector;
            _body = entity;

            _player = _body as TheodenController;
            
            return this;
        }
        
        public void Dash(Vector3 direction, DashParameters data)
        {
            OnUpdateRotationRequest?.Invoke(new object[0]);
            TimerManager.SetTimer(_dashEffectDuration, () => SetCooldown(data), data.dashDuration);
            _startCoroutine?.Invoke(DashMovement(direction.Clone(), data.dashDistance / data.dashDuration * 0.0001f));
        }
        
        private void SetCooldown(DashParameters data)
        {
            TimerManager.SetTimer(_dashCooldownTimer, data.dashCooldown);
        }
            
        private IEnumerator DashMovement(Vector3 direction, float speed)
        {
            IsLocked = true;
            OnDashBegin?.Invoke();
            
            yield return new WaitForEndOfFrame();
            _player.SetInvulnerable(true);
            
            while (_dashEffectDuration.IsActive)
            {
                //_detector.ForceDetection();
                if (!_detector.IsColliding)
                {
                    var newDir = direction;
                
                    newDir = LocomotionUtility.GetNewDirection(newDir, speed,_body);
                
                    _body.Transform.position += newDir * (speed * Time.deltaTime);
                }
                yield return new WaitForEndOfFrame();
            }
            
            _player.SetInvulnerable(false);
            IsLocked = false;
            OnDashEnd?.Invoke();
            
        }

        public void OnGamePause()
        {
            throw new NotImplementedException();
        }

        public void OnGameResume()
        {
            throw new NotImplementedException();
        }
    }
}