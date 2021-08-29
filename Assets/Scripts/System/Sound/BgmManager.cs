using Models.Signals;
using UnityEngine;
using Zenject;

namespace System.Sound
{
    public class BgmManager
    {
        private readonly SignalBus _signalBus;
        private readonly AudioSource _source;
        private readonly BgmSettings _settings;

        public BgmManager(SignalBus signalBus, AudioSource source, BgmSettings settings)
        {
            _signalBus = signalBus;
            _source = source;
            _settings = settings;
            
            GameObject.DontDestroyOnLoad(_source);
            _signalBus.Subscribe<Bgm.Play>(Play);
        }

        private void Play(Bgm.Play data)
        {
            _source.clip = _settings.Clip(data.Clip);
            _source.Play();
        }
    }
}