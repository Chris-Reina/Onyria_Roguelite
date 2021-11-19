using UnityEngine;
using UnityEngine.VFX;

namespace DoaT
{
    public abstract class Projectile : MonoBehaviour, IUpdate, IUnloadable, IPausable
    {
        protected RangeAttackInfo _info;
        
        [SerializeField] protected DamageType _damageType;
        [SerializeField] protected VisualEffect _destroyEffect;
        [SerializeField] protected AudioCue _destroyCue;

        public abstract void Initialize();
        public abstract void Initialize(float initialSpeed);
        public abstract void Initialize(Vector3 directionalSpeed);
        public abstract void Initialize(Vector3 direction, float speed);

        protected virtual void Awake()
        {
            EventManager.Subscribe(GameEvents.OnSceneUnload, Unload);
            ExecutionSystem.AddUpdate(this);
        }

        public virtual void SetInfo(RangeAttackInfo info)
        {
            _info = info;
        }

        protected virtual void CollisionFeedback()
        {
            AudioSystem.PlayCue(_destroyCue);
            var a = Instantiate(_destroyEffect, transform.position - (transform.forward * 0.75f), Quaternion.identity);
            a.Play();
        }
        public abstract float GetSpeed();


        public virtual void OnUpdate() { }

        public virtual void Unload(params object[] parameters)
        {
            CollisionFeedback();
            ExecutionSystem.RemoveUpdate(this, false);
        }

        public virtual void DestroyProjectile(IUpdate u)
        {
            ExecutionSystem.RemoveUpdate(u, false);
            Destroy(gameObject);
        }
        protected virtual void OnDestroy()
        {
            ExecutionSystem.RemoveUpdate(this, true);
            EventManager.Unsubscribe(GameEvents.OnSceneUnload, Unload);
        }

        public virtual void OnGamePause() { }
        public virtual void OnGameResume() { }
    }
}