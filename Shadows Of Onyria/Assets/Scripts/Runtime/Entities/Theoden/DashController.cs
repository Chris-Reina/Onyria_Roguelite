using System;
using System.Collections;
using DoaT.Inputs;
using UnityEngine;

namespace DoaT
{
    public sealed class DashController : IController
    {
        private const float DASH_COST = 15f;
        
        public event Action OnDashBegin;
        public event Action OnDashEnd;

        private readonly WallDetector _detector;
        private readonly TheodenController _parent;

        public bool InControl { get; private set; }
        public bool IsLocked => _behaviour.IsLocked;
        public bool IsOnCooldown => _behaviour.IsOnCooldown;
        
        private TheodenData _data;
        private CharacterInput _inputData;
        private IDashBehaviour _behaviour;

        public DashController(TheodenController parent, WallDetector detector)
        {
            _parent = parent;
            _detector = detector;
            ExecutionSystem.AddPausable(this);
        }

        public void Initialize(TheodenData data, CharacterInput input)
        {
            _data = data;
            _inputData = input;
            
            _behaviour = PersistentData.SoulInventory.GetDashController().Initialize(_parent, _detector, StartCoroutine);
            _behaviour.OnDashBegin += OnDashBeginEvent;
            _behaviour.OnDashEnd += OnDashEndEvent;
            _behaviour.OnUpdateRotationRequest += UpdateRotationRequest;

            EventManager.Subscribe(UIEvents.OnSoulWindowApply, UpdateData);
            EventManager.Subscribe(PlayerEvents.OnEnableInputs, EnableInputEvent);
            EventManager.Subscribe(PlayerEvents.OnDisableInputs, DisableInputEvent);
            
            OnDashEnd += Finished;
        }

        private Coroutine StartCoroutine(IEnumerator routine)
        {
            return _parent.StartCoroutine(routine);
        }
        
        public void Interrupt() {} //todo: WIP

        public void ControlGained()
        {
            InControl = true;
            
            if(SceneContext.UseDarkness) _parent.SpendDarkness(DASH_COST);
            Dash();
        }
        public void ControlLost() => InControl = false;
        public void Dispose()
        {
            _behaviour.Unload();
            DisableInput();
            ExecutionSystem.RemovePausable(this);
            EventManager.Unsubscribe(UIEvents.OnSoulWindowApply, UpdateData);
            EventManager.Unsubscribe(PlayerEvents.OnEnableInputs, EnableInputEvent);
            EventManager.Unsubscribe(PlayerEvents.OnDisableInputs, DisableInputEvent);
        }

        private void EnableInputEvent(object[] a ) => EnableInput();
        private void DisableInputEvent(object[] a ) => DisableInput();
        public void EnableInput() => InputSystem.BindKey(InputProfile.Gameplay ,"Dash", KeyEvent.Press, RequestControl);
        public void DisableInput() => InputSystem.UnbindKey(InputProfile.Gameplay ,"Dash", KeyEvent.Press, RequestControl);

        private void Dash()
        {
            var direction = _inputData.LastMovementInput;
            
            if (_inputData.HasInput)
            {
                direction = _inputData.MovementInput;
            }
        
            _parent.SetNewRotationTarget(direction, true, true);
            //_detector.ForceDetection();
            _behaviour.Dash(direction, _data.dash);
        } 
        private void Finished() => _parent.ControllerFinished(this);
        private void UpdateData(params object[] parameters)
        {
            _behaviour.OnDashEnd -= OnDashEndEvent;
            _behaviour.OnDashBegin -= OnDashBeginEvent;
            _behaviour.OnUpdateRotationRequest -= UpdateRotationRequest;
            _behaviour.Unload();
            
            PersistentData.SoulInventory.AssignSoulBySlots();
            
            _behaviour = PersistentData.SoulInventory.GetDashController().Initialize(_parent, _detector, StartCoroutine);
            _behaviour.OnUpdateRotationRequest += UpdateRotationRequest;
            _behaviour.OnDashBegin += OnDashBeginEvent;
            _behaviour.OnDashEnd += OnDashEndEvent;
        }
        private void RequestControl()
        {
            if (InControl || IsOnCooldown) return;
            if (SceneContext.UseDarkness && !_parent.CanSpendDarkness(DASH_COST))
            {
                Messenger.Display("Not Enough Darkness", Messenger.Error);
                return;
            }
            
            _parent.RequestControl(this);
        }

        private void OnDashBeginEvent() => OnDashBegin?.Invoke();
        private void OnDashEndEvent() => OnDashEnd?.Invoke();
        private void UpdateRotationRequest(object[] objects)
        {
            _parent.SetNewRotationTarget(_inputData.LastMovementInput, true);
        }

        public void OnGamePause()
        {
            _behaviour.OnGamePause();
        }

        public void OnGameResume()
        {
            _behaviour.OnGameResume();
        }
    }
}