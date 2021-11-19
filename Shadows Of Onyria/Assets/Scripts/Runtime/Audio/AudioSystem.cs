using UnityEngine;

namespace DoaT
{
    public class AudioSystem : MonoBehaviour
    {
        private static AudioSystem Current { get; set; }
        
        [SerializeField] private AudioDurationTracker _audioTrackerPrefab;
        
        private Pool<AudioDurationTracker> _audioSourcePool;

        private void Awake()
        {
            if (Current == null)
                Current = this;
            else
            {
                Destroy(this);
                return;
            }
            
            _audioSourcePool = new Pool<AudioDurationTracker>(_audioTrackerPrefab, 5, Factory, true);
        }

        private void StopAllCues()
        {
            var objects = _audioSourcePool.CurrentActiveObjects;
            
            foreach (var activeObj in objects)
            {
                activeObj.Deactivate();
            }
        }
        
        public static AudioDurationTracker PlayCue(AudioCue cue) => Current.PlayCueImpl(cue);
        private AudioDurationTracker PlayCueImpl(AudioCue cue)
        {
            var source = _audioSourcePool.GetObject();
            source.AudioSource.Setup(cue);
            source.Activate(Vector3.zero, Quaternion.identity);
            return source;
        }

        public static AudioDurationTracker PlayCue(AudioCue cue, Vector3 position) => Current.PlayCueImpl(cue, position);
        private AudioDurationTracker PlayCueImpl(AudioCue cue, Vector3 position)
        {
            var source = _audioSourcePool.GetObject();
            source.AudioSource.Setup(cue);
            source.Activate(position, Quaternion.identity);

            return source;
        }

        private AudioDurationTracker Factory(object prefab) => Instantiate((AudioDurationTracker)prefab, transform);
    }
}
