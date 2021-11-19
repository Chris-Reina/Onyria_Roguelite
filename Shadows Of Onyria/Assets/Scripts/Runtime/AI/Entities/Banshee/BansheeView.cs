using System.Timers;
using UnityEngine;
using UnityEngine.VFX;

namespace DoaT.AI
{
    public class BansheeView : MonoBehaviour, IUpdate, IUnloadable, IPausable
    {
        private BansheeModel _m;
        private BansheeController _c;

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
        [SerializeField] private VisualEffect _hitEffectPrefab;
        [SerializeField] private Transform _effectSpawnPoint;
        private Material _myMat;
        private static readonly int SummonSpeedMultiplier = Animator.StringToHash("SummonSpeedMultiplier");


        private void Awake()
        {
            _m = GetComponent<BansheeModel>();
            _c = GetComponent<BansheeController>();

            _c.OnDamageTaken += OnTakeDamageEvent;

            _c.OnMoveBegin += OnMoveBeginEvent;
            _c.OnMoveEnd += OnMoveEndEvent;

            _c.OnSummonBegin += SummonBeginEvent;
            //_c.OnSummonBegin += OnAttackEffectEvent;
            //_c.OnAttackEnd += OnAttackEndEvent;

            //_c.OnStun += () => _m.animator.SetTrigger(Stun);


            _c.OnDeath += OnDeath;
            //_c.OnHeal += OnHealEvent;
        }
        
        private void OnDeath()
        {
            //SetRagdollEffect(true);
            _m.animator.SetBool(IsDead, true);
            TimerManager.SetTimer(new TimerHandler(), () => gameObject.SetActive(false), 1.2f);
        }

        private void SummonBeginEvent(float x)
        {
            _m.animator.SetFloat(SummonSpeedMultiplier, x);
            _m.animator.SetTrigger(Attack);
        } 
        private void OnAttackEffectEvent(float f)
        {
            _m.animator.SetFloat(SummonSpeedMultiplier, f);
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
            if (damageOnHealth)
            {
                _m.animator.SetTrigger(Stun);
            }

            AudioSystem.PlayCue(_damageCue);
            var hitEffect = Instantiate(_hitEffectPrefab); //TODO: FIX (?)
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