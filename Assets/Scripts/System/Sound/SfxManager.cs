using System.Sound;
using Models.Signals;
using UnityEngine;
using Zenject;

namespace System
{
    public class SfxManager
    {
        private readonly SignalBus _signalBus;
        private readonly AudioSource _source;
        private readonly SfxSettings _settings;

        public SfxManager(SignalBus signalBus, AudioSource source, SfxSettings settings)
        {
            _signalBus = signalBus;
            _source = source;
            _settings = settings;
            
            _signalBus.Subscribe<SFX.Play>(Play);
        }

        private void Play(SFX.Play data)
        {
            throw new NotImplementedException();
        }
    }
}