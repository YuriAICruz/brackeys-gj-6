using System;
using System.Gameplay;
using Graphene.Time;
using Models.Interfaces;
using Models.Signals;
using UnityEngine;
using Zenject;

namespace Presentation.Gameplay
{
    public class HealthPillow : MonoBehaviour, IInteractable
    {
        [Inject] private SignalBus _signalBus;
        [Inject] private ITimeManager _timeManager;
        [Inject] private GameSettings _gameSettings;
        private Coroutine _animation;

        public InteractionType type;
        
        public bool CanActivate { get; private set; }
        public InteractionType EffectType => type;

        private void Awake()
        {
            CanActivate = true;
        }

        public void Activate(Action onEnd)
        {
            CanActivate = false;

            _signalBus.Fire(new FX.Smoke(transform.position));

            _animation = _timeManager.Wait(_gameSettings.healDuration, () =>
            {
                onEnd?.Invoke();
            });
        }

        public void Cancel()
        {
            if (_animation != null)
                _timeManager.Stop(_animation);
        }
    }
}