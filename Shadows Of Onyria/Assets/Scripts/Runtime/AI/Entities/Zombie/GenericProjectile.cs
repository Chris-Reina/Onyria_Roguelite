using System;
using UnityEngine;

namespace DoaT
{
    public class GenericProjectile : Projectile
    {        
        [SerializeField] protected float _speed = 10;
        [SerializeField] protected bool _distanceBased = false;
        [SerializeField] protected float _maxDistance = 15f;
        [SerializeField] protected Rigidbody _rb;

        [SerializeField] protected bool _destroyOnHit = true;
        [SerializeField] protected LayerMask HitMask;
        [SerializeField] protected LayerMask DestroyMask;

        protected Vector3 _initialPosition;
        protected bool _isDisposed;

        protected override void Awake()
        {
            base.Awake();
            if (!_rb) _rb = GetComponent<Rigidbody>();
        }

        #region Initialize
        public override void Initialize()
        {
            Initialize(transform.forward, _speed);
        }
        public override void Initialize(float initialSpeed)
        {
            Initialize(transform.forward, initialSpeed);
        }
        public override void Initialize(Vector3 directionalSpeed)
        {
            Initialize(directionalSpeed, 1);
        }
        public override void Initialize(Vector3 direction, float speed)
        {
            _rb.velocity = direction * speed;
            _initialPosition = transform.position;
            transform.rotation = Quaternion.LookRotation(direction);
        }
        #endregion

        public override void SetInfo(RangeAttackInfo info)
        {
            base.SetInfo(info);
            
            if (info.maxDistance > 0)
            {
                _distanceBased = true;
                _maxDistance = info.maxDistance;
            }
        }

        public override float GetSpeed()
        {
            return _speed;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (HitMask.ContainsLayer(other.gameObject.layer))
            {
                other.gameObject.GetComponent<IAttackable>().TakeDamage(_info, _damageType);
                if (_destroyOnHit)
                {
                    CollisionFeedback();
                    DestroyProjectile(this);
                    return;
                }
            }
            
            if (DestroyMask.ContainsLayer(other.gameObject.layer))
            {
                CollisionFeedback();
                DestroyProjectile(this);
                return;
            }
        }

        public override void OnUpdate()
        {
            if (_isDisposed) return;
            if (!_distanceBased) return;
            var distance = (_initialPosition - transform.position).sqrMagnitude;
            
            if (distance >= _maxDistance * _maxDistance)
            {
                DestroyProjectile(this);
            }
        }

        public override void DestroyProjectile(IUpdate u)
        {
            base.DestroyProjectile(u);
            _isDisposed = true;
        }

        private Vector3 pause_Velocity;
        public override void OnGamePause()
        {
            base.OnGamePause();
            if (_rb != null)
            {
                pause_Velocity = _rb.velocity;
                _rb.velocity = new Vector3(0, 0, 0);
            }
        }

        public override void OnGameResume()
        {
            base.OnGameResume();
            if(_rb != null) _rb.velocity = pause_Velocity;
           
        }
    }
}