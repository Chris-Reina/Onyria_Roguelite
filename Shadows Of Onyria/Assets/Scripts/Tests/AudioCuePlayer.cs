using UnityEngine;

namespace DoaT
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioCuePlayer : MonoBehaviour
    {
        public AudioCue cue;

        private void Start()
        {
            var source = GetComponent<AudioSource>();
            source.Setup(cue);
            source.Play();
        }
    }
}
