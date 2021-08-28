using System;
using System.Gameplay;
using Models.Accessors;
using UnityEngine;
using Zenject;
using Physics = UnityEngine.Physics;

namespace Presentation.Gameplay
{
    [RequireComponent(typeof(Camera))]
    public class GameCamera : MonoBehaviour
    {
        public bool main;
        public float speed;
        public float rotationSpeed;
        public float minDistance;
        public float playerDistance;
        public Vector3 playerOffset;
        public float zoomMultiplier;

        [Inject] private ICamera _camera;
        [Inject] private GameManager _gameManager;
        [Inject] private PhysicsSettings _physicsSettings;
        private Camera _cameraComponent;
        private Quaternion _direction;
        private Vector3 _lastPosition;
        private Vector3 _noZ;
        private Vector3 _velocity;

        private void Awake()
        {
            _noZ = new Vector3(1, 1, 0);

            _cameraComponent = GetComponent<Camera>();
            if (main)
                _camera.ActiveCamera.Commit(_cameraComponent);

            //var dir = _gameManager.Player.Center - transform.position;
            var playerBosDir = _gameManager.Boss.Position - _gameManager.Player.Center;
            _lastPosition = _gameManager.Player.Center + (playerBosDir + playerOffset) * playerDistance;

            var dir = _gameManager.Player.Center - _lastPosition;
            _direction = Quaternion.LookRotation(dir);
        }

        private void Update()
        {
            var playerScreen = _cameraComponent.ScreenToWorldPoint(
                Vector3.Scale(_cameraComponent.WorldToScreenPoint(_gameManager.Player.Center), _noZ) +
                Vector3.forward * 0.1f
            );

            var bossScreen = _cameraComponent.ScreenToWorldPoint(
                Vector3.Scale(_cameraComponent.WorldToScreenPoint(_gameManager.Boss.Position), _noZ) +
                Vector3.forward * 0.1f
            );

            var bossHeadScreen = _cameraComponent.ScreenToWorldPoint(
                Vector3.Scale(_cameraComponent.WorldToScreenPoint(_gameManager.Boss.Center), _noZ) +
                Vector3.forward * 0.1f
            );

            //var screenDir = ((playerScreen - bossScreen) + (playerScreen - bossHeadScreen))/2;
            var screenDistance = Math.Max((playerScreen - bossScreen).magnitude,
                (playerScreen - bossHeadScreen).magnitude);

            var playerDir = (_gameManager.Player.Center - _lastPosition);
            var bossDir = (_gameManager.Boss.Position - _lastPosition);

            var playerBosDir = _gameManager.Boss.Position - _gameManager.Player.Center;

            var po = transform.TransformDirection(playerOffset);
            var fwd = playerBosDir.normalized;// _gameManager.Player.Transform.forward;
            var offset = (playerBosDir + po + fwd * (screenDistance * zoomMultiplier)) * playerDistance;

            var pos = _gameManager.Player.Center + offset;
            
            Debug.DrawRay(_gameManager.Player.Center, offset, Color.red);
            if (Physics.Raycast(new Ray(_gameManager.Player.Center, offset), out var hit, offset.magnitude,
                _physicsSettings.hittable | _physicsSettings.movementBlockers))
            {
                pos = hit.point - offset.normalized * 0.1f;
            }


            if ((pos - _lastPosition).magnitude < minDistance)
            {
                return;
            }

            _lastPosition = Vector3.SmoothDamp(_lastPosition, pos, ref _velocity, speed);

            //var dir = (_gameManager.Player.Center + _gameManager.Player.Direction * directionMagnitude) - _lastPosition;

            var dir = (playerDir + bossDir) / 2;
            //dir.x = 0;

            var look = Quaternion.LookRotation(dir);

            _direction = Quaternion.Slerp(_direction, look, rotationSpeed * Time.deltaTime);
            transform.rotation = _direction;
            transform.position = _lastPosition;
        }
    }
}