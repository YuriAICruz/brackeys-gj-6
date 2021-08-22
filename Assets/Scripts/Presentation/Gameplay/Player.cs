using System.Gameplay;
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
        private Vector3 _lookDirection;

        protected override void Awake()
        {
            base.Awake();

            _camera.ActiveCamera.AddListener(SetCamera);

            _signalBus.Subscribe<InputSignal.Up>(OnInputEvent);
            _signalBus.Subscribe<InputSignal.Down>(OnInputEvent);
            _signalBus.Subscribe<InputSignal.Axes>(UpdateAxis);
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

        private void OnInputEvent(InputSignal.Up data)
        {
        }

        private void OnInputEvent(InputSignal.Down data)
        {
        }

        protected override void FixedUpdate()
        {
            if (_currentCamera == null) return;

            //var dir = _currentCamera.transform.TransformDirection(new Vector3(_direction.x, 0, _direction.y));
            var dir = _currentCamera.transform.TransformDirection(_direction);
            dir.y = dir.z;
            dir.z = 0;
            dir.Normalize();
            transform.position = _physics.Evaluate(dir * stats.speed, transform.position, Time.fixedDeltaTime);

            TurnTo(dir);
        }

        private void TurnTo(Vector2 direction)
        {
            var dir = new Vector3(direction.x, 0, direction.y);

            if (dir.sqrMagnitude > 0.15f)
                _lookDirection = dir;
            if (_lookDirection.sqrMagnitude < 0.15f)
                return;

            var look = Quaternion.LookRotation(_lookDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, look, Time.fixedDeltaTime * stats.turnSpeed);
        }
    }
}