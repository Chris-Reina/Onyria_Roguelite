using System;
using System.Collections.Generic;
using DoaT;
using UnityEngine;

[Serializable]
public class AttackSequence
{
    public event Action<int, float> OnBeginStep;

    public List<Attack> attackChain = new List<Attack>();
    public bool IsAttacking => _timer.IsActive;
    public bool IsLocked => _timer.IsActive;
    
    private Func<AttackInfo> _attackInfoFunc;
    private AttackInfo AttackInfo => _attackInfoFunc?.Invoke();
    private IEntity _attacker;
    private TimerHandler _timer = new TimerHandler();
    private bool _inputRecorded = false;
    private bool _endSequenceNextFrame = false;
    private int _index = 0;

    public void Initialize(Func<AttackInfo> info, IEntity attacker)
    {
        _attackInfoFunc += info;
        _attacker = attacker;
    }
    
    public void Update()
    {
        if (_endSequenceNextFrame)
        {
            _endSequenceNextFrame = false;
            EndSequence();
            return;
        }
        
        if (!_inputRecorded || _timer.IsActive) return;
        _inputRecorded = false;

        ExecuteAttack();
    }
    
    public void Attack()
    {
        _inputRecorded = true;
    }

    private void EndStep()
    {
        if (!_inputRecorded) return;
        
        _inputRecorded = false;
        _index += 1;
        
        if (_index >= attackChain.Count)
        {
            _endSequenceNextFrame = true;
            return;
        }

        ExecuteAttack();
    }
    
    private void EndSequence()
    {
        _index = 0;
        _inputRecorded = false;
        if (_timer.IsActive) { TimerManager.CancelTimer(_timer); }
    }

    private void ExecuteAttack()
    {
        var attack = attackChain[_index];
        
        TimerManager.SetTimer(_timer, EndSequence, attackChain[0].Duration);

        foreach (var effect in attack.effects)
        {
            TimerManager.SetTimedAction
            (
                _timer, 
                x => x >= attack.EffectDurationByEffect(effect),
                () => effect.Execute(AttackInfo, _attacker, attack)
            );
        }

        TimerManager.SetTimedAction(_timer, x => x >= attack.UnlockComboDuration, EndStep);
        OnBeginStep?.Invoke(_index, attack.AnimationSpeed);
    }
}
