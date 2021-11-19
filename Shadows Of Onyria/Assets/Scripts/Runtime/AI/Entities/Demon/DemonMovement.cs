using UnityEngine;

namespace DoaT.AI
{
    public class DemonMovement : State
    {
        private readonly DemonController _c;
        private readonly DemonModel _m;

        private float _distanceRecalculatePoint = 2.5f;
        private bool _cancelAttack = false;

        public DemonMovement(StateManager stateManager, DemonController controller) : base(stateManager)
        {
            _c = controller;
            _m = controller.Model;
            controller.OnPathUpdated += () => _c.CurrentIndex = 0;
            
            _c.OnDamageTaken += OnDamageTakenImpl;
        }

        public override void Awake()
        {
            //DebugManager.Log($"Entering {GetType()}");
            GetPath();
        }

        public override void Execute()
        {
            _m.IsMoving = false;
            if (_m.IsDead)
            {
                _stateManager.SetState<DemonDeath>();
                return;
            }
            
            if (_cancelAttack)
            {
                _cancelAttack = false;
                _stateManager.SetState<DemonStagger>();
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

                if (Vector3.Distance(_c.Path[_c.Path.Count - 1], _m.targetData.Position) > _distanceRecalculatePoint)
                    GetPath();

                var distance = Vector3.Distance(_c.Position, _m.targetData.Position);

                if (distance < _m.data.attack.detection.radius * 0.8f)
                {
                    if (!_c.IsAttackOnCooldown)
                    {
                        if (Physics.SphereCast(
                            (_c.transform.position + new Vector3(0, 1.45f, 0) + _c.Direction * 0.4f),
                            0.25f,
                            (_m.targetData.Position - (_c.Position)).normalized,
                            out var hit,
                            distance - 0.4f,
                            LayersUtility.PLAYER_DETECTION_MOVEMENT_MASK,
                            QueryTriggerInteraction.Collide))
                        {
                            if (LayersUtility.IsInMask(LayersUtility.PLAYER_DETECTION_MOVEMENT_MASK,
                                hit.collider.gameObject.layer))
                            {
                                _stateManager.SetState<DemonAttack>();
                                return;
                            }
                        }
                    }
                    else if (distance <= _m.data.attack.detection.radius * 0.8f)
                    {
                        _m.RotationPoint = _m.targetData.Position;
                        return;
                    }
                }

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
                _stateManager.SetState<DemonIdle>();
                return;
            }
        }

        public override void Sleep()
        {
            //DebugManager.LogWarning($"Exiting {GetType()}");

            _m.IsMoving = false;
        }

        private void GetPath()
        {
            _c.Model.pathfinder.GetPath(_c.PathRequest);
        }
        
        private void OnDamageTakenImpl(Vector3 direction, bool onHealth)
        {
            if (!_m.Shielded && _stateManager.IsActualState<DemonMovement>() && onHealth)
                _cancelAttack = true;
        }
    }

}