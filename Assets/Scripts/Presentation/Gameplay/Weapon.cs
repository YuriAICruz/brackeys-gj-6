using System;
using Graphene.Time;
using Models.Interfaces;
using Models.ModelView;
using UnityEngine;
using Zenject;

namespace Presentation.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Weapon : MonoBehaviour, IWeapon
    {
        private Rigidbody _rigidBody;
        private Collider _collider;

        public bool fromPlayer;

        public IActor Owner { get; private set; }

        [Inject] private ITimeManager _timer;
        [Inject] private SignalBus _signalBus;
        private Coroutine _swingAnimation;

        public WeaponAttributes attributes;

        protected virtual void Awake()
        {
            Owner = transform.GetComponentInParent<IActor>();

            Owner.SetupWeapon(this);

            _rigidBody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();

            _signalBus.Subscribe<Models.Signals.Actor.Attack>(Swing);
            _collider.enabled = false;
        }

        protected virtual void Swing(Models.Signals.Actor.Attack signal)
        {
            _collider.enabled = true;

            if (_swingAnimation != null)
                _timer.Stop(_swingAnimation);

            var t0 = 0f;
            _swingAnimation = _timer.Wait(signal.data.delay, () =>
            {
                _swingAnimation = _timer.Wait(signal.data.damageDuration, t =>
                {
                    SwingUpdate(t, t - t0);
                    t0 = t;
                }, SwingEnd);
            });
        }

        protected virtual void SwingUpdate(float elapsed, float delta)
        {
        }

        protected virtual void SwingEnd()
        {
            _collider.enabled = false;
        }

        public void Discard()
        {
            Destroy(gameObject);
        }
    }
}