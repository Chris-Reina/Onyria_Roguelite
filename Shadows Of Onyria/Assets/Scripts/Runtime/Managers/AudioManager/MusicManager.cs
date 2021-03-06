using System;
using UnityEngine;
using UnityEngine.Audio;

namespace DoaT
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { get; private set; }

        public PlayerStreamedData playerData;
        public AudioSource MainSource { get; private set; }
        public AudioSource SideSource { get; private set; }

        [SerializeField] private AudioSource _sourceOne = default;
        [SerializeField] private AudioSource _sourceTwo = default;
        [Space]
        [SerializeField] private float _transitionTime = 0.5f;
        [SerializeField] private float _musicVolume = 1;
        [SerializeField] private AudioMixerGroup _mixerGroup = default;
        [Space]
        [SerializeField] private AudioClip _defaultMusic = default;

        private bool _shouldChange = false;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            if (_sourceOne == null || _sourceTwo == null)
            {
                var sources = GetComponents<AudioSource>();
                if(sources.Length >= 2)
                {
                    _sourceOne = sources[0];
                    _sourceTwo = sources[1];
                }
                else
                {
                    if (_sourceOne == null)
                    {
                        _sourceOne = gameObject.AddComponent<AudioSource>();
                        _sourceOne.playOnAwake = false;
                    }

                    if (_sourceTwo == null)
                    {
                        _sourceTwo = gameObject.AddComponent<AudioSource>();
                        _sourceTwo.playOnAwake = false;
                    }
                }
            }

            _sourceOne.outputAudioMixerGroup = _mixerGroup;
            _sourceTwo.outputAudioMixerGroup = _mixerGroup;

            MainSource = _sourceOne;
            SideSource = _sourceTwo;

            SideSource.volume = 0;
        }

        private void Start()
        {
            if (MainSource.isPlaying || SideSource.isPlaying) return;
            
            MainSource.clip = _defaultMusic;
            MainSource.Play();
        }

        private void Update()
        {
            if (!(SideSource.volume < _musicVolume) || !_shouldChange) return;
            
            MainSource.volume -= _musicVolume / _transitionTime * Time.deltaTime;
            SideSource.volume += _musicVolume / _transitionTime * Time.deltaTime;

            //DebugManager.Log($"MainSource Volume : {MainSource.volume}  ---  MainSource clip : {MainSource.clip}");
            //DebugManager.Log($"SideSource Volume : {SideSource.volume}  ---  SideSource clip : {SideSource.clip}");

            if (!(SideSource.volume >= _musicVolume)) return;
            
            SideSource.volume = _musicVolume;
            _shouldChange = false;
            ChangeMain();

        }

        private void LateUpdate()
        {
            transform.position = playerData.Position;
        }

        public void ChangeMusicTo(AudioClip clip)
        {
            SideSource.clip = clip;
            SideSource.Play();

            _shouldChange = true;
        }

        private void ChangeMain()
        {
            if (MainSource == _sourceOne)
            {
                MainSource = _sourceTwo;
                SideSource = _sourceOne;
            }
            else
            {
                MainSource = _sourceOne;
                SideSource = _sourceTwo;
            }

            SideSource.Stop();
        }
    }
}
