using System;
using UnityEngine;
using UnityEngine.VFX;

namespace DoaT
{
    public class LightDampener : MonoBehaviour, IUpdate, IUnloadable
    {
        public new Light light;
        public VisualEffect _vfx;
        public bool dampen = true;

        public float distance = 50f;

        private TheodenController _target;

        private void Awake()
        {
            light = GetComponent<Light>();
        }

        private void Start()
        {
            ExecutionSystem.AddUpdate(this);
            EventManager.Subscribe(GameEvents.OnSceneUnload, Unload);
            
            _target = World.GetPlayer();
        }

        public void OnUpdate()
        {
            if (!dampen && !light.enabled)
            {
                light.enabled = true;
                if (_vfx != null) _vfx.Play();
                return;
            }

            var a = _target.Position;
            var b = transform.position;

            a.y = 0;
            b.y = 0;

            var isFar = (b - a).sqrMagnitude > Mathf.Pow(distance, 2);
            switch (isFar)
            {
                case true when light.enabled:
                {
                    light.enabled = false;
                    if (_vfx != null) _vfx.Stop();
                    break;
                }
                case false when !light.enabled:
                {
                    light.enabled = true;
                    if (_vfx != null) _vfx.Play();
                    break;
                }
            }
        }

        public void Unload(params object[] parameters)
        {
            ExecutionSystem.RemoveUpdate(this, true);
        }

        private void OnDestroy()
        {
            EventManager.Unsubscribe(GameEvents.OnSceneUnload, Unload);
        }
    }
}
