using System;
using UnityEngine;

namespace DoaT
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioDurationTracker : MonoBehaviour, IUpdate, IPoolSpawn, IPausable
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private bool _waitForLoopEnd;

        private Pool<AudioDurationTracker> _parentPool;
        public AudioSource AudioSource => _audioSource;

        private void Awake()
        {
            if(_audioSource == null)
                _audioSource = GetComponent<AudioSource>();
        }

        public void OnUpdate()
        {
            if (!gameObject.activeSelf || _waitForLoopEnd) return;
            if (!_audioSource.isPlaying)
            {
                Deactivate();
            }
        }
        
        public void EndLoop()
        {
            AudioSource.loop = false;
            _waitForLoopEnd = false;
        }
        public void SetParentPool<T>(T parent) //Answer to some Inheritance issues with looping interfaces         
        {
            _parentPool = parent as Pool<AudioDurationTracker>;
        }

        public void Activate(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            ExecutionSystem.AddUpdate(this);
            _audioSource.Play();
        }

        public void Deactivate()
        {
            ExecutionSystem.RemoveUpdate(this, false);
            _audioSource.Stop();
            _audioSource.loop = false;
            _audioSource.clip = null;
            _audioSource.volume = 1;
            _parentPool.ReturnObject(this);
        }
        
        public AudioDurationTracker Create()
        {
            Activate(default, default);
            return this;
        }

        public AudioDurationTracker SetAudioSource(AudioSource source)
        {
            _audioSource = source;
            return this;
        }

        public AudioDurationTracker SetWaitForLoopEnd(bool shouldWait)
        {
            _waitForLoopEnd = shouldWait;
            return this;
        }

        private void OnDestroy()
        {
            ExecutionSystem.RemoveUpdate(this, true);
        }

        public void OnGamePause()
        {
            _audioSource.Pause();
        }

        public void OnGameResume()
        {
            _audioSource.Play();
        }
    }
}
