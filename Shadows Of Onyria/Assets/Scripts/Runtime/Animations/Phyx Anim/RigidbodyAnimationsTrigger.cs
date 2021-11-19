using System;
using UnityEngine;

namespace DoaT
{
    public class RigidbodyAnimationsTrigger : MonoBehaviour
    {
        public PhysicsAnimationClusterData dataHolder;
        public RigidbodyAnimationPlayer[] players;

        public bool playOnSetActive = false;
        public float playSpeed;
        public bool canTrigger = true;

        private void Awake()
        {
            players = GetComponentsInChildren<RigidbodyAnimationPlayer>();

            foreach (var p in players)
            {
                p.SetData(dataHolder.GetAnimationData(p.gameObject.name), playSpeed);
            }
        }

        private void OnEnable()
        {
            if (!Application.isPlaying || !playOnSetActive || !canTrigger) return;
            
            canTrigger = false;
            Play();
        }

        public void Reset()
        {
            foreach (var player in players)
            {
                player.Reset();
            }
        }
        
        public void Play()
        {
            canTrigger = false;
            foreach (var player in players)
            {
                player.Play();
            }
        }
    }
}
