using UnityEngine;
using UnityEngine.VFX;

namespace DoaT.AI
{
    public class DemonView : MonoBehaviour, IUpdate, IPausable
    {
        private DemonModel _m;
        private DemonController _c;

        private static readonly int Stun = Animator.StringToHash("Stun");
        private static readonly int Effect = Animator.StringToHash("AttackEffect");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int IsDead = Animator.StringToHash("IsDead");
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        private static readonly int AttackSpeedMultiplier = Animator.StringToHash("AttackSpeedMultiplier");
        private static readonly int MovementSpeedMultiplier = Animator.StringToHash("MovementSpeedMultiplier");

        private static readonly int DissolveStage = Shader.PropertyToID("_DissolveStage");

        [Header("AudioCues")] [SerializeField] private AudioCue _damageCue;
        [SerializeField] private AudioCue _deathCue;

        [SerializeField] private Collider _normalCollider;
        [SerializeField] private float _ragdollAppliedForce = 200f;
        [SerializeField] private Collider[] _ragdollColliders;
        [SerializeField] private Rigidbody[] _ragdollRigidbodies;
        [SerializeField] private VisualEffect _hitEffectPrefab;
        [SerializeField] private Transform _effectSpawnPoint;
        private Material _myMat;


        private void Awake()
        {
            _m = GetComponent<DemonModel>();
            _c = GetComponent<DemonController>();

            _c.OnDamageTaken += OnTakeDamageEvent;

            _c.OnMoveBegin += OnMoveBeginEvent;
            _c.OnMoveEnd += OnMoveEndEvent;

            _c.OnAttackBegin += AttackBeginEvent;
            //_c.OnAttackEnd += OnAttackEndEvent;

            //_c.OnStun += () => _m.animator.SetTrigger(Stun);


            _c.OnDeath += OnDeath;
            //_c.OnHeal += OnHealEvent;
        }

        private void OnDeath()
        {
            //AudioSystem.PlayCue(_deathCue);
        }


        private void AttackBeginEvent(float f)
        {
            _m.animator.SetFloat(AttackSpeedMultiplier, f);
            _m.animator.SetTrigger(Attack);
        }

        private void OnMoveEndEvent() => _m.animator.SetBool(IsMoving, false);
        private void OnMoveBeginEvent() => _m.animator.SetBool(IsMoving, true);

        private void Start()
        {
            ExecutionSystem.AddUpdate(this);
            _myMat = _m.skullsRenderer.material;
        }

        public void OnUpdate()
        {
        }

        private void OnDestroy()
        {
            ExecutionSystem.RemoveUpdate(this, true);
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

        private void OnHealEvent()
        {
        }

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

                foreach (var rCollider in _ragdollColliders)
                {
                    rCollider.enabled = true;
                }

                foreach (var rRigidbody in _ragdollRigidbodies)
                {
                    rRigidbody.isKinematic = false;
                    rRigidbody.AddExplosionForce(_ragdollAppliedForce, _c.Position + _c.Transform.forward, 5f);
                }

                return;
            }

            foreach (var rCollider in _ragdollColliders)
            {
                rCollider.enabled = false;
            }

            foreach (var rRigidbody in _ragdollRigidbodies)
            {
                rRigidbody.isKinematic = true;
            }
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