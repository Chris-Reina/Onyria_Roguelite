using UnityEngine;

namespace DoaT.AI
{
    public class ZombieMovement : State
    {
        private readonly ZombieController _zc;
        private readonly ZombieModel _model;

        private float _distanceRecalculatePoint = 2.5f;

        public ZombieMovement(StateManager stateManager, ZombieController controller) : base(stateManager)
        {
            _zc = controller;
            _model = controller.Model;
            controller.OnPathUpdated += () => _zc.CurrentIndex = 0;
        }

        public override void Awake()
        {
            if (_zc.DebugMe) DebugManager.Log($"Entering {GetType()}");
            _zc.currentState = GetType().ToString();

            _model.IsMoving = true;
            GetPath();
        }

        public override void Execute()
        {
            if (_model.IsDead)
            {
                _stateManager.SetState<ZombieDeath>();
                return;
            }

            if (_zc.Manager.EnemyDetected)
            {
                if (_zc.Path == null|| _zc.CurrentIndex >= _zc.Path.Count)
                {
                    _model.IsMoving = false;
                    GetPath();
                    return;
                }
                _model.IsMoving = true;
                
                if (Vector3.Distance(_zc.Path[_zc.Path.Count - 1], _model.targetData.Position) > _distanceRecalculatePoint)
                {
                    GetPath();
                }

                if (Vector3.Distance(_zc.Position, _model.targetData.Position) < _model.data.viewDistance/2)//  -- Estoy a distancia de atacar del target
                {
                    _stateManager.SetState<ZombieUseAbility>();
                    return;
                }
                
                _model.lastKnownPosition = _model.targetData.Position;

                if(Vector3.Distance(_zc.Path[_zc.CurrentIndex], _zc.Position) < _model.data.nodeDetection)//NODE DETECTION
                    _zc.CurrentIndex++;

                #region MOVEMENT

                var dir = Vector3.zero;
                    
                foreach (var behaviour in _zc._steeringBehaviours)
                {
                    var temp = behaviour.GetDirection(_zc.Neighbours);
                    dir += temp;
                }

                dir = dir.normalized;

                _zc.transform.position += dir * (_model.MovementSpeed * Time.deltaTime);
                _model.RotationPoint = _zc.Position + dir;

                #endregion
            }
            else
            {
                _stateManager.SetState<ZombieIdle>();
                return;
            }
        }

        public override void Sleep()
        {
            if (_zc.DebugMe) DebugManager.Log($"Exiting {GetType()}");
            _zc.currentState = "";
            
            _model.IsMoving = false;
        }
        
        private void GetPath()
        {
            _zc.Model.pathfinder.GetPath(_zc.PathRequest);
        }
    }

}