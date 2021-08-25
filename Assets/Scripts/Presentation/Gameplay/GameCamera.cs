using System;
using System.Gameplay;
using Models.Accessors;
using UnityEngine;
using Zenject;

namespace Presentation.Gameplay
{
    [RequireComponent(typeof(Camera))]
    public class GameCamera : MonoBehaviour
    {
        public bool main;
        public float speed;
        public float minDistance;

        [Inject] private ICamera _camera;
        [Inject] private GameManager _gameManager;
        private Camera _cameraComponent;
        private Quaternion _direction;

        private void Awake()
        {
            _cameraComponent = GetComponent<Camera>();
            if (main)
                _camera.ActiveCamera.Commit(_cameraComponent);
            
            //var dir = _gameManager.Player.Center - transform.position;
            var dir = transform.forward;
            _direction = Quaternion.LookRotation(dir);
        }

        private void Update()
        {
            var dir = _gameManager.Player.Center - transform.position;
            dir.x = 0;

            if (dir.magnitude < minDistance) return;
            
            var look = Quaternion.LookRotation(dir);

            _direction = Quaternion.Lerp(_direction, look, speed*Time.deltaTime);
            transform.rotation = _direction;
        }
    }
}