using System;
using Graphene.Time;
using UnityEngine;
using Zenject;

namespace Presentation.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class Weapon : MonoBehaviour, IWeapon
    {
        private Rigidbody _rigidBody;
        private Collider _collider;
        
        public IActor Owner { get; private set; }

        [Inject] private ITimeManager _timer;
        private Coroutine _swingAnimation;

        private void Awake()
        {
            Owner = transform.GetComponentInParent<IActor>();
            _rigidBody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();

            _collider.enabled = false;
        }
        
        public void Swing(float duration)
        {
            _collider.enabled = true;

            if(_swingAnimation != null) 
                _timer.Stop(_swingAnimation);
            
            _swingAnimation = _timer.Wait(duration, () => { _collider.enabled = false; });
        }
    }
}