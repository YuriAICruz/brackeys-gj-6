using System;
using System.Collections.Generic;
using System.Gameplay;
using Graphene.Time;
using Models.Interfaces;
using Models.ModelView;
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

        [SerializeField] private int maxHp;
        private Coroutine _jumpAnimation;
        public int Hp { get; private set; }

        public void SetupWeapon(IWeapon weapon)
        {
            if (inventory.weapon != null)
                inventory.weapon.Discard();
            inventory.weapon = weapon;
        }

        protected virtual void Awake()
        {
            Hp = maxHp;
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

            var elapsed = Timer.time - states.lastAttack;

            if (states.attacking)
            {
                if (elapsed > stats.attacks[states.attackStage].delay +
                    stats.attacks[states.attackStage].damageDuration * 0.6f)
                    _attackQueue.Enqueue(Timer.time);

                if (_attack != null)
                    return;
            }

            if (elapsed > stats.attackInputDelay)
            {
                states.attackStage = 0;
            }

            if (!_physics.Grounded && states.attackStage > 0)
            {
                _attackQueue.Clear();
                return;
            }

            states.attacking = true;
            var t = Timer.time;

            if (_attack != null)
                _timer.Stop(_attack);

            _signalBus.Fire(new Models.Signals.Actor.Attack(states.attackStage,
                stats.attacks[states.attackStage]));

            CalculateDirection();
            var dir = GetNormalizedDirection();
            TurnTo(dir);

            _attack = _timer.Wait(() =>
            {
                if (!states.attacking)
                    return true;

                var elapsed = Timer.time - t;
                return elapsed > stats.attacks[states.attackStage].duration;
            }, () =>
            {
                _attack = null;
                states.attackStage = (states.attackStage + 1) % stats.attacks.Length;
                states.lastAttack = Timer.time;


                if (!_physics.Grounded)
                {
                    _attackQueue.Clear();
                }
                else if (_attackQueue.Count > 0)
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

            var dir = GetNormalizedDirection();

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

        private Vector3 GetNormalizedDirection()
        {
            var dir = states.direction;
            if (states.direction.sqrMagnitude < 0.1f)
                dir = new Vector3(transform.forward.x, transform.forward.z);

            dir.Normalize();
            return dir;
        }

        protected virtual void Jump()
        {
            if (states.jumping) return;

            states.jumping = true;
            _physics.Jump(stats.jumpForce);


            var t = Timer.time;

            if (_jumpAnimation != null)
                _timer.Stop(_jumpAnimation);

            _jumpAnimation = _timer.Wait(() =>
            {
                var elapsed = Timer.time - t;

                if (elapsed > stats.maxJumpTime || !states.jumping)
                    return true;

                return false;
            }, StopJump);
        }

        protected virtual void StopJump()
        {
            if (_jumpAnimation != null)
                _timer.Stop(_jumpAnimation);

            states.jumping = false;
            _physics.StopJump();
        }

        public void Damage(int damage)
        {
            Hp -= damage;
            _signalBus.Fire(new Models.Signals.Actor.Damage(damage, Hp));
        }
    }
}