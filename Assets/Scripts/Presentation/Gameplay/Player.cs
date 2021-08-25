﻿using System.Gameplay;
using Graphene.Time;
using Models.Accessors;
using ModestTree;
using UnityEngine;
using Zenject;

namespace Presentation.Gameplay
{
    public class Player : Actor
    {
        private Vector2 _direction;

        [Inject] private ICamera _camera;
        private Camera _currentCamera;

        protected override void Awake()
        {
            base.Awake();

            _camera.ActiveCamera.AddListener(SetCamera);

            _signalBus.Subscribe<InputSignal.Up>(OnInputEvent);
            _signalBus.Subscribe<InputSignal.Down>(OnInputEvent);
            _signalBus.Subscribe<InputSignal.Axes>(UpdateAxis);
            
            _physics.SetSphereRadius(stats.radius, stats.height);
        }

        private void SetCamera(Camera camera)
        {
            _currentCamera = camera;
        }

        private void UpdateAxis(InputSignal.Axes data)
        {
            _direction.x = data.values[1];
            _direction.y = data.values[0];
        }

        private void OnInputEvent(InputSignal.Down data)
        {
            switch (data.id)
            {
                case 0:
                    states.running = true;
                    break;
                case 1:
                    Attack();
                    break;
                case 2:
                    Dodge();
                    break;
                case 3:
                    Jump();
                    break;
            }
        }

        private void OnInputEvent(InputSignal.Up data)
        {
            switch (data.id)
            {
                case 0:
                    states.running = false;
                    break;
                case 3:
                    StopJump();
                    break;
            }
        }

        protected override void AerialAttack()
        {
            states.attacking = true;
            if (_attack != null)
                _timer.Stop(_attack);

            states.attackStage = stats.aerialAttackStage;
            _signalBus.Fire(new Models.Signals.Actor.Attack(states.attackStage, stats.aerialAttack));

            _attack = _timer.Wait(stats.aerialAttack.delay, () =>
            {
                var delta = Timer.time;
                var t = Timer.time;
                _attack = _timer.Wait(() =>
                {
                    var elapsed = Timer.time - t;

                    transform.position = _physics.Drop(transform.position, Timer.time - delta);
                    delta = Timer.time;

                    if (_physics.Grounded || elapsed >= stats.aerialAttack.damageDuration)
                        return true;

                    return false;
                }, () =>
                {
                    _attack = null;

                    states.attackStage = 0;
                    states.lastAttack = Timer.time;

                    _attackQueue.Clear();

                    states.attacking = false;
                });
            });
        }

        public override void Damage(int damage)
        {
            base.Damage(damage);
            
            if(Hp <=0)
                _signalBus.Fire<Models.Signals.Player.Death>();
        }

        protected override void CalculateDirection()
        {
            var dir = _currentCamera.transform.TransformDirection(_direction);
            // dir.y = dir.z;
            // dir.z = 0;
            // dir.Normalize();
            states.direction = dir;
        }

        protected override void Move(float delta)
        {
            if (_currentCamera == null) return;

            transform.position = _physics.Evaluate(states.direction * (states.running ? stats.runSpeed : stats.speed),
                transform.position, delta);

            states.currentSpeed = states.direction.magnitude * (states.running ? 2 : 1);
            states.grounded = _physics.Grounded;
        }
    }
}