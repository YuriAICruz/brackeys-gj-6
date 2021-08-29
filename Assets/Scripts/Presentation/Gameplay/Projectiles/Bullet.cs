using System;
using System.ComponentModel;
using System.Gameplay;
using Graphene.Time;
using Models.Interfaces;
using Models.Signals;
using UnityEngine;
using Zenject;

namespace Presentation.Gameplay.Projectiles
{
    public class Bullet : MonoBehaviour
    {
        public class Factory : PlaceholderFactory<Bullet>
        {
            private readonly DiContainer _container;

            public Factory(DiContainer container)
            {
                _container = container;
            }
            
            public override Bullet Create()
            {
                return base.Create();
            }
            public Bullet Create(Bullet prefab)
            {
                var instance = _container.InstantiatePrefab(prefab).GetComponent<Bullet>();
                
                return instance;
            }
        }

        [Inject] private SignalBus _signalBus;

        public bool Running { get; private set; }

        private Vector3 _direction;
        private Vector3 _lastPosition;

        public int damage = 1;
        public LayerMask mask;
        public float duration = 2;
        private float _speed;
        
        private float _time;
        private protected float _delay;
        [SerializeField] private float _radius;

        private Collider[] _collision;

        private void Awake()
        {
            _collision = new Collider[1];
            Deactivate();
        }

        protected virtual void Deactivate()
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
            this._speed = speed;
            _delay = delay;

            _lastPosition = position;

            _time = Timer.time;
        }

        protected virtual void Update()
        {
        }

        protected virtual void FixedUpdate()
        {
            if (!Running) return;

            if (Timer.time - _time > duration + _delay)
            {
                Deactivate();
                return;
            }

            if (Timer.time - _time < _delay)
            {
                return;
            }

            var pos = _lastPosition;

            pos += GetDirection(_direction) * (_speed * Timer.fixedDeltaTime);

            var dir = pos - _lastPosition;

            Debug.DrawRay(_lastPosition, dir, Color.red, duration);

            if (UnityEngine.Physics.Raycast(new Ray(_lastPosition, dir), out var hit, dir.magnitude * 2, mask))
            {
                var damageable = hit.transform.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    _signalBus.Fire(new FX.Hit(hit.point));
                    _signalBus.Fire(new Score.OnHit(hit.transform.gameObject));
                    damageable.Damage(damage);
                }

                Deactivate();
                return;
            }

            var hits = UnityEngine.Physics.OverlapSphereNonAlloc(_lastPosition, _radius, _collision, mask);
            for (int i = 0; i < hits; i++)
            {
                var damageable = _collision[i].GetComponent<IDamageable>();
                if (damageable != null)
                {
                    _signalBus.Fire(new FX.Hit(_collision[i].ClosestPoint(transform.position)));
                    damageable.Damage(damage);
                }

                Deactivate();
                return;
            }

            _lastPosition = pos;
            transform.position = pos;
        }

        protected virtual Vector3 GetDirection(Vector3 dir)
        {
            return dir;
        }
    }
}