using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationOverrider : MonoBehaviour
{
    public Animator animator;
    private AnimatorOverrideController _animatorOverrideController;
    private AnimatorOverrideController _animatorOverrideControllerBase;

    public AnimationClip baseAttack;
    public AnimationClip baseIdle;
    public AnimationClip baseRun;
    public AnimationClip baseDeath;
    public AnimationClip baseStandUp;
    
    private void Awake()
    {
        _animatorOverrideControllerBase = new AnimatorOverrideController(animator.runtimeAnimatorController);
        ResetController();
    }
    
    public void ResetController()
    {
        _animatorOverrideController = new AnimatorOverrideController(_animatorOverrideControllerBase);
        animator.runtimeAnimatorController = _animatorOverrideController;
    }

    public void ChangeAttackAnimation(AnimationClip clip)
    {
        _animatorOverrideController[baseAttack.name] = clip;
    }
    
    public void ChangeIdleAnimation(AnimationClip clip)
    {
        _animatorOverrideController[baseIdle.name] = clip;
    }
    
    public void ChangeRunAnimation(AnimationClip clip)
    {
        _animatorOverrideController[baseRun.name] = clip;
    }

    public void ChangeDeathAnimation(AnimationClip clip)
    {
        _animatorOverrideController[baseDeath.name] = clip;
    }
    
    public void ChangeStandUpAnimation(AnimationClip clip)
    {
        _animatorOverrideController[baseStandUp.name] = clip;
    }
}
