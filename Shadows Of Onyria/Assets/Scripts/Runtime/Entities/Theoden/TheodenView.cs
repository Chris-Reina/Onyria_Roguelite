using System;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

// ReSharper disable InconsistentNaming

namespace DoaT
{
    public class TheodenView : MonoBehaviour, IUpdate, IUnloadable, IPausable
    {
        #region Animator and Shader Hashes
        private static readonly int Sword_EmissionTint = Shader.PropertyToID("_EmissionTint");
        private static readonly int SwordTrail_EmissionTint = Shader.PropertyToID("TrailColor");
        
        private static readonly int AttackIndex = Animator.StringToHash("AttackIndex");

        private static readonly int Dash = Animator.StringToHash("Dash");
        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int IsDead = Animator.StringToHash("IsDead");
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        private static readonly int IsDashing = Animator.StringToHash("IsDashing");
        private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
        private static readonly int IsChargingAttack = Animator.StringToHash("IsChargingAttack");
        private static readonly int PlaySpecialAnimation = Animator.StringToHash("PlaySpecialAnimation");

        private static readonly int IdleSpeedMultiplier = Animator.StringToHash("IdleSpeedMultiplier");
        private static readonly int DashSpeedMultiplier = Animator.StringToHash("DashSpeedMultiplier");
        private static readonly int DeathSpeedMultiplier = Animator.StringToHash("DeathSpeedMultiplier");
        private static readonly int AttackSpeedMultiplier = Animator.StringToHash("AttackSpeedMultiplier");
        private static readonly int MovementSpeedMultiplier = Animator.StringToHash("MovementSpeedMultiplier");
        private static readonly int RangeAttackSpeedMultiplier = Animator.StringToHash("RangeAttackSpeedMultiplier");
        private static readonly int SpecialAnimationSpeedMultiplier = Animator.StringToHash("SpecialAnimationSpeedMultiplier");
        
        #endregion

        [Header("References")]
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform rangeAttackSpawnPoint;
        [SerializeField] private ParticleSystem dashTrailsPS;
        [SerializeField] private Renderer _swordRenderer;
        [SerializeField] private Renderer _bodyRenderer;
        [SerializeField] private VisualEffect _swordTrail;
        [SerializeField] private VisualEffect _hitEffect;
        [SerializeField] private Transform _hitEffectPosition;

        [Header("Cues")] public AudioCue[] deathSounds;
        public AudioCue rangeAttackReleaseSound;
        public AudioCue dashSound;

        [Header("Parameters")] public float movementSpeedMultiplier = 0.5f;

        [Header("Model Emission")]
        [SerializeField] private TheodenVisualEmission _vEmission;
        
        private float WalkMovementSpeedMultiplier => _m.Data.locomotion.movementSpeed * movementSpeedMultiplier;
        
        private TheodenModel _m;
        private TheodenController _c;
        private bool _hasDied = false;
        private bool _lastMovementState;
        
        private Material _swordMaterial;
        private Material _bodyMaterial;

        #region Initialization
        private void Awake()
        {
            _c = GetComponent<TheodenController>();
            _m = GetComponent<TheodenModel>();
            if (_animator == null) _animator = GetComponentInChildren<Animator>();

            _swordMaterial = _swordRenderer.material;
            _bodyMaterial = _bodyRenderer.material;
            
            _vEmission.Initialize();
        }

        private void Start()
        {
            GameManager.Current.OnLoadingComplete += Initialize;

            _m.health.OnDeath += OnDeath;

            _c.OnMove += UpdateIsWalking;

            _c.OnDashBegin += TriggerDash;
            _c.OnDashEnd += EndDash;

            _c.OnAttackEnd += () => UpdateIsAttacking(false);
            _c.OnAttackBegin += () => UpdateIsAttacking(true);
            _c.OnAttackStepEnd += () => UpdateIsAttacking(false);
            _c.OnAttackStepBegin += (x, y) => UpdateIsAttacking(true);
            _c.OnAttackStepBegin += ComboBeginStepEvent;
            _c.OnAttackCancel += MainAttackCancel;
            
            _c.OnRangeAttackBegin += OnRangeCharge;
            _c.OnRangeAttackRelease += RangeAttackRelease;
            _c.OnRangeCancel += RangeAttackCancel;

            _c.OnDamageReceived += OnDamageTakenEvent;
            //_c.OnPreparingFire;
            //_c.OnReadyToFire;
            
            EventManager.Subscribe(PlayerEvents.OnMainAttackChange, VFX_MainAttackEmissive);
            EventManager.Subscribe(PlayerEvents.OnRangeAttackChange, VFX_RangeAttackEmissive);
            EventManager.Subscribe(GameEvents.OnSceneUnload, Unload);
            ExecutionSystem.AddUpdate(this);
        }



        public void OnGamePause()
        {
            _animator.speed = 0;
            _swordTrail.playRate = 0f;
            _hitEffect.playRate = 0f;
            if (!dashTrailsPS.isStopped) dashTrailsPS.Pause();
        }
        public void OnGameResume()
        {
            _animator.speed = 1f;
            _swordTrail.playRate = 1f;
            _hitEffect.playRate = 1f;
            if (!dashTrailsPS.isStopped) dashTrailsPS.Play();

            if (!_c.IsDashing)
                dashTrailsPS.Stop();
        }

        private void Initialize()
        {
            _animator.SetBool(IsDead, false);
            _animator.SetBool(IsMoving, false);
            _animator.SetBool(IsDashing, false);
            _animator.SetBool(PlaySpecialAnimation, false);
        }
        #endregion

        public void OnUpdate()
        {
            _animator.SetFloat(MovementSpeedMultiplier, WalkMovementSpeedMultiplier);
            _vEmission.Update();
        }
        
        #region Movement
        private void UpdateIsWalking(bool isWalking)
        {
            if (_lastMovementState == isWalking) return;

            _lastMovementState = isWalking;
            _animator.SetBool(IsMoving, isWalking);
            if (isWalking) _animator.SetTrigger(Walk);
        }
        #endregion

        #region Dash
        private void TriggerDash()
        {
            _animator.SetBool(IsDashing, true);
            _animator.SetTrigger(Dash);
            dashTrailsPS.Play();
            AudioSystem.PlayCue(dashSound);

            TimerManager.SetTimer(new TimerHandler(), () => dashTrailsPS.Stop(), _m.Data.dash.dashDuration);
        }
        private void EndDash()
        {
            _animator.SetBool(IsDashing, false);
            dashTrailsPS.Stop();
        }
        #endregion
        
        #region Main Attack
        private void TriggerAttack()
        {
            _animator.SetTrigger(Attack);
            _vEmission.SetSwordEmissionTarget(1);
        }

        private void UpdateIsAttacking(bool isAttacking)
        {
            _vEmission.SetSwordEmissionTarget(isAttacking ? 1 : 0);
            _animator.SetBool(IsAttacking, isAttacking);
        }

        private void ComboBeginStepEvent(int index, float animationSpeed)
        {
            _animator.SetInteger(AttackIndex, index);
            _animator.SetFloat(AttackSpeedMultiplier, animationSpeed);
            TriggerAttack();
        }
        
        private void MainAttackCancel()
        {
            _animator.SetBool(IsAttacking, false);
            _vEmission.SetSwordEmissionTarget(0);
        }
        #endregion
        
        #region Range Attack

        private void OnRangeCharge(float speed)
        {
            _animator.SetFloat(RangeAttackSpeedMultiplier, speed);
            _animator.SetBool(IsChargingAttack, true);
            _vEmission.SetArmEmissionTarget(1);
        }

        private void RangeAttackCancel()
        {
            _animator.SetBool(IsChargingAttack, false);
            _vEmission.SetArmEmissionTarget(0);
        }
        private void RangeAttackRelease()
        {
            AudioSystem.PlayCue(rangeAttackReleaseSound);
            _animator.SetBool(IsChargingAttack, false);
            _vEmission.SetArmEmissionTarget(0);
        }
        #endregion

        #region Other
        private void OnDamageTakenEvent(Vector3 direction)
        {
            var temp = Instantiate(_hitEffect, _hitEffectPosition);//.position, Quaternion.identity);
            temp.SetVector3("AttackDirection", direction);
            temp.Play();
        }
        private void OnDeath()
        {
            if (_hasDied) return;
            _hasDied = true;
            AudioSystem.PlayCue(deathSounds[Random.Range(0, deathSounds.Length)]);
            
            //TODO: Take Somewhere else
            
            PersistentData.ItemInventory.data.Clear();
        }
        #endregion

        #region VFX
        private void VFX_MainAttackEmissive(params object[] parameters)
        {
            var newColor = ((SColor) parameters[0]).ToColor();

            _swordMaterial.SetColor(Sword_EmissionTint, newColor);
            _swordTrail.SetVector4(SwordTrail_EmissionTint, newColor);
        }
        private void VFX_RangeAttackEmissive(params object[] parameters)
        {
            var newColor = ((SColor) parameters[0]).ToColor();

            _bodyMaterial.SetColor(Sword_EmissionTint, newColor);
            //_swordTrail.SetVector4(SwordTrail_EmissionTint, newColor);
        }
        #endregion

        public void Unload(params object[] parameters)
        {
            GameManager.Current.OnLoadingComplete -= Initialize;
            ExecutionSystem.RemoveUpdate(this, true);
        }

        private void OnDestroy()
        {
            EventManager.Unsubscribe(PlayerEvents.OnMainAttackChange, VFX_MainAttackEmissive);
            EventManager.Unsubscribe(PlayerEvents.OnRangeAttackChange, VFX_RangeAttackEmissive);
            EventManager.Unsubscribe(GameEvents.OnSceneUnload, Unload);
        }
    }
    
    [Serializable]
    public class TheodenVisualEmission
    {
        [Range(0f, 1f)] public float swordEmission;
        [Range(0f, 1f)] public float armEmission;
        public float transitionSpeed = 0.1f;

        [SerializeField] private Renderer swordRenderer;
        [SerializeField] private Renderer bodyRenderer;

        private Material _swordMaterial;
        private Material _bodyMaterial;
        private float _swordEmissionTarget;
        private float _armEmissionTarget;

        private static readonly int EmissionGauge = Shader.PropertyToID("_Emission_Gauge");

        public void Initialize()
        {
            _swordMaterial = swordRenderer.material;
            _bodyMaterial = bodyRenderer.material;
        }

        public void Update()
        {
            swordEmission = Mathf.Lerp(swordEmission, _swordEmissionTarget, transitionSpeed);
            armEmission = Mathf.Lerp(armEmission, _armEmissionTarget, transitionSpeed);
            
            _swordMaterial.SetFloat(EmissionGauge, swordEmission);
            _bodyMaterial.SetFloat(EmissionGauge, armEmission);
        }

        public void SetSwordEmissionTarget(float value)
        {
            _swordEmissionTarget = value;
        }
        
        public void SetArmEmissionTarget(float value)
        {
            _armEmissionTarget = value;
        }
    }
}