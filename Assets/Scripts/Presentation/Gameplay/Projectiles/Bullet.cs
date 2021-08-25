using System;
using System.Gameplay;
using Graphene.Time;
using Models.Interfaces;
using UnityEngine;
using Zenject;

namespace Presentation.Gameplay.Projectiles
{
    public class Bullet : MonoBehaviour
    {
        public bool Running { get; private set; }

        private Vector3 _direction;
        private float _speed;
        private Vector3 _lastPosition;

        public int damage = 1;
        public LayerMask mask;
        public float duration = 2;
        private float _time;
        private float _delay;
        [SerializeField]
        private float _radius;

        private Collider[] _collision;

        private void Awake()
        {
            _collision = new Collider[1];
            Deactivate();
        }

        private void Deactivate()
        {
            Running = false;

            MoveAway();
        }

        private void MoveAway()
        {
            transform.position = Vector3.one * -999;
        }

        public virtual void Shoot(Vector3 position, Vector3 dir, float speed, float delay)
        {
            MoveAway();
            Running = true;
            _direction = dir;
            _speed = speed;
            _delay = delay;

            _lastPosition = position;

            _time = Timer.time;
        }

        protected virtual void Update()
        {
        }

        protected virtual void FixedUpdate()
        {
            if(!Running) return;
            
            if(Timer.time - _time > duration+_delay)
            {
                Deactivate();
                return;
            }
            
            if(Timer.time - _time < _delay)
            {
                return;
            }
            
            var pos = _lastPosition;

            pos += _direction * (_speed * Timer.fixedDeltaTime);
            
            var dir = pos - _lastPosition;

            Debug.DrawRay(_lastPosition, dir, Color.red, duration);
            
            if (UnityEngine.Physics.Raycast(new Ray(_lastPosition, dir), out var hit, dir.magnitude*2, mask))
            {
                var damageable = hit.transform.GetComponent<IDamageable>();
                damageable?.Damage(damage);
                
                Deactivate();
                return;
            }
            
            var hits = UnityEngine.Physics.OverlapSphereNonAlloc(_lastPosition, _radius, _collision, mask);
            for (int i = 0; i < hits; i++)
            {
                var damageable = _collision[i].GetComponent<IDamageable>();
                damageable?.Damage(damage);
                
                Deactivate();
                return;
            }

            _lastPosition = pos;
            transform.position = pos;
        }
    }
}