using System;
using UnityEngine;

namespace DoaT
{
    public sealed class MovementController : IController, IUpdate
    {
        public event Action<bool> OnMove;
        
        private WallDetector _detector;
        private TheodenController _parent;
        
        private RotationData _currentRotation;
        private TheodenData _data;
        private CharacterInput _inputData;
        private ILocomotionBehaviour _behaviour;

        public bool IsLocked => false;
        public bool IsOnCooldown => _behaviour.IsOnCooldown;
        public bool InControl { get; private set; }
        
        public RotationData CurrentRotation
        {
            set => _currentRotation = value;
            get => _currentRotation;
        }
        
        private bool ShouldUpdate => true;

        public MovementController(TheodenController parent, WallDetector detector)
        {
            _parent = parent;
            _detector = detector;
        }

        public void Initialize(TheodenData data, CharacterInput input)
        {
            _data = data;
            _inputData = input;
            
            _behaviour = PersistentData.SoulInventory.GetLocomotionController().Initialize(_parent, _detector);
            _behaviour.OnMove += OnMoveEvent;

            EventManager.Subscribe(UIEvents.OnSoulWindowApply, UpdateData);
            EventManager.Subscribe(PlayerEvents.OnEnableInputs, EnableInputEvent);
            EventManager.Subscribe(PlayerEvents.OnDisableInputs, DisableInputEvent);

            _inputData.UpdateLastMovementDirection(new Vector3(0, 0, 0));
        }


        public void ControlGained()
        {
            InControl = true;
        }

        public void ControlLost()
        {
            InControl = false;
            OnMove?.Invoke(false);
        }

        public void Interrupt() { }
        public void OnGamePause() => _behaviour.OnGamePause();
        public void OnGameResume() => _behaviour.OnGameResume();

        private void EnableInputEvent(object[] a ) => EnableInput();
        private void DisableInputEvent(object[] a ) => DisableInput();
        public void DisableInput()
        {
            ExecutionSystem.RemoveUpdate(this, false);
        }

        public void EnableInput()
        {
            ExecutionSystem.AddUpdate(this);
        }

        public void OnUpdate()
        {
            if(InControl)
                UpdateController();

            RotateToDirection();
        }
        
        private void UpdateController()
        {
            var newDir = _inputData.MovementInput;
            var isZero = !newDir.Any(x => x != 0);
            
            if (isZero)
            {
                OnMove?.Invoke(false);
                return;
            }

            newDir = LocomotionUtility.GetNewDirection(newDir,_data.locomotion.movementSpeed, _parent);

            _currentRotation = new RotationData(newDir, false);
            _behaviour.Move(newDir, _data.locomotion, true);
            _inputData.UpdateLastMovementDirection();
        }

        private void RotateToDirection()
        {
            _behaviour.Rotate(_currentRotation, _data.locomotion);
        }

        private void UpdateData(params object[] parameters)
        {
            _behaviour.OnMove -= OnMoveEvent;
            _behaviour.Unload();
            PersistentData.SoulInventory.AssignSoulBySlots();
            _behaviour = PersistentData.SoulInventory.GetLocomotionController().Initialize(_parent, _detector);
            _behaviour.OnMove += OnMoveEvent;
        }

        private void OnMoveEvent(bool isMoving) => OnMove?.Invoke(isMoving);

        public void RotateForced()
        {
            RotateToDirection();
        }

        public void Dispose()
        {
            _parent = null;
            _detector = null;
            _currentRotation = default;
            _data = null;
            _inputData = null;
            
            _behaviour.OnMove -= OnMoveEvent;
            _behaviour.Unload();
            _behaviour = null;
            EventManager.Unsubscribe(UIEvents.OnSoulWindowApply, UpdateData);
            EventManager.Unsubscribe(PlayerEvents.OnEnableInputs, EnableInputEvent);
            EventManager.Unsubscribe(PlayerEvents.OnDisableInputs, DisableInputEvent);
            //ExecutionSystem.RemoveUpdate(this, true);
        }
    }
}