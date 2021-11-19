using UnityEngine;
using UnityEngine.VFX;

namespace DoaT.AI
{
    public class CrystalSkullView : MonoBehaviour, IUpdate, IUnloadable, IPausable
    {
        private CrystalSkullModel _m;
        private CrystalSkullController _c;

        private static readonly int Stun = Animator.StringToHash("Stun");
        private static readonly int Effect = Animator.StringToHash("AttackEffect");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int IsDead = Animator.StringToHash("IsDead");
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        private static readonly int AttackSpeedMultiplier = Animator.StringToHash("AttackSpeedMultiplier");
        private static readonly int MovementSpeedMultiplier = Animator.StringToHash("MovementSpeedMultiplier");

        private static readonly int DissolveStage = Shader.PropertyToID("_DissolveStage");
        
        [Header("AudioCues")]
        [SerializeField] private AudioCue _damageCue;
        [SerializeField] private AudioCue _deathCue;

        [SerializeField] private Collider _normalCollider;
        [SerializeField] private Collider _ragdollCollider;
        [SerializeField] private Rigidbody _ragdollRigidbody;
        [SerializeField] private VisualEffect _hitEffectPrefab;
        [SerializeField] private Transform _effectSpawnPoint;
        private Material _myMat;
        

        private void Awake()
        {
            _m = GetComponent<CrystalSkullModel>();
            _c = GetComponent<CrystalSkullController>();

            _c.OnDamageTaken += OnTakeDamageEvent;

            _c.OnMoveBegin += OnMoveBeginEvent;
            _c.OnMoveEnd += OnMoveEndEvent;

            _c.OnAttackBegin += OnAttackBeginEvent;
            _c.OnAttackEffect += OnAttackEffectEvent;
            //_c.OnAttackEnd += OnAttackEndEvent;

            //_c.OnStun += () => _m.animator.SetTrigger(Stun);


            _c.OnDeath += OnDeath;
            //_c.OnHeal += OnHealEvent;
        }

        private void OnDeath()
        {
            SetRagdollEffect(true);
        }

        private void OnAttackBeginEvent() => _m.animator.SetTrigger(Attack);
        private void OnAttackEffectEvent(float f)
        {
            _m.animator.SetFloat(AttackSpeedMultiplier, f);
            _m.animator.SetTrigger(Effect);
        }

        private void OnMoveEndEvent() => _m.animator.SetBool(IsMoving, false);
        private void OnMoveBeginEvent() => _m.animator.SetBool(IsMoving, true);

        private void Start()
        {
            //ExecutionSystem.AddUpdate(this, true);
            _myMat = _m.skullsRenderer.material;
        }

        public void Unload(params object[] parameters)
        {
            //ExecutionSystem.RemoveUpdate(this, true);
        }
        
        public void OnUpdate()
        {
        }

        private void OnTakeDamageEvent(Vector3 attackDirection, bool damageOnHealth)
        {
            if (!_m.Shielded && damageOnHealth)
            {
                _m.animator.SetTrigger(Stun);
            }

            AudioSystem.PlayCue(_damageCue);
            var hitEffect = Instantiate(_hitEffectPrefab); //TODO: FIX
            hitEffect.transform.position = _effectSpawnPoint.position;
            //hitEffect.SetVector3("AttackDirection", attackDirection);
            hitEffect.Play();
        }
        private void OnHealEvent(){}

        public void SetDissolveStage(float dissolveTimerProgress)
        {
            _myMat.SetFloat(DissolveStage, dissolveTimerProgress);
        }

        public void SetRagdollEffect(bool shouldRagdoll) //TODO: Might Reimplement
        {
            if (shouldRagdoll)
            {
                AudioSystem.PlayCue(_deathCue);
                _m.animator.enabled = false;
                _normalCollider.enabled = false;
                _ragdollCollider.enabled = true;
                _ragdollRigidbody.isKinematic = false;
                _ragdollRigidbody.AddExplosionForce(200f, _c.Position + _c.Transform.forward, 5f);

                return;
            }

            _ragdollRigidbody.isKinematic = true;
            _ragdollCollider.enabled = false;
        }
        
        public void OnGamePause()
        {
            _m.animator.speed = 0f;
        }

        public void OnGameResume()
        {
            _m.animator.speed = 1f;
        }
    }
}