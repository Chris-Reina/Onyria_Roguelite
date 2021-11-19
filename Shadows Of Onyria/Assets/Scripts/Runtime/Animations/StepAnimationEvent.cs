using System;
using UnityEngine;

namespace DoaT
{
    public class StepAnimationEvent : MonoBehaviour
    {
        [Tooltip("Event Name: StepSoundEvent")]
        public AudioCue TheodenStepCue;

        private bool _sound = true;
        
        private void Awake()
        {
            EventManager.Subscribe(GameEvents.OnSceneUnload, DisableSound);
        }

        private void DisableSound(params object[] obj)
        {
            _sound = false;
        }

        public void StepSoundEvent()
        {
            if (!_sound) return;
            
            AudioSystem.PlayCue(TheodenStepCue);
        }

        private void OnDestroy()
        {
            EventManager.Unsubscribe(GameEvents.OnSceneUnload, DisableSound);
        }
    }
}
