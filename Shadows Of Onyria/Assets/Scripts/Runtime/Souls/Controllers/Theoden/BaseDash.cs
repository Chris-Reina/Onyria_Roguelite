#pragma warning disable CS0067

using System;
using UnityEngine;
using System.Collections;

namespace DoaT
{
    [Serializable, CreateAssetMenu(menuName = "Abilities/Concrete/Controller/Base Dash", fileName = "Controller_BaseDash")]
    public class BaseDash : CharacterController, IClone<BaseDashBehaviour>
    {
        public AnimationClip dashClip;
        
        public BaseDashBehaviour Clone()
        {
            return new BaseDashBehaviour();
        }

        public override T GetController<T>()
        {
            return Clone() as T;
        }
    }
    
    [Serializable]
    public sealed class BaseDashBehaviour : IDashBehaviour
    {
        public event Action<object[]> OnActionCancel;
        public event Action<object[]> OnUpdateRotationRequest;
        public event Action OnDashBegin;
        public event Action OnDashEnd;
    
        private IEntity _body;
        private WallDetector _detector;
        private Func<IEnumerator,Coroutine> _startCoroutine;
        
        private readonly TimerHandler _dashCooldownTimer = new TimerHandler();
        private readonly TimerHandler _dashEffectDuration = new TimerHandler();
    
        public bool IsOnCooldown => _dashCooldownTimer.IsActive;
        public bool IsLocked { get; private set; }

        private bool _interrupt = false;
        public void Unload(params object[] parameters)
        {
            _interrupt = true;
        }

        public IDashBehaviour Initialize(IEntity entity, WallDetector detector, Func<IEnumerator, Coroutine> startCoroutine)
        {
            _startCoroutine = startCoroutine;
            _detector = detector;
            _body = entity;
    
            return this;
        }
    
        public void Dash(Vector3 direction, DashParameters data)
        {
            OnUpdateRotationRequest?.Invoke(new object[0]);
            TimerManager.SetTimer(_dashEffectDuration, () => SetCooldown(data), data.dashDuration);
            _startCoroutine?.Invoke(DashMovement(direction.Clone(), (data.dashDistance / data.dashDuration), data.dashDistance));
        }
    
        private void SetCooldown(DashParameters data)
        {
            TimerManager.SetTimer(_dashCooldownTimer, data.dashCooldown);
        }
        
        private IEnumerator DashMovement(Vector3 direction, float speed, float distance)
        {
            IsLocked = true;
            OnDashBegin?.Invoke();

            //var initTime = Time.unscaledTime;
            //var init = _body.Transform.position;
            var totalDist = 0f;
            
            yield return null;
            while (_dashEffectDuration.IsActive && totalDist < distance)
            {
                if (_interrupt)
                {
                    IsLocked = false;
                    OnDashEnd?.Invoke();
                    yield break;
                }
                
                if (ExecutionSystem.Paused)
                    yield return new WaitUntil(() => ExecutionSystem.Paused == false);
                
                
                if (_detector.IsColliding)
                {
                    yield return null;
                    continue;
                }
                
                var hasDetected = _detector.CastDetection(direction, speed, out var possibleDistance);
                var newDir = LocomotionUtility.GetNewDirection(direction, speed, _body);

                if (!hasDetected)
                {
                    var dist = speed * Time.unscaledDeltaTime;
                    totalDist += dist;
                    _body.Transform.position += newDir * dist;
                }
                else if (possibleDistance > 0.38f)
                {
                    possibleDistance -= 0.38f;
                    totalDist += possibleDistance;
                    _body.Transform.position += newDir * possibleDistance;
                }
                yield return null;
            }
            
            if (ExecutionSystem.Paused)
                yield return new WaitUntil(() => ExecutionSystem.Paused == false);
            
            
            if (!_detector.IsColliding && totalDist < distance)
            {
                var hasDetected = _detector.CastDetection(direction, speed, out var possibleDistance);
                var newDir = LocomotionUtility.GetNewDirection(direction, speed, _body);

                if (!hasDetected)
                {
                    var dist = speed * Time.unscaledDeltaTime;
                    //totalDist += dist;
                    _body.Transform.position += newDir * dist;
                }
                else if (possibleDistance > 0.38f)
                {
                    possibleDistance -= 0.38f;
                    //totalDist += possibleDistance;
                    _body.Transform.position += newDir * possibleDistance;
                }
            }

            // Debug.Log("Target Distance" + distance);
            // Debug.Log("Measured distance: " + Vector3.Distance(_body.Transform.position, init));
            // Debug.Log("Total distance: " + totalDist);
            // Debug.Log("Taken Time: " + (Time.unscaledTime - initTime));
            
            IsLocked = false;
            OnDashEnd?.Invoke();
        }

        public void OnGamePause() { }
        public void OnGameResume() { }
    }
}
#pragma warning restore CS0067