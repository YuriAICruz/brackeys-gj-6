using System.Gameplay;
using Models.Signals;
using UnityEngine;
using Zenject;

namespace Presentation.Effects
{
    public class ParticlesManager
    {
        private readonly SignalBus _signalBus;
        private readonly FxSettings _fxSettings;

        public ParticlesManager(SignalBus signalBus, FxSettings fxSettings)
        {
            _signalBus = signalBus;
            _fxSettings = fxSettings;
            
            _signalBus.Subscribe<FX.Hit>(Hit);
            _signalBus.Subscribe<FX.Puff>(Puff);
            _signalBus.Subscribe<FX.Slash>(Slash);
            _signalBus.Subscribe<FX.Smoke>(Smoke);
        }

        private void Hit(FX.Hit data)
        {
            Object.Instantiate(_fxSettings.hit, data.position, Quaternion.identity);
        }

        private void Puff(FX.Puff data)
        {
            Object.Instantiate(_fxSettings.puff, data.position, Quaternion.identity);
        }

        private void Slash(FX.Slash data)
        {
            Object.Instantiate(_fxSettings.slash, data.position, Quaternion.identity);
        }

        private void Smoke(FX.Smoke data)
        {
            Object.Instantiate(_fxSettings.smokeScreen, data.position, Quaternion.identity);
        }
    }
}