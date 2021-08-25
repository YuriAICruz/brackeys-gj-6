using System;
using System.Gameplay;
using Models.Interfaces;
using UnityEngine;
using Zenject;
using Physics = System.Gameplay.Physics;

namespace Presentation.Gameplay
{
    public class MeleeWeapon : Weapon
    {
        [Inject] private IPhysics _physics;
        [Inject] private PhysicsSettings _physicsSettings;

        [Space] public Vector3 colliderBase;
        public Vector3 colliderTip;
        public float radius;
        public int collisionPoints = 3;

        private Vector3 _initialDirection;
        private Vector3 _initialPoint;
        
        public LayerMask _mask;

        protected override void Awake()
        {
            base.Awake();

            //_mask =  (1 << _physicsSettings.hittable) | (1 << _physicsSettings.enemies);
            _mask =  _physicsSettings.hittable | _physicsSettings.enemies;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            var dir = transform.TransformDirection(colliderTip - colliderBase);
            Gizmos.DrawLine(transform.TransformPoint(colliderBase), transform.position + dir);

            for (int i = 0; i < collisionPoints; i++)
            {
                Gizmos.DrawWireSphere(transform.TransformPoint(colliderBase) + dir * i / (collisionPoints - 1), radius);
            }
        }

        protected override void Swing(Models.Signals.Actor.Attack signal)
        {
            base.Swing(signal);

            _initialPoint = transform.TransformPoint(colliderBase);
            _initialDirection = transform.TransformDirection(colliderTip - colliderBase);
        }

        protected override void SwingUpdate(float elapsed, float delta)
        {
            base.SwingUpdate(elapsed, delta);

            for (int i = 0; i < collisionPoints; i++)
            {
                var step = i / (collisionPoints - 1f);
                var currentDir = transform.TransformDirection(colliderTip - colliderBase);

                var pos = transform.TransformPoint(colliderBase) + currentDir * step;
                var ini = (_initialPoint + _initialDirection * step);
                var dir = pos - ini;

                Debug.DrawRay(ini, dir, Color.yellow, 1);
                Debug.DrawRay(transform.TransformPoint(colliderBase), currentDir * step, Color.red, 1);

                if (_physics.CheckSphere(ini, dir, radius, _mask, out RaycastHit hit))
                {
                    var damageable = hit.transform.GetComponent<IDamageable>();

                    if (damageable != null)
                    {
                        damageable.Damage(attributes.damage);
                        break;
                    }
                    
                    var breakable = hit.transform.GetComponent<IBreakable>();
                    if (breakable != null) 
                    {
                        breakable.Break();
                        continue;
                    }
                }
            }

            _initialPoint = transform.TransformPoint(colliderBase);
            _initialDirection = transform.TransformDirection(colliderTip - colliderBase);
        }
    }
}