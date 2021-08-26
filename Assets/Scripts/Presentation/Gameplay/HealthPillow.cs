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

        private int _used;

        public GameObject full, destroyed;

        public InteractionType type;
        
        public bool CanActivate { get; private set; }
        public InteractionType EffectType => type;

        private void Awake()
        {
            CanActivate = true;
            full.SetActive(true);
            destroyed.SetActive(false);
        }

        public void Activate(Action onEnd)
        {
            CanActivate = false;

            _signalBus.Fire(new FX.Smoke(transform.position));

            _animation = _timeManager.Wait(_gameSettings.healDuration, () =>
            {
                _used++;
                if (_used == 1)
                {
                    full.SetActive(false);
                    destroyed.SetActive(true);
                    CanActivate = true;
                }else if (_used > 1)
                {
                    full.SetActive(false);
                    destroyed.SetActive(false);
                }
                onEnd?.Invoke();
            });
        }

        public void Cancel()
        {
            if (_animation != null)
                _timeManager.Stop(_animation);
            
            if(_used < 2)
                CanActivate = true;
        }
    }
}