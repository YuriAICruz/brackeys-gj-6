using System;
using UnityEngine;
using Zenject;

namespace Presentation.Effects
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleFx : MonoBehaviour
    {
        public float duration;
        public float destroyDelay;
        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            Invoke(nameof(StartDestruction), duration);
        }

        private void StartDestruction()
        {   
            _particleSystem.Stop();
            Invoke(nameof(DestroySelf), destroyDelay);
        }
        
        private void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}