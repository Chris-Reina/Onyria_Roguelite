using UnityEngine;

namespace DoaT.AI
{
    public class BansheeMovement : State
    {
        private readonly BansheeController _c;
        private readonly BansheeModel _m;
        
        private float _distanceRecalculatePoint = 2.5f;
        private bool _cancelAttack = false;
        private float forcedIdleTime = 4f;
        
        public BansheeMovement(StateManager stateManager, BansheeController controller) : base(stateManager)
        {
            _c = controller;
            _m = controller.Model;
            controller.OnPathUpdated += () => _c.CurrentIndex = 0;
            
            _c.OnDamageTaken += OnDamageTakenImpl;
        }

        public override void Awake()
        {
            var tarPos =  _m.targetData.Position;
            var distanceToPlayer = (tarPos - _c.Position).magnitude;

            if (distanceToPlayer <= 4)
            {
                var dir = (_c.Position - tarPos).normalized;
                _c.UpdatedTargetPosition = _c.Position + (dir * 10f);
            }
            else if (distanceToPlayer > 12)
            {
                _c.UpdatedTargetPosition = tarPos;
            }
            else
            {
                var dir = (_c.Position - tarPos).normalized;
                dir = Quaternion.Euler(0, Random.Range(20, 340), 0) * dir;
                _c.UpdatedTargetPosition = _c.Position + (dir * 5f);
            }

            GetPath();
        }

        public float walkingTime;
        
        public override void Execute()
        {
            _m.IsMoving = false;
            if (_m.IsDead)
            {
                _stateManager.SetState<BansheeDeath>();
                return;
            }
            
            walkingTime += Time.deltaTime;
            if (walkingTime > 3)
            {
                _stateManager.SetState<BansheeIdle>();
                return;
            }

            
            if (_cancelAttack)
            {
                _cancelAttack = false;
                _stateManager.SetState<BansheeStagger>();
                return;
            }

            if (_c.Manager.EnemyDetected)
            {
                if (_c.Path == null) return;
                if (_c.CurrentIndex >= _c.Path.Count)
                {
                    GetPath();
                    return;
                }

                // if (Vector3.Distance(_c.Path[_c.Path.Count - 1], _m.targetData.Position) > _distanceRecalculatePoint)
                //     GetPath();

                //var distance = Vector3.Distance(_c.Position, _m.targetData.Position);

                if (!_c.IsSummonOnCooldown)
                {
                    _stateManager.SetState<BansheeSummon>();
                    return;
                }

                //TODO: IMPLEMENT CHANGES TO SUMMON AND CHANNEL
                // if (distance < _m.data.attack.detection.radius * 0.8f)  
                // {
                //     if (!_c.IsAttackOnCooldown)
                //     {
                //         if (Physics.SphereCast(
                //             (_c.transform.position + new Vector3(0, 1.45f, 0) + _c.Direction * 0.4f),
                //             0.25f,
                //             (_m.targetData.Position - (_c.Position)).normalized,
                //             out var hit,
                //             distance - 0.4f,
                //             LayersUtility.PLAYER_DETECTION_MOVEMENT_MASK,
                //             QueryTriggerInteraction.Collide))
                //         {
                //             if (LayersUtility.IsInMask(LayersUtility.PLAYER_DETECTION_MOVEMENT_MASK,
                //                 hit.collider.gameObject.layer))
                //             {
                //                 _stateManager.SetState<DemonAttack>();
                //                 return;
                //             }
                //         }
                //     }
                //     else if (distance <= _m.data.attack.detection.radius * 0.8f)
                //     {
                //         _m.RotationPoint = _m.targetData.Position;
                //         return;
                //     }
                // }

                if (Vector3.Distance(_c.Path[_c.CurrentIndex], _c.Position) < _m.data.nodeDetection)
                    _c.CurrentIndex++;

                #region MOVEMENT

                _m.IsMoving = true;
                var dir = Vector3.zero;
                foreach (var behaviour in _c.SteeringBehaviours)
                {
                    var temp = behaviour.GetDirection(_c.Neighbours);
                    dir += temp;
                }

                dir = dir.normalized;

                _c.transform.position += dir * (_m.MovementSpeed * Time.deltaTime);
                _m.RotationPoint = _c.Position + dir;

                #endregion
            }
            else
            {
                _stateManager.SetState<BansheeIdle>();
                return;
            }
        }

        public override void Sleep()
        {
            //DebugManager.LogWarning($"Exiting {GetType()}");
            _c.ForceIdle(forcedIdleTime);
            walkingTime = 0;
            _m.IsMoving = false;
        }

        private void GetPath()
        {
            _c.Model.pathfinder.GetPath(_c.PathRequest);
        }
        
        private void OnDamageTakenImpl(Vector3 direction, bool onHealth)
        {
            if (!_m.Shielded && _stateManager.IsActualState<BansheeMovement>() && onHealth)
                _cancelAttack = true;
        }
    }
}