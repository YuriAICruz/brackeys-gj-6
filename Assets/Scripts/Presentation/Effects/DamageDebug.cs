using System;
using Graphene.Time;
using Models.Signals;
using UnityEngine;
using Zenject;

namespace Presentation.Effects
{
    [RequireComponent(typeof(Renderer))]
    public class DamageDebug : MonoBehaviour
    {
        [Inject] private SignalBus _signalBus;
        [Inject] private ITimeManager _timer;
        private Material _material;
        private Coroutine _blinkAnimation;

        private void Awake()
        {
            _signalBus.Subscribe<Models.Signals.Actor.Damage>(Blink);

            _material = GetComponent<Renderer>().material;
        }

        private void Blink(Actor.Damage data)
        {
            _material.color = Color.red;
            
            if (_blinkAnimation != null)
                _timer.Stop(_blinkAnimation);
            
            _blinkAnimation = _timer.Wait(0.2f, () => _material.color = Color.white);
        }
    }
}