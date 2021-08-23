using System;
using System.Collections.Generic;
using System.Gameplay;
using Graphene.Time;
using UnityEngine;
using Zenject;

namespace Presentation.Gameplay
{
    public class Actor : MonoBehaviour, IActor, IDamageable
    {
        [Inject] protected IPhysics _physics;
        [Inject] protected SignalBus _signalBus;
        [Inject] protected ITimeManager _timer;

        public ActorStatistics stats;
        public ActorStates states;
        public Inventory inventory;

        private Queue<float> _attackQueue = new Queue<float>();
        private Coroutine _attack;

        public void SetupWeapon(IWeapon weapon)
        {
            if (inventory.weapon != null)
                inventory.weapon.Discard();
            inventory.weapon = weapon;
        }
        
        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void Update()
        {
        }

        protected void FixedUpdate()
        {
            if (states.attacking || states.dodging) return;

            CalculateDirection();
            Move(Time.fixedDeltaTime);
            TurnTo(states.direction, Time.fixedDeltaTime);
        }

        protected virtual void CalculateDirection()
        {
        }

        protected virtual void Move(float delta)
        {
            transform.position = _physics.Evaluate(transform.position, delta);
        }

        protected virtual void TurnTo(Vector2 direction)
        {
            var dir = new Vector3(direction.x, 0, direction.y);

            var look = Quaternion.LookRotation(dir);
            transform.rotation = look;
        }


        protected virtual void TurnTo(Vector2 direction, float delta)
        {
            var dir = new Vector3(direction.x, 0, direction.y);

            if (dir.sqrMagnitude > 0.15f)
            {
                states.direction = dir;
            }

            if (states.direction.sqrMagnitude < 0.15f)
                return;

            var look = Quaternion.LookRotation(states.direction);
            var fwd = transform.forward;
            transform.rotation = Quaternion.Lerp(transform.rotation, look, delta * stats.turnSpeed);

            states.turnAngle = Vector3.Angle(transform.forward, fwd);
        }


        protected virtual void Attack()
        {
            if (states.dodging)
                return;


            var el = Timer.time - states.lastAttack;

            if (states.attacking)
            {
                if (el > stats.attacksDurations[states.attackStage] * 0.5f)
                    _attackQueue.Enqueue(Timer.time);

                if (_attack != null)
                    return;
            }

            if (el > stats.attackInputDelay)
            {
                states.attackStage = 0;
            }

            states.attacking = true;
            var t = Timer.time;

            if (_attack != null)
                _timer.Stop(_attack);
            
            inventory.weapon?.Swing(stats.attacksDurations[states.attackStage] * 0.6f);
            
            _attack = _timer.Wait(() =>
            {
                if (!states.attacking)
                    return true;

                var elapsed = Timer.time - t;
                return elapsed > stats.attacksDurations[states.attackStage];
            }, () =>
            {
                _attack = null;
                states.attackStage = (states.attackStage + 1) % stats.attacksDurations.Length;
                states.lastAttack = Timer.time;

                if (_attackQueue.Count > 0)
                {
                    var t = _attackQueue.Dequeue();
                    _attackQueue.Clear();
                    Attack();
                    return;
                }

                states.attacking = false;
            });
        }

        protected virtual void Dodge()
        {
            if (states.dodging)
                return;

            states.attacking = false;
            states.dodging = true;

            CalculateDirection();

            var dir = states.direction;
            if (states.direction.sqrMagnitude < 0.1f)
                dir = new Vector3(transform.forward.x, transform.forward.z);

            dir.Normalize();
            states.turnAngle = Vector3.Angle(transform.forward, new Vector3(dir.x, 0, dir.y));

            if (states.turnAngle < 90)
                TurnTo(dir);
            else
                TurnTo(-dir);

            var t0 = 0f;

            _timer.Wait(stats.dodgeDuration, (t) =>
            {
                transform.position = _physics.Evaluate(dir * stats.dodgeSpeed, transform.position, t - t0);
                t0 = t;
            }, () => { states.dodging = false; });
        }

        protected virtual void Jump()
        {
            _physics.Jump(stats.jumpForce);
        }

        protected virtual void StopJump()
        {
            _physics.StopJump();
        }
    }
}