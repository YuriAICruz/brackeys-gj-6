﻿using System.Gameplay;
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
                    states.jumping = true;
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
                    states.jumping = false;
                    StopJump();
                    break;
            }
        }

        protected override void CalculateDirection()
        {
            var dir = _currentCamera.transform.TransformDirection(_direction);
            dir.y = dir.z;
            dir.z = 0;
            dir.Normalize();
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