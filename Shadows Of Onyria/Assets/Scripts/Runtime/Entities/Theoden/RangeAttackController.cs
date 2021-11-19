using System;
using System.Collections;
using DoaT.Inputs;
using UnityEngine;

namespace DoaT
{
    public sealed class RangeAttackController : IController
    {
        private const float ATTACK_COST = 25f;
        
        public event Action<float> OnAttackBegin;
        public event Action<float> OnPreparingFire;
        public event Action OnReadyToFire;
        public event Action OnAttackEnd;
        public event Action OnAttackCancel;

        public bool InControl { get; private set; }
        public bool IsLocked => _behaviour.IsLocked;
        public bool IsOnCooldown => _behaviour.IsOnCooldown;

        private readonly TheodenController _parent;
        private readonly Action<Coroutine> _stopCoroutine;
        private readonly Func<IEnumerator, Coroutine> _startCoroutine;
        
        private TheodenData _data;
        private CharacterInput _inputData;
        private IAttackBehaviour _behaviour;
        private AttackInputType _inputTypeMask = AttackInputType.None;
        private AttackInputType _inputRequestMask = AttackInputType.None;
        private AttackInputType _currentInputType = AttackInputType.Press;
        private Coroutine _currentCoroutine;

        public RangeAttackController(TheodenController parent, Func<IEnumerator, Coroutine> startCoroutine, Action<Coroutine> stopCoroutine)
        {
            _parent = parent;
            _stopCoroutine = stopCoroutine;
            _startCoroutine = startCoroutine;
            
            ExecutionSystem.AddPausable(this);
        }
        
        public void Initialize(TheodenData data, CharacterInput input)
        {
            _data = data;
            _inputData = input;

            _behaviour = PersistentData.SoulInventory.GetRangeAttackController().Initialize(GetInfo, _parent);
            _inputTypeMask = _behaviour.AttackInputMask;
            _inputRequestMask = _behaviour.AttackRequestInputMask;
            _behaviour.OnAttackEnd += OnAttackEndEvent;
            _behaviour.OnAttackBegin += OnAttackBeginEvent;
            _behaviour.OnActionCancel += OnAttackCancelEvent;
            _behaviour.OnAttackStepEnd += OnReadyToFireEvent;
            _behaviour.OnAttackUpdate += OnPreparingFireEvent;
            _behaviour.OnUpdateRotationRequest += OnUpdateRotationRequest;

            _behaviour.OnSpendDarkness += SpendDarkness;
            
            EventManager.Raise(PlayerEvents.OnRangeAttackChange, new SColor(_behaviour.SoulType.soulEnergyColor));

            EventManager.Subscribe(UIEvents.OnSoulWindowApply, UpdateData);
            EventManager.Subscribe(PlayerEvents.OnEnableInputs, EnableInputEvent);
            EventManager.Subscribe(PlayerEvents.OnDisableInputs, DisableInputEvent);
        }

        private void SpendDarkness() //TODO: Define Correct Implementation
        {
            if(SceneContext.UseDarkness) _parent.SpendDarkness(ATTACK_COST);
        }

        private void UpdateData(params object[] parameters)
        {
            _behaviour.OnAttackEnd -= OnAttackEndEvent;
            _behaviour.OnAttackBegin -= OnAttackBeginEvent;
            _behaviour.OnActionCancel -= OnAttackCancelEvent;
            _behaviour.OnAttackStepEnd -= OnReadyToFireEvent;
            _behaviour.OnAttackUpdate -= OnPreparingFireEvent;
            _behaviour.OnUpdateRotationRequest -= OnUpdateRotationRequest;
            
            _behaviour.OnSpendDarkness -= SpendDarkness;
            
            _behaviour.Unload();
            DisableInput();
            
            PersistentData.SoulInventory.AssignSoulBySlots();
            
            _behaviour = PersistentData.SoulInventory.GetRangeAttackController().Initialize(GetInfo, _parent);
            _behaviour.OnAttackEnd += OnAttackEndEvent;
            _behaviour.OnAttackBegin += OnAttackBeginEvent;
            _behaviour.OnActionCancel += OnAttackCancelEvent;
            _behaviour.OnAttackStepEnd += OnReadyToFireEvent;
            _behaviour.OnAttackUpdate += OnPreparingFireEvent;
            _behaviour.OnUpdateRotationRequest += OnUpdateRotationRequest;
            
            _behaviour.OnSpendDarkness += SpendDarkness;
            
            _inputTypeMask = _behaviour.AttackInputMask;
            _inputRequestMask = _behaviour.AttackRequestInputMask;
            EnableInput();

            EventManager.Raise(PlayerEvents.OnRangeAttackChange, new SColor(_behaviour.SoulType.soulEnergyColor));
        }

        private void EnableInputEvent(object[] a ) => EnableInput();
        private void DisableInputEvent(object[] a ) => DisableInput();
        public void EnableInput()
        {
            if(_inputRequestMask.HasFlag(AttackInputType.Press)) InputSystem.BindKey(InputProfile.Gameplay, "RangeAttack", KeyEvent.Press, RequestOnPress);
            if(_inputRequestMask.HasFlag(AttackInputType.Sustain)) InputSystem.BindKey(InputProfile.Gameplay, "RangeAttack", KeyEvent.Hold, RequestOnSustain);
            if(_inputRequestMask.HasFlag(AttackInputType.Release)) InputSystem.BindKey(InputProfile.Gameplay, "RangeAttack", KeyEvent.Release, RequestOnRelease);
            
            if(_inputTypeMask.HasFlag(AttackInputType.Press)) InputSystem.BindKey(InputProfile.Gameplay, "RangeAttack", KeyEvent.Press, OnPress);
            if(_inputTypeMask.HasFlag(AttackInputType.Sustain)) InputSystem.BindKey(InputProfile.Gameplay, "RangeAttack", KeyEvent.Hold, OnSustain);
            if(_inputTypeMask.HasFlag(AttackInputType.Release)) InputSystem.BindKey(InputProfile.Gameplay, "RangeAttack", KeyEvent.Release, OnRelease);
        }
        public void DisableInput()
        {
            if(_inputRequestMask.HasFlag(AttackInputType.Press)) InputSystem.UnbindKey(InputProfile.Gameplay, "RangeAttack", KeyEvent.Press, RequestOnPress);
            if(_inputRequestMask.HasFlag(AttackInputType.Sustain)) InputSystem.UnbindKey(InputProfile.Gameplay, "RangeAttack", KeyEvent.Hold, RequestOnSustain);
            if(_inputRequestMask.HasFlag(AttackInputType.Release)) InputSystem.UnbindKey(InputProfile.Gameplay, "RangeAttack", KeyEvent.Release, RequestOnRelease);
            
            if(_inputTypeMask.HasFlag(AttackInputType.Press)) InputSystem.UnbindKey(InputProfile.Gameplay, "RangeAttack", KeyEvent.Press, OnPress);
            if(_inputTypeMask.HasFlag(AttackInputType.Sustain)) InputSystem.UnbindKey(InputProfile.Gameplay, "RangeAttack", KeyEvent.Hold, OnSustain);
            if(_inputTypeMask.HasFlag(AttackInputType.Release)) InputSystem.UnbindKey(InputProfile.Gameplay, "RangeAttack", KeyEvent.Release, OnRelease);
        }

        private void RequestOnPress()
        {
            if(SceneContext.UseDarkness && !_parent.CanSpendDarkness(ATTACK_COST)) 
            {
                Messenger.Display("Not Enough Darkness", Messenger.Error);
                return;
            }
            
            _currentInputType = AttackInputType.Press;
            _parent.RequestControl(this);
        }
        private void RequestOnSustain()
        {
            if(SceneContext.UseDarkness && !_parent.CanSpendDarkness(ATTACK_COST)) 
            {
                Messenger.Display("Not Enough Darkness", Messenger.Error);            
                return;
            }
        
            _currentInputType = AttackInputType.Sustain;
            _parent.RequestControl(this);
            
        }
        private void RequestOnRelease()
        {
            if(SceneContext.UseDarkness && !_parent.CanSpendDarkness(ATTACK_COST)) 
            {
                Messenger.Display("Not Enough Darkness", Messenger.Error);            
                return;
            }
        
            _currentInputType = AttackInputType.Release;
            _parent.RequestControl(this);
        }
        
        private void OnPress()
        {
            if (IsOnCooldown) return;
            if (InControl)
            {
                _behaviour.SendImpulsePress();
            }
        }
        private void OnSustain()
        {
            if (IsOnCooldown) return;
            if (!InControl) return;
            
            _behaviour.SendImpulseSustained();
        }
        private void OnRelease()
        {
            if (IsOnCooldown) return;
            if (InControl)
            {
                _behaviour.SendImpulseRelease();
                return;
            }
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
            if(_behaviour.NeedsUpdate) _currentCoroutine = _startCoroutine(UpdateAttackLoop());
        }

        public void ControlLost()
        {
            InControl = false;
            _behaviour.Interrupt();
            if (_currentCoroutine != null)
            {
                _stopCoroutine(_currentCoroutine);
                _currentCoroutine = null;
            }
        }

        public void Dispose()
        {
            DisableInput();
            ExecutionSystem.RemovePausable(this);
            EventManager.Unsubscribe(UIEvents.OnSoulWindowApply, UpdateData);
            EventManager.Unsubscribe(PlayerEvents.OnEnableInputs, EnableInputEvent);
            EventManager.Unsubscribe(PlayerEvents.OnDisableInputs, DisableInputEvent);
        }

        private IEnumerator UpdateAttackLoop()
        {
            while (_behaviour.NeedsUpdate)
            {
                _behaviour.Update();
                yield return null;
            }
        }
        
        public void Interrupt()
        {
            //if(_currentCoroutine != null) _stopCoroutine(_currentCoroutine);
            _behaviour.Interrupt();
        }

        private AttackInfo GetInfo() => new AttackInfo(_data.baseDamage, _data.criticalChance, _data.criticalMultiplier, _parent);
        
        #region EventWrappers
        private void OnAttackCancelEvent(object[] objects) => OnAttackCancel?.Invoke();
        private void OnReadyToFireEvent(object[] objects) => OnReadyToFire?.Invoke();
        private void OnPreparingFireEvent(object[] objects) => OnPreparingFire?.Invoke((float)objects[0]);
        private void OnAttackBeginEvent(object[] objects) => OnAttackBegin?.Invoke((float)objects[0]);
        private void OnAttackEndEvent(object[] objects)
        {
            OnAttackEnd?.Invoke();
            _parent.ControllerFinished(this);
        }
        private void OnUpdateRotationRequest(object[] objects)
        {
            _parent.SetNewRotationTarget(_inputData.AimInput, true);
        }
        #endregion

        public void OnGamePause() => _behaviour.OnGamePause();
        public void OnGameResume()
        {
            if (_parent.IsCurrentController(this))
            {
                _behaviour.SendImpulseRelease();
            }
            _behaviour.OnGameResume();
        }
    }
}