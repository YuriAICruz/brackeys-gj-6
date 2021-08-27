using System;
using System.Collections.Generic;
using System.Gameplay;
using Graphene.Time;
using Models.Accessors;
using Models.Interfaces;
using Models.ModelView;
using Models.Signals;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Presentation.Gameplay
{
    public class Actor : MonoBehaviour, IActor, IActorData, IDamageable
    {
        [Inject] protected IPhysics _physics;
        [Inject] protected SignalBus _signalBus;
        [Inject] protected ITimeManager _timer;

        private Observer<int> _dataHp = new Observer<int>();

        public Transform Transform => transform;
        public Vector3 Position => transform.position;
        public virtual Vector3 Center => transform.position + Vector3.up * stats.radius;
        public virtual Vector3 Direction => states.direction;

        public int MaxHp => stats.maxHp;

        Observer<int> IActorData.Hp => _dataHp;

        public ActorStatistics stats;
        public ActorStates states;
        public Inventory inventory;

        private protected Queue<float> _attackQueue = new Queue<float>();
        private protected Coroutine _attack;

        private Coroutine _jumpAnimation;
        private protected bool _running;

        public int Hp => _dataHp.GetValue();


        public void SetupWeapon(IWeapon weapon)
        {
            inventory.weapon?.Discard();

            inventory.weapon = weapon;
        }

        protected virtual void Awake()
        {
            DisableActor();

            _signalBus.Subscribe<Game.Start>(SetUpActor);
            _signalBus.Subscribe<Game.End>(DisableActor);
        }

        protected virtual void Start()
        {
        }


        protected virtual void OnTriggerEnter(Collider other)
        {
            states.onTrigger = other;
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            states.onTrigger = null;
        }


        protected virtual void SetUpActor()
        {
            _dataHp.Commit(stats.maxHp);
            _running = true;
        }

        protected virtual void DisableActor()
        {
            _running = false;
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void Update()
        {
            //if (!_running) return;
        }

        protected void FixedUpdate()
        {
            if (!_running || states.attacking || states.dodging || states.stag || states.activating ||
                states.dead) return;

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


        protected virtual void TurnTo(Vector3 direction, float delta)
        {
            direction.y = 0;

            if (direction.sqrMagnitude > 0.15f)
            {
                states.direction = direction;
            }

            if (states.direction.sqrMagnitude < 0.15f)
                return;

            var look = Quaternion.LookRotation(states.direction);
            var fwd = transform.forward;
            transform.rotation =
                Quaternion.Lerp(transform.rotation, look, delta * stats.turnSpeed * (states.running ? 2 : 1));

            states.turnAngle = Vector3.Angle(transform.forward, fwd);
        }


        protected virtual void Attack()
        {
            if (states.dodging)
                return;

            var elapsed = Timer.time - states.lastAttack;

            if (states.attacking && _physics.Grounded)
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

            if (!_physics.Grounded)
            {
                if (states.attackStage > 0)
                {
                    _attackQueue.Clear();
                    return;
                }

                if (!states.attacking)
                {
                    AerialAttack();
                    return;
                }
            }

            states.attacking = true;

            if (_attack != null)
                _timer.Stop(_attack);

            _signalBus.Fire(new Models.Signals.Actor.Attack(states.attackStage,
                stats.attacks[states.attackStage]));

            CalculateDirection();
            var dir = GetNormalizedDirection();
            TurnTo(dir);

            var t = Timer.time;
            _attack = _timer.Wait(() => { return AttackIsRunning(t, stats.attacks[states.attackStage]); }, () =>
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

        protected virtual void AerialAttack()
        {
            states.attacking = false;
            _attackQueue.Clear();
        }

        private bool AttackIsRunning(float t, AttackAnimation attack)
        {
            if (!states.attacking)
                return true;

            var elapsed = Timer.time - t;
            return elapsed > attack.duration;
        }

        protected virtual void Dodge()
        {
            if (states.dodging || !_physics.Grounded)
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

            if (_physics.Grounded)
                _signalBus.Fire(new FX.Puff(transform.position));

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

        public virtual void Damage(int damage)
        {
            if (states.activating)
                DisableActivation();

            if (states.damaged || states.dodging) return;

            var hp = _dataHp.GetValue();

            hp -= damage;
            _dataHp.Commit(hp);

            _signalBus.Fire(new Models.Signals.Actor.Damage(damage, Hp));

            states.damaged = true;
            states.stag = true;

            states.dead = hp <= 0;

            _timer.Wait(stats.damageStagDuration, () =>
            {
                states.stag = false;
                _timer.Wait(stats.damageInvincibilityDuration, () => { states.damaged = false; });
            });
        }

        private void Heal()
        {
            _dataHp.Commit(stats.maxHp);
        }

        protected void DisableActivation()
        {
            states.activating = false;

            if (!states.onTrigger) return;

            var interactable = states.onTrigger.GetComponent<IInteractable>();

            interactable?.Cancel();
        }

        protected void ActivationEffect(InteractionType type)
        {
            switch (type)
            {
                case InteractionType.Heal:
                    Heal();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}