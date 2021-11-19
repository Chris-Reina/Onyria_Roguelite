using System;
using System.Collections;
using DoaT.Inputs;
using UnityEngine;

namespace DoaT
{
    public class MainAttackController : IController
    {
        public event Action OnAttackBegin;
        public event Action OnAttackEnd;
        public event Action OnAttackCancel;
        
        public event Action<int, float> OnAttackStepBegin;
        public event Action OnAttackStepEnd;
    
        public bool InControl { get; private set; }
        public bool IsLocked => _behaviour.IsLocked;
        public bool IsOnCooldown => _behaviour.IsOnCooldown;

        private readonly TheodenController _parent;
        
        private TheodenData _data;
        private CharacterInput _inputData;
        private IAttackBehaviour _behaviour;
        private AttackInputType _inputTypeMask = AttackInputType.None;
        private AttackInputType _currentInputType = AttackInputType.Press;
        private Coroutine _currentCoroutine;
        private bool _inputEnabled;
        
        public MainAttackController(TheodenController parent)
        {
            _parent = parent;
            ExecutionSystem.AddPausable(this);
        }
        
        public void Initialize(TheodenData data, CharacterInput input)
        {
            _data = data;
            _inputData = input;

            _behaviour = PersistentData.SoulInventory.GetMainAttackController().Initialize(GetInfo, _parent);
            _inputTypeMask = _behaviour.AttackInputMask;
            _behaviour.OnAttackEnd += OnAttackEndEvent;
            _behaviour.OnAttackBegin += OnAttackBeginEvent;
            _behaviour.OnActionCancel += OnAttackCancelEvent;
            _behaviour.OnAttackStepEnd += OnAttackStepEndEvent;
            _behaviour.OnAttackStepBegin += OnAttackStepBeginEvent;
            _behaviour.OnUpdateRotationRequest += UpdateRotationRequest;
            
            EventManager.Raise(PlayerEvents.OnMainAttackChange, new SColor(_behaviour.SoulType.soulEnergyColor));

            EventManager.Subscribe(UIEvents.OnSoulWindowApply, UpdateData);
            EventManager.Subscribe(PlayerEvents.OnEnableInputs, EnableInputEvent);
            EventManager.Subscribe(PlayerEvents.OnDisableInputs, DisableInputEvent);
        }
        
        private void UpdateData(params object[] parameters)
        {
            _behaviour.OnAttackEnd -= OnAttackEndEvent;
            _behaviour.OnAttackBegin -= OnAttackBeginEvent;
            _behaviour.OnActionCancel -= OnAttackCancelEvent;
            _behaviour.OnAttackStepEnd -= OnAttackStepEndEvent;
            _behaviour.OnAttackStepBegin -= OnAttackStepBeginEvent;
            _behaviour.OnUpdateRotationRequest -= UpdateRotationRequest;
            _behaviour.Unload();
            DisableInput();
            
            PersistentData.SoulInventory.AssignSoulBySlots();
            
            _behaviour = PersistentData.SoulInventory.GetMainAttackController().Initialize(GetInfo, _parent);
            _behaviour.OnAttackEnd += OnAttackEndEvent;
            _behaviour.OnAttackBegin += OnAttackBeginEvent;            
            _behaviour.OnActionCancel += OnAttackCancelEvent;
            _behaviour.OnAttackStepEnd += OnAttackStepEndEvent;
            _behaviour.OnAttackStepBegin += OnAttackStepBeginEvent;
            _behaviour.OnUpdateRotationRequest += UpdateRotationRequest;
            _inputTypeMask = _behaviour.AttackInputMask;
            EnableInput();

            EventManager.Raise(PlayerEvents.OnMainAttackChange, new SColor(_behaviour.SoulType.soulEnergyColor));
        }

        private IEnumerator UpdateAttackLoop()
        {
            while (_behaviour.NeedsUpdate)
            {
                _behaviour.Update();
                yield return null;
            }
        }

        public void EnableInput()
        {
            if (_inputEnabled) return;
            _inputEnabled = true;
            if(_inputTypeMask.HasFlag(AttackInputType.Press)) InputSystem.BindKey(InputProfile.Gameplay, "Attack", KeyEvent.Press, OnPress);
            if(_inputTypeMask.HasFlag(AttackInputType.Sustain)) InputSystem.BindKey(InputProfile.Gameplay, "Attack", KeyEvent.Hold, OnSustain);
            if(_inputTypeMask.HasFlag(AttackInputType.Release)) InputSystem.BindKey(InputProfile.Gameplay, "Attack", KeyEvent.Release, OnRelease);
        }
        public void DisableInput()
        {
            if (!_inputEnabled) return;
            _inputEnabled = false;
            /*if(_inputTypeMask.HasFlag(AttackInputType.Press))*/ InputSystem.UnbindKey(InputProfile.Gameplay, "Attack", KeyEvent.Press, OnPress);
            /*if(_inputTypeMask.HasFlag(AttackInputType.Sustain))*/ InputSystem.UnbindKey(InputProfile.Gameplay, "Attack", KeyEvent.Hold, OnSustain);
            /*if(_inputTypeMask.HasFlag(AttackInputType.Release))*/ InputSystem.UnbindKey(InputProfile.Gameplay, "Attack", KeyEvent.Release, OnRelease);
        }

        private void OnPress()
        {
            if (IsOnCooldown || !GameState.CanAttackMelee) return;
            if (InControl)
            {
                _behaviour.SendImpulsePress();
                return;
            }

            _currentInputType = AttackInputType.Press;
            _parent.RequestControl(this);
        }
        private void OnSustain()
        {
            if (IsOnCooldown || !GameState.CanAttackMelee) return;
            if (InControl)
            {
                _behaviour.SendImpulseSustained();
                return;
            }
            
            _currentInputType = AttackInputType.Sustain;
            _parent.RequestControl(this);
        }
        private void OnRelease()
        {
            if (IsOnCooldown || !GameState.CanAttackMelee) return;
            if (InControl)
            {
                _behaviour.SendImpulseRelease();
                return;
            }
            
            _currentInputType = AttackInputType.Release;
            _parent.RequestControl(this);
        }

        public void ControlGained()
        {
            InControl = true;

            switch (_currentInputType)
            {
                case AttackInputType.None:
                    InControl = false;
                    return;
                case AttackInputType.Press:
                    _behaviour.SendImpulsePress();
                    break;
                case AttackInputType.Sustain:
                    _behaviour.SendImpulseSustained();
                    break;
                case AttackInputType.Release:
                    _behaviour.SendImpulseRelease();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _currentInputType = AttackInputType.None;
            _currentCoroutine = _parent.StartCoroutine(UpdateAttackLoop());
        }
        public void ControlLost()
        {
            InControl = false;
            _behaviour.Interrupt();
            _parent.StopCoroutine(_currentCoroutine);
        }

        #region EventWrappers
        private void EnableInputEvent(object[] a ) => EnableInput();
        private void DisableInputEvent(object[] a ) => DisableInput();
        private void OnAttackCancelEvent(object[] objects) => OnAttackCancel?.Invoke();
        private void OnAttackBeginEvent(object[] objects) => OnAttackBegin?.Invoke();
        private void OnAttackEndEvent(object[] objects)
        {
            OnAttackEnd?.Invoke();
            _parent.ControllerFinished(this);
        }
        private void OnAttackStepEndEvent(object[] objects) => OnAttackStepEnd?.Invoke();

        private void OnAttackStepBeginEvent(object[] objects) => OnAttackStepBegin?.Invoke((int) objects[0], (float) objects[1]);
        private void UpdateRotationRequest(object[] objects)
        {
            _parent.SetNewRotationTarget(_inputData.AimInput, true);
        }
        #endregion
        
        private AttackInfo GetInfo()
        {
            return new AttackInfo(_data.baseDamage, _data.criticalChance, _data.criticalMultiplier, _parent);
        }
        public void Interrupt()
        {
            _behaviour.Interrupt();
        }
        public void OnGamePause()
        {
            _behaviour.OnGamePause();
        }
        public void OnGameResume()
        {
            _behaviour.OnGameResume();
            if (_parent.IsCurrentController(this))
            {
                _behaviour.Interrupt();
                OnAttackEnd?.Invoke();
                _parent.ControllerFinished(this);
            }
        }
        public void Dispose()
        {
            DisableInput();
            _behaviour.Unload();
            ExecutionSystem.RemovePausable(this);
            EventManager.Unsubscribe(UIEvents.OnSoulWindowApply, UpdateData);
            EventManager.Unsubscribe(PlayerEvents.OnEnableInputs, EnableInputEvent);
            EventManager.Unsubscribe(PlayerEvents.OnDisableInputs, DisableInputEvent);
        }
    }
}