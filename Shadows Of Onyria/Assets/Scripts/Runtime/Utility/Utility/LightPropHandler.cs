using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.VFX;

namespace DoaT
{
    public class LightPropHandler : MonoBehaviourInit, IPausable
    {
        [SerializeField] private Light _light;
        [SerializeField] private VisualEffect _vfx;

        private float _intensity;
        private HDAdditionalLightData _additionalLightData;
        private bool _isVFXNull;

        private void Awake()
        {
            if (_light == null) _light = GetComponent<Light>();
            if (_vfx == null) _vfx = GetComponent<VisualEffect>();
            
            _isVFXNull = _vfx == null;

            _additionalLightData = _light.GetComponent<HDAdditionalLightData>();
            _intensity = _light.intensity;
        }
        
        public override float OnInitialization()
        {
            ExecutionSystem.AddPausable(this);
            return 1f;
        }

        public void SetActiveState(bool isActive)
        {
            if (isActive)
            {
                _light.gameObject.SetActive(true);
                _light.enabled = true;
                
                if (_vfx == null) return;
                _vfx.gameObject.SetActive(true);
                _vfx.enabled = true;
                _vfx.Play();
            }
            else
            {
                _light.enabled = false;
                _light.gameObject.SetActive(false);
                
                if (_vfx == null) return;
                _vfx.Stop();
                _vfx.enabled = false;
                _vfx.gameObject.SetActive(false);
            }
        }
        public void SetIntensityStage(float range)
        {
            if (range < 0 || range > 1) range = Mathf.Clamp01(range);

            _light.intensity = Mathf.Lerp(0, _intensity, range);
        }

        public void OnGamePause()
        {
            if(!_isVFXNull) _vfx.playRate = 0;
        }
        public void OnGameResume()
        {
            if(!_isVFXNull) _vfx.playRate = 1;
        }

        private void OnDisable() => _additionalLightData.intensity = 0f;
        private void OnDestroy() => ExecutionSystem.RemovePausable(this);
    }
}