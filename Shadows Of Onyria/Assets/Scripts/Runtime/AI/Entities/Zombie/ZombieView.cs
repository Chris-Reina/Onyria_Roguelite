using System;
using System.Collections;
using UnityEngine;

namespace DoaT.AI
{
    public class ZombieView : MonoBehaviour, IUpdate, IUnloadable, IPausable
    {
        private ZombieModel _model;
        private ZombieController _controller;

        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int AttackSpeedMultiplier = Animator.StringToHash("AttackSpeedMultiplier");
        private static readonly int IsDead = Animator.StringToHash("IsDead");

        private readonly TimerHandler _emissiveHandler = new TimerHandler();
        private static readonly int AttackEmissionMultiplier = Shader.PropertyToID("_AttackEmissionMultiplier");

        private void Awake()
        {
            _model = GetComponent<ZombieModel>();
            _model.TriggerAttackCallback += SetAttackTrigger;

            _controller = GetComponent<ZombieController>();
            EventManager.Subscribe(GameEvents.OnSceneUnload, Unload);
        }

        private void Start()
        {
            ExecutionSystem.AddUpdate(this);
        }

        public void OnUpdate()
        {
            _model.animator.SetBool(IsMoving, _model.IsMoving);
            //_model.animator.SetBool(IsDead, _model.IsDead);
        }

        private void SetAttackTrigger(float time)
        {
            TimerManager.SetTimer(_emissiveHandler, 1f);
            StartCoroutine(EmissivePanza());
            _model.animator.SetFloat(AttackSpeedMultiplier, time);
            _model.animator.SetTrigger(Attack);
        }

        private IEnumerator EmissivePanza()
        {
            while (_emissiveHandler.IsActive)
            {
                if (ExecutionSystem.Paused)
                    yield return new WaitUntil(() => ExecutionSystem.Paused == false);
                
                if (_emissiveHandler.Progress < 0.5f)
                {
                    _controller._myMat.SetFloat(AttackEmissionMultiplier, _emissiveHandler.Progress / 0.5f);
                }
                else
                {
                    _controller._myMat.SetFloat(AttackEmissionMultiplier, 1 - _emissiveHandler.Progress);
                }

                yield return new WaitForEndOfFrame();
            }
        }

        public void Unload(params object[] parameters)
        {
            ExecutionSystem.RemoveUpdate(this, false);
        }

        private void OnDestroy()
        {
            EventManager.Unsubscribe(GameEvents.OnSceneUnload, Unload);
            ExecutionSystem.RemoveUpdate(this, true);
        }

        public void OnGamePause()
        {
            _model.animator.speed = 0f;
        }

        public void OnGameResume()
        {
            _model.animator.speed = 1f;
        }
    }
}