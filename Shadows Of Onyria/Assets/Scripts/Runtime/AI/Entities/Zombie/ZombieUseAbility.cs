using System.Collections;
using System.Collections.Generic;
using DoaT;
using DoaT.AI;
using UnityEngine;

public class ZombieUseAbility : State
{
#pragma warning disable
    private readonly ZombieController _zc;
    private readonly ZombieModel _model;

    private float _waitTimer;
    private float _waitTimerMin = 0.05f;
    private float _waitTimerMax = 0.3f;
    private readonly TimerHandler _handler = new TimerHandler();
        
    private float _timer, _timerMax, _animationDelay;
    private bool _effectTriggered, _attackTriggered;
#pragma warning restore

    public ZombieUseAbility(StateManager stateManager, ZombieController controller) : base(stateManager)
    {
        _zc = controller;
        _model = controller.Model;
    }

    public override void Awake()
    {
        if (_zc.DebugMe) DebugManager.Log($"Entering {GetType()}");
        _zc.currentState = GetType().ToString();

        _attackTriggered = false;
        _effectTriggered = false;
        
        _waitTimer = Random.Range(_waitTimerMin, _waitTimerMax);
        _timer = 0;
        // _timerMax = _model.data.attack.AnimationDuration;
        // _animationDelay = _timerMax * _model.data.attack.attackEffectDelay;

        _model.RotationPoint = _model.targetData.Position;
        TimerManager.SetTimer(
            _handler, 
            () => _stateManager.SetState<ZombieIdle>(),
            _model.data.attack.Duration,
            _waitTimer);

        foreach (var effect in _model.data.attack.effects)
        {
            TimerManager.SetTimedAction
            (
                _handler, 
                x => x >= _model.data.attack.EffectDurationByEffect(effect),
                () => effect.Execute(_zc.GetAttackInfo(), _zc, _model.data.attack)
            );
        }

        _model.TriggerAttackCallback?.Invoke(_model.data.attack.AnimationSpeed);
    }

    public override void Execute()
    {
        if (_model.IsDead)
        {
            _stateManager.SetState<ZombieDeath>();
            return;
        }
    }

    public override void Sleep()
    {
        if (_zc.DebugMe) DebugManager.Log($"Exiting {GetType()}");
        _zc.currentState = "";

        TimerManager.CancelTimer(_handler);
    }

    private void TriggerAttack()
    {
        _attackTriggered = true;
        //_model.TriggerAttackCallback?.Invoke(_model.data.attack.animationSpeedMultiplier);
        if(_zc.DebugMe) DebugManager.Log("Ability Effect Triggered");
    }
}
