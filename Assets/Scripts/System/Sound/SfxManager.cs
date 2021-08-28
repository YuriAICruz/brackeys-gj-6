using System.Sound;
using Graphene.Time;
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
        private readonly TimeManager _timer;

        public SfxManager(SignalBus signalBus, AudioSource source, SfxSettings settings, TimeManager timer)
        {
            _signalBus = signalBus;
            _source = source;
            _settings = settings;
            _timer = timer;

            GameObject.DontDestroyOnLoad(_source);
            _signalBus.Subscribe<SFX.Play>(Play);
        }

        private void Play(SFX.Play data)
        {
            _timer.Wait(data.delay, () =>
            {
                //AudioSource.PlayClipAtPoint(_settings.Clip(data.Clip), data.position, _source.volume);
                _source.transform.position = data.position;
                _source.pitch = 1 + (UnityEngine.Random.value * 0.2f - 0.1f);
                _source.PlayOneShot(_settings.Clip(data.Clip));
            });
        }
    }
}